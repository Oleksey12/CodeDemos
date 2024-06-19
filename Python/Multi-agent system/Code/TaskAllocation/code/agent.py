from Project.TaskAllocation.code.action_enviroment import TaskAllocationEnvironment
from Project.MissionDecomposition.code.custom_loggers import create_logger
from voyager.prompts import load_prompt
from dataclasses import dataclass
import voyager.utils as U

import logging
import os

@dataclass
class TaskRequest():
    addresser: str
    sender: str
    task_name: str
    request_type: str
    callback_func: callable


class Agent():
    def __init__(self,
                name,
                chat_client,
                code_client,
                enviroment: TaskAllocationEnvironment.__annotations__,
                role: str = "",
                max_interpretate_retries = 3,
                log_file_path = os.path.join("logs", "main.log"),
                log_level = logging.INFO):
        
        self._logger = create_logger(logging.getLogger(f"{__name__}.Agent"), 
                                                   log_file_path, 
                                                   log_level)
        U.f_mkdir(os.path.dirname(log_file_path))
      
        self.name = name
        
        self._chat_client = chat_client
        self._code_client = code_client
        
        self._enviroment = enviroment
        self._previous_thoughts = ""
        
        self._role = role
        
        self._max_interpretate_retries = max_interpretate_retries
        
        self._greetings_prompt = load_prompt("greetings")
        
        self._select_prompt = load_prompt("select_task")
        self._select_prompt = load_prompt("select_task")
        self._discuss_prompt = load_prompt("discuss_task")        
        
        self._comment_prompt = load_prompt("discuss_comment")
        self._join_prompt = load_prompt("discuss_join")
        self._take_instruments_prompt = load_prompt("discuss_take_instruments")
        self._take_task_prompt = load_prompt("discuss_take_task")
        
        self._respond_prompt = load_prompt("respond") 
        
        self._interpretate_selection_prompt = load_prompt("interpretate_selection")
        self._interpretate_respond_prompt = load_prompt("interpretate_respond")
        

    def clear_data(self):
        self._previous_thoughts = ""
    
    async def select_task(self) -> tuple:
        system_message = self._select_prompt.format(role=self._role, tasks=self._enviroment.generate_task_allocation_for_agent(self.name))
        human_message = self._enviroment.generate_task_list_for_agent()
        
        # self._logger.debug(system_message + "\n" + human_message)
        
        self._logger.debug(f"Agents missions:\n{self._enviroment.generate_task_allocation_for_agent(self.name)}")
        text = await self._chat_client.chat_llm(system_message, human_message)
        
        if text == "Unknown":
            self._logger.error(f"Failed to generate task selection message for input: {human_message}")
            raise RuntimeError(f"Failed to generate task selection message for input: {human_message}")
        
        self._logger.debug(f"Speaker {self.name} said: {text}")
        code = await self._apply_task_selection(text)        
        
        return f"{self.name}: {text}", f"{self.name}: {code}"
        
    async def _apply_task_selection(self, text: str) -> int:
        system_message = self._interpretate_selection_prompt
        human_message = text 
        
        response = ""
        err = ""
        for _ in range(self._max_interpretate_retries):
            try:
                response: str = await self._code_client.chat_llm(system_message.format(error=err), human_message)
                if not response.startswith("take_task("):
                    break
                self._logger.debug(f"Generated code for speaker text: {response}")
                
                # Парсим функцию
                params = response[10 : -1].split(', ')
                task_number = int(params[0])
                # task_name = params[1].strip().replace("\"","").capitalize()
                
                self._enviroment.take_task(task_number, self.name)
                self._logger.debug(f"Agents missions:\n{self._enviroment.generate_task_allocation_for_agent(self.name)}")
                return response
            except RuntimeError as exc:
                self._logger.warning(f"Can't parse generated code: {str(exc)}")
                err = f"From last coding round an error occured! Fix this - {str(exc)}"
            
        raise RuntimeError("Invalid selection text, try to create a new one!")
             
    async def think(self, speaker_name, input_text) -> str:
        thoughts = "None"
        if self._previous_thoughts:
            thoughts = self._previous_thoughts
            thoughts = None
        
        system_message = self._discuss_prompt.format(name=self.name,  
                                                     tasks=self._enviroment.generate_task_allocation_for_agent(self.name, speaker_name),
                                                     thoughts=thoughts)
        human_message = input_text
        
        # self._logger.debug(system_message + "\n" + human_message)
        text = await self._code_client.chat_llm(system_message, human_message)
        self._logger.debug(f"Player {self.name} took action: {text}")
        return f"{self.name}: {text}"
    
    async def create_comment(self, chat_messages, speaker_message) -> str:
        system_message = self._comment_prompt
        human_message = chat_messages
        system_message = system_message.format(name=self.name, role=self._role, speaker=speaker_message)
        message = await self._chat_client.chat_llm(system_message, human_message)
        self._logger.debug(f"Player {self.name} discussed: {message}")
        return f"{self.name}: {message}"
         
    async def create_request(self, 
                             agent_action, 
                             task_number,
                             speaker_message, 
                             speaking_agent) -> tuple:
        
        system_message, human_message = self._join_prompt, self._enviroment.generate_task_allocation_for_agent(self.name, speaking_agent.name) 
        
        if agent_action.endswith("request_join()"):
            self._enviroment.request_join(task_number, self.name)
            text_summary = f"{self.name} wants to join you in making your task"
        elif agent_action.endswith("request_change_task()"):
            system_message = self._take_task_prompt
            self._enviroment.request_change_task(task_number, self.name)
            text_summary = f"{self.name} wants to take your task, and do it by himself"
        elif agent_action.endswith("request_take_instruments()"):
            system_message = self._take_instruments_prompt
            self._enviroment.request_take_instruments(task_number, self.name)
            text_summary = f"{self.name} wants to help you with instruments for your task"
        
        system_message = system_message.format(name=self.name, role=self._role, speaker=speaker_message)
        message: str = await self._chat_client.chat_llm(system_message, human_message)
        self._logger.debug(f"Player {self.name} discussed: {message}")
        
        return f"{self.name}: {message}", *await speaking_agent.response(f"{self.name}: {message}", text_summary, speaker_message)
        
    async def response(self, text, text_summary, speaker_message) -> str:
        system_message = self._respond_prompt.format(role=self._role,
                                                     message=speaker_message,
                                                     tasks=self._enviroment.generate_task_allocation_for_agent(self.name, self.name))
        
        human_message = text + "\nSummary:\n" + text_summary
        
        text = await self._chat_client.chat_llm(system_message, human_message)
        self._logger.debug(f"Speaker responded: {text}")
        action = await self._apply_response_action(text)
        self._logger.debug(f"New agents missions:\n{self._enviroment.generate_task_allocation_for_agent(self.name)}")
        
        return f"{self.name}: {text}", f"{self.name}: {action}"
    
    async def _apply_response_action(self, text) -> None:
        system_message = self._interpretate_respond_prompt
        human_message = text 
        
        response = ""
        err = ""
        for _ in range(self._max_interpretate_retries):
            try:
                response: str = await self._code_client.chat_llm(system_message.format(error=err), human_message)
                self._logger.debug(f"Generated code for speaker response: {response}")
                
                if response.startswith("accept"):
                    self._enviroment.accept_request(self.name)
                    return response
                if response.startswith("decline"):
                    self._enviroment.decline_request(self.name)
                    return response
                
            except RuntimeError as exc:
                self._logger.error(f"Can't parse generated code: {str(exc)}")
                err = f"From last coding round an error occured! Fix this - {str(exc)}"
            
        raise RuntimeError("Invalid response text, try to create a new one!")
        
    async def create_greeting_message(self) -> str:
        human_message = "Write a small greeting message to start the discussion about task allocation process"
        system_message = self._greetings_prompt.format(role=self._role)
        return await self._chat_client.chat_llm(system_message, human_message)
        
    # Возможность сохранять создаёт большие проблемы
    def save_ideas(self, text):
        self._previous_thoughts = text
    
    def get_remain_task_count(self) -> int:
        return self._enviroment.task_count()
    
    def task_allocation_result(self) -> dict:
        return self._enviroment.get_task_allocation_result()

    