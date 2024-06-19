from Project.MissionDecomposition.code.custom_loggers import create_logger
from Project.TaskAllocation.code.agent import Agent
import voyager.utils as U
import logging
import asyncio
import json
import os


class SpeekingClubManager:
    def __init__(self,
        agents_list: list[Agent.__annotations__],
        chat_size = 2,
        max_iterations = 2,
        possible_actions = {
            "comment()" : "def comment() - Give your opinion about the speaker decision. Start conversation with this function",
            "request_join()": "def request_join() - Ask the speaker, if you can join him making the task together",
            "skip()": "def skip() - Gives another collegue, who didn't take his action, opportunity to speak",
            "request_change_task()": "def request_change_task() - If you disagree and don't support speaker decision, you can ask him to take his mission",
            "request_take_instruments()": "def request_take_instruments() - Help the speaker with instruments for his mission",
        },
        log_file_path = os.path.join("logs", "main.log"),
        log_level = logging.INFO
    ):
        
        self._logger = create_logger(logging.getLogger(f"{__name__}.SpeakingClubManager"), 
                                                   log_file_path, 
                                                   log_level)
        U.f_mkdir(os.path.dirname(log_file_path))
        
        
        self._chat_size = chat_size
        self._current_agent = 0
        
        self._possible_actions: dict[str, str] = possible_actions
        self._remain_actions: dict[str, str] = self._possible_actions.copy()
        
        self._agents_list: list = agents_list
        self._action_subscribers: callable = []
        self._chat_subscribers: callable = []
        
        self._max_iterations = max_iterations
        self._chat = []       
        self._actions = []       
        
        
    def _start_new_round(self):
        self._actions = [] 
        self._chat = []
        self._remain_actions = self._possible_actions.copy()
        self._current_agent = (self._current_agent + 1) % len(self._agents_list)
        
    def _update_remain_actions(self, message: str):
        if message.endswith("request_join()"):
            if self._remain_actions.get("request_change_task()"):
                self._remain_actions.pop("request_change_task()")
            self._remain_actions.pop("request_join()")
        elif message.endswith("request_change_task()"):
            self._remain_actions.pop("request_change_task()")
        elif message.endswith("request_take_instruments()"):
            self._remain_actions.pop("request_take_instruments()")
            if self._remain_actions.get("request_change_task()"):
                self._remain_actions.pop("request_change_task()")
            
    def _get_actions(self) -> str:
        actions = self._remain_actions 
        return "\n".join([f"{num + 1}) {val}" for num, val in enumerate(actions.values())])
        
    async def run_round(self) -> None:
        [agent.clear_data() for agent in self._agents_list]
        
        speaker = self._agents_list[self._current_agent]
        try:
            speaker_message, code = await speaker.select_task()
        except RuntimeError as _:
            self._logger.warning(f"Speaker {speaker.name} skipped his opportunity to select a task")
            return
        
        self._logger.debug(speaker_message)
        #Agent: take_task(x, "Name")
        task_number = int(code.split(': ')[1][10 : -1].split(', ')[0])
        
        for func in self._chat_subscribers:
            await func(speaker_message)
            
        self._update_actions_history(code)
        
        talking_agents = self._agents_list[::]
        talking_agents.pop(self._current_agent)
        
        iteration = 0
        while iteration < self._max_iterations:
            iteration += 1
            think_tasks = []
            # Аснхронные задания бе-бе-бе
            for num, agent in enumerate(talking_agents):
                think_tasks.append(asyncio.create_task(agent.think(speaker, self._form_discuss_input(agent.name, speaker.name))))
                   
            # Определяем какой агент будет говорить первым
            actor = 0
            ready_task = None
            while True:
                await asyncio.sleep(0.1)
                done, pending = await asyncio.wait(think_tasks, return_when=asyncio.FIRST_COMPLETED)
                for task in done:
                    if not task.result().endswith("skip()"):
                        ready_task = task
                        actor = think_tasks.index(task)
                        think_tasks.remove(task)
                        break
                
                if ready_task:
                    break
                elif not pending:
                    return               
                    
            # Выполнение действий говорящим агентом
            agent_message, speaker_response, speaker_code = None, None, None
            try:
                if ready_task.result().endswith("comment()"):
                    text = self._extract_text(self._agents_list[actor].name)
                    agent_message = await talking_agents[actor].create_comment(json.dumps(text), speaker_message)
                else:
                    agent_message, speaker_response, speaker_code = await talking_agents[actor].create_request(ready_task.result(), 
                                                                                                               task_number, 
                                                                                                               speaker_message, 
                                                                                                               speaker)        
                await self._update_chat(agent_message)
                self._update_actions_history(ready_task.result())
                
                if speaker_response and speaker_code:
                    await self._update_chat(speaker_response)
                    self._update_actions_history(speaker_code)
                    self._update_remain_actions(ready_task.result())
            except Exception as exc:
                err = str(exc)
                self._logger.error(err)
            
            # Остальные запоминают свои мысли
            talking_agents.pop(actor) 
            other_agents_thoughts = await asyncio.gather(*think_tasks)
            for num, thought in enumerate(other_agents_thoughts):
                if not thought.endswith("skip()"):
                   talking_agents[num].save_ideas(thought)
            
            # После ответа первого разрешаем говорить абсолютно всем агентам
            # talking_agents = self._agents_list
            talking_agents = self._agents_list[::]
            talking_agents.pop(self._current_agent)
              
    def _form_discuss_input(self, agent_name, speaker_name) -> str:
        text = "Previous actions:\n"
        text += self._extract_actions(agent_name, speaker_name)
        text += "Call one of this functions:\n" + self._get_actions()
        return text

    def _extract_text(self, agent_name) -> dict[(str, str)]:
        res = {}
        chat_copy = self._chat[::]
        agent_messages = list(filter(lambda m: m.startswith(agent_name), chat_copy))
        for message in agent_messages:
            chat_copy.remove(message)
            
        if len(agent_messages) > 0 and len(agent_messages) <= self._chat_size:
            res["Your messages"] = "\n".join([message for message in agent_messages])
        elif len(agent_messages) > self._chat_size:
            res["Your messages"] = "\n".join([message for message in agent_messages[-self._chat_size:]])
        
        if len(chat_copy) > 0 and len(chat_copy) <= self._chat_size:
            res["Previous chat messages"] = "\n".join([message for message in chat_copy])
        elif len(chat_copy) > self._chat_size:
            res["Previous chat messages"] = "\n".join([message for message in chat_copy[-self._chat_size:]])
        
        return res

    def _extract_actions(self, agent_name, speaker_name):
        actions_text = "\n".join(self._actions)
        actions_text = actions_text.replace(agent_name, agent_name + "(You)").replace(speaker_name, speaker_name + "(Speaker)")
        return actions_text
            
    def subscribe_on_chat_updating(self, func: callable):
        self._chat_subscribers.append(func)
        
    def subscribe_on_action_updating(self, func: callable):
        self._action_subscribers.append(func)
        
    def _update_actions_history(self, text):
        self._actions.append(text)
        for func in self._action_subscribers:
            func(text)
    
    async def _update_chat(self, text):
        self._logger.debug(text)
        self._chat.append(text)
        for func in self._chat_subscribers:
            await func(text)
        
    def get_allocation_result(self):
        return self._agents_list[0].task_allocation_result()   
        
    async def greeting_message(self):
        message = await self._agents_list[0].create_greeting_message()
        return message
        
    async def run_allocation(self):
        while self._agents_list[0].get_remain_task_count() > 0:
            await self.run_round()
            self._start_new_round()            
            
        self._logger.info(self._agents_list[0].task_allocation_result())
    
    
        