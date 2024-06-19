from Project.MissionDecomposition.code.openai_clients import ChatClient
from Project.MissionDecomposition.code.custom_loggers import create_logger

from Project.MultiAgentSystem.code.ColoredPrint import Colors, colored_output
from Project.MultiAgentSystem.code.Handlers.Handler import Handler

from Project.TaskAllocation.code.action_enviroment import TaskAllocationEnvironment
from Project.TaskAllocation.code.speaking_club_manager import SpeekingClubManager
from Project.TaskAllocation.code.agent import Agent

from voyager import Voyager

import logging
import asyncio
import os

class AllocationHandler(Handler):
    def __init__(self,
                 voyagers: dict[Voyager],
                 number: int, 
                 task_allocation_code_client: ChatClient.__annotations__,
                 task_allocation_chat_client: ChatClient.__annotations__,
                 base_coordinates: list[int] = None,
                 next_handler: Handler = None,
                 max_code_generation_tries: int = 2,
                 agents_on_allocation_distance: int = 4,
                 allocation_agent_log_path: str = os.path.join("logs", "agents.log"),
                 allocation_agent_log_level = logging.INFO,
                 allocation_manager_chat_size: int = 2,
                 allocation_manager_discuss_messages: int = 2,
                 allocation_manager_possible_actions: dict = {
                    "comment()" : "def comment() - Give your opinion about the speaker decision. Start conversation with this function",
                    "request_take_instruments()": "def request_take_instruments() - Help the speaker with instruments for his mission",
                    "skip()": "def skip() - Gives another collegue, who didn't take his action, opportunity to speak",
                    "request_change_task()": "def request_change_task() - If you disagree and don't support speaker decision, you can ask him to take his mission",
                 },
                 allocation_manager_log_path: str = os.path.join("logs", "chat.log"),
                 allocation_manager_log_level = logging.INFO,
                 
                 log_path: str = os.path.join("logs", "operator.log"),
                 log_level = logging.INFO):
        
        super(AllocationHandler, self).__init__(number, next_handler)
        self._logger = create_logger(logging.getLogger(f"{__name__}.AllocationHandler"), 
                                            log_path, 
                                            log_level)
        self.name = "AllocationHandler"
        self._voyagers = voyagers
        self._agents_names = voyagers.keys()
        self._base_coordinates = base_coordinates
        
        self._task_allocation_code_client = task_allocation_code_client
        self._task_allocation_chat_client = task_allocation_chat_client
        
        self._max_code_generation_count = max_code_generation_tries
        self._allocation_agent_log_path = allocation_agent_log_path
        self._allocation_agent_log_level = allocation_agent_log_level
        
        self._allocation_manager_chat_size = allocation_manager_chat_size
        self._allocation_manager_discuss_messages = allocation_manager_discuss_messages
        self._allocation_manager_possible_actions = allocation_manager_possible_actions
        self._allocation_manager_log_path = allocation_manager_log_path
        self._allocation_manager_log_level = allocation_manager_log_level
        self._allocation_distance = agents_on_allocation_distance

        
        self._moveToPosition = """async function moveToPosition(bot) {{
    point = new Vec3({coordinates});
    distance = {distance};
    bot.pathfinder.goto(new GoalNear(point.x, point.y, point.z, distance))
}}    
await moveToPosition(bot);"""
    
    def modify_request(self, request: dict) -> dict:
        try:
            task_info = request["Input"]
            speaking_club = self._setup_allocation_enviroment(task_info["Mission items"])
            asyncio.run(self._begin_allocation(speaking_club))
        except Exception as err:
            colored_output(f"Ошибка во время распределения заданий: {err}!", color=Colors.RED)
            self._logger.error(f"Ошибка при распределении заданий: {err}!")
            request["Error"] = err
            request["Success"] = False
            return request
        
        request["Input"] = speaking_club.get_allocation_result()
        request["Success"] = True
        return request
    
    async def _begin_allocation(self, allocator):
        _ = await self._agents_gathering()
        
        async def bot_write(text: str):
            name_end = text.index(": ")
            name, message = text[:name_end], text[name_end+2:]
            print(name, message)
            await self._voyagers[name].bot_write_to_chat(message)
            

        allocator.subscribe_on_chat_updating(bot_write)
        _ = await list(self._voyagers.values())[0].bot_write_to_chat(await allocator.greeting_message())
        _ = await allocator.run_allocation()
     
    async def _agents_gathering(self):
        # if len(self._agents_names) == 1:
        #     return
        # posistion = [obs[0][1]["status"]["position"] for obs in observations]
        # center = (posistion[0]["x"] + (posistion[1]["x"] - posistion[0]["x"]) / 2,
        #           posistion[0]["z"] + (posistion[1]["z"] - posistion[0]["z"]) / 2)
        # coordinates = f"{center[0]}, 70, {center[1]}"
        
        coordinates = f"{self._base_coordinates[0]}, {self._base_coordinates[1]}, {self._base_coordinates[2]}"
        code = self._moveToPosition.format(coordinates=coordinates, distance=self._allocation_distance)
        
        tasks = [asyncio.create_task(bot.create_embodiment()) for bot in self._voyagers.values()]
        _ = await asyncio.wait(tasks)
        tasks = [asyncio.create_task(bot.bot_execute_code(code)) for bot in self._voyagers.values()]
        _ = await asyncio.wait(tasks)        
        
        for v in self._voyagers.values():
            for _ in range(2):
                try:
                    await v.observe()
                    break
                except RuntimeError:
                    await v.bot_execute_code(code)
            else:
                raise RuntimeError(f"Bot {v.name} didn't reach the discussion point")
                
    def _setup_allocation_enviroment(self, tasks: list[dict]) -> SpeekingClubManager:
        environment = TaskAllocationEnvironment(tasks, self._agents_names)
        agents = []
        for name in self._agents_names:
            agents.append(Agent(name, 
                                self._task_allocation_chat_client, 
                                self._task_allocation_code_client, 
                                environment, 
                                self._role_reciever(name + ".txt"),
                                max_interpretate_retries=self._max_code_generation_count,
                                log_file_path=self._allocation_agent_log_path,
                                log_level=self._allocation_agent_log_level))
            
        speeking_club_manager = SpeekingClubManager(agents, 
                                                    self._task_allocation_chat_client, 
                                                    log_file_path=self._allocation_manager_log_path, 
                                                    log_level=self._allocation_manager_log_level,
                                                    possible_actions=self._allocation_manager_possible_actions)
        return speeking_club_manager
    
    def _role_reciever(self, role_name: str) -> str:
        file_dir = os.path.dirname(__file__)
        previous_dir = os.path.join(file_dir, os.pardir)
        roles_dir = os.path.join(previous_dir, previous_dir, "roles")
        text=""
        
        if os.path.exists(os.path.join(roles_dir, role_name)):
            with open(os.path.join(roles_dir, role_name), 'r') as f:
                text = f.read()
        self._logger.info(text)
        
        return text      