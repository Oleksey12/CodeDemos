from Project.MissionInterpretation.code.Interpretation import InterpretationAgent
from Project.MissionDecomposition.code.mission_analyzer import MissionAnalyzer
from Project.MissionDecomposition.code.main import TaskDekompositionAgent
from Project.MissionDecomposition.code.openai_clients import ChatClient
from Project.MissionDecomposition.code.custom_loggers import create_logger
from Project.TaskAllocation.code.action_enviroment import TaskAllocationEnvironment
from Project.TaskAllocation.code.speaking_club_manager import SpeekingClubManager
from Project.TaskAllocation.code.agent import Agent
from Project.MultiAgentSystem.code.WorkingAgent import WorkingAgent

from voyager import Voyager
import faker.generator

import numpy as np
import asyncio
import json
import logging
import faker
import os

class Colors:
    """ ANSI color codes """
    BLACK = "\033[0;30m"
    RED = "\033[0;31m"
    GREEN = "\033[0;32m"
    BROWN = "\033[0;33m"
    BLUE = "\033[0;34m"
    PURPLE = "\033[0;35m"
    CYAN = "\033[0;36m"
    LIGHT_GRAY = "\033[0;37m"
    DARK_GRAY = "\033[1;30m"
    LIGHT_RED = "\033[1;31m"
    LIGHT_GREEN = "\033[1;32m"
    YELLOW = "\033[1;33m"
    LIGHT_BLUE = "\033[1;34m"
    LIGHT_PURPLE = "\033[1;35m"
    LIGHT_CYAN = "\033[1;36m"
    LIGHT_WHITE = "\033[1;37m"
    BOLD = "\033[1m"
    FAINT = "\033[2m"
    ITALIC = "\033[3m"
    UNDERLINE = "\033[4m"
    BLINK = "\033[5m"
    NEGATIVE = "\033[7m"
    CROSSED = "\033[9m"
    END = "\033[0m"

class AgentGroupOperator ():
    def __init__(self,
                 user_agent: InterpretationAgent.__annotations__,
                 decomposition_agent: TaskDekompositionAgent.__annotations__,
                 mission_analyzer: MissionAnalyzer.__annotations__,
                 task_allocation_code_client: ChatClient.__annotations__,
                 task_allocation_chat_client: ChatClient.__annotations__,
                 openai_api_key: str,
                 mc_port: int,
                 base_coordinates: list[int],
                 voyagers = None,
                 agents_on_allocation_distance: int = 6,
                 agents_on_cooperation_distance: int = 8,
                 operator_log_path: str = os.path.join("logs", "operator.log"),
                 operator_log_level = logging.INFO,
                 max_code_generation_tries: int = 2,
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
                 agents_count: int = 3,
                 agents_names: list = ["Anna", "Steve", "Nikita"]
                 ) -> None:
        
        self._logger = create_logger(logging.getLogger(f"{__name__}.AgentGroupOperator"), 
                                                   operator_log_path, 
                                                   operator_log_level)
        
        self._user_agent = user_agent
        self._decomposition_agent = decomposition_agent
        self._mission_analyzer = mission_analyzer
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
        
        self._base_coordinates = base_coordinates
        
        self._allocation_distance = agents_on_allocation_distance
        self._cooperation_distance = agents_on_cooperation_distance
        
        self._agents_names = agents_names
        for _ in range(agents_count - len(agents_names)):
            self._agents_names.append(faker.Faker.name)
        self._agents_names = self._agents_names[:agents_count]
        
        if voyagers:
            self._voyagers = voyagers
        else:
            self._voyagers = {name : Voyager(bot_name=name,
                                    enable_pauses=False,
                                    server_port=3000 + num,
                                    mc_port=mc_port,
                                    openai_api_key=openai_api_key,
                                    ckpt_dir="test1",
                                    resume=True) for num, name in enumerate(self._agents_names)}
        
        self._requests = []
        
        # Результат декомпозиции заданий
        self._items = []
        self._items_tasks = []
        self._finishing_sequence = []
        
        # Результат распределения задач
        self._tasks = []
        
        self._moveToPosition = """async function moveToPosition(bot) {{
    point = new Vec3({coordinates});
    distance = {distance};
    bot.pathfinder.goto(new GoalNear(point.x, point.y, point.z, distance))
}}    
await moveToPosition(bot);"""

        self._move_to_entity = """async function moveToEntity(bot) {{
    point = bot.players["{name}"].entity.position;
    distance = {distance};
    bot.pathfinder.goto(new GoalNear(point.x, point.y, point.z, distance))
}}    
await moveToEntity(bot);"""

        self._storeItems = """async function storeInventory(bot) {{
    await bot.pathfinder.goto(new GoalNear({base}, 5));
    
    const chestBlock = bot.findBlock({{
        point: bot.entity.position,
        matching: block => block.name === 'chest',
        maxDistance: 64,
        minCount: 1,
    }});
    
    await bot.pathfinder.goto(new GoalNear(chestBlock.position.x, chestBlock.position.y, chestBlock.position.z, 2));
    
    const chest = await bot.openChest(chestBlock);
    for (const item of bot.inventory.items()) {{
        await chest.deposit(item.type, null, item.count);
    }}
    await chest.close();
}}
await storeInventory(bot);"""


    def _role_reciever(self, role_name: str) -> str:
        file_dir = os.path.dirname(__file__)
        previous_dir = os.path.join(file_dir, os.pardir)
        roles_dir = os.path.join(previous_dir, "roles")
        text=""
        
        if os.path.exists(os.path.join(roles_dir, role_name)):
            with open(os.path.join(roles_dir, role_name), 'r') as f:
                text = f.read()
        self._logger.info(text)
        
        return text

    def _colored_output(self, text, color, end="\n") -> None:
        print(color + text + Colors.END, end=end)
    
    def start(self):
        os.system("clear")
        self._colored_output("Многоагентная система Woygaer v0.1\n", Colors.BLUE)
        
        while True:
            self._colored_output('Введите задание на английском языке, или команду "/stop"', Colors.BLINK, end=" ")
            user_task = input().strip()
            if user_task == "/stop":
                for v in self._voyagers.values():
                    v.env.close()
                break
            try:
                result = self._user_agent.format_task(user_task)
            except RuntimeError as err:
                self._colored_output('Некорректное задание', Colors.BLINK)
                self._logger.warning(f"Неверные пользовательские данные - {user_task}, ошибка - {err}")
                continue
            self._colored_output(f'Ваша задание интерпретировано агентом, как {result}, продолжаем [y/n]?', Colors.BLINK, end=" ")
            user_task = input().strip()
            if user_task != "y":
                continue

            try:
                task_info: dict = self._decomposition_agent.run(result)
            except RuntimeError as err:
                self._colored_output('Ошибка при декомпозиции задания!', Colors.BLINK)
                self._logger.error(f"Ошибка при декомпозиции задания - {result}, ошибка - {err}")
                continue
            
            mission_items = "\n".join([f"{num+1}) {item}" for num, item in enumerate(task_info["Beutified mission items"])])
            action_sequence = "\n".join(f"{num+1}) {json.dumps(action)}" for num, action in enumerate(task_info["Action sequence"]))
            
            self._logger.info(f"Выполнения задания {json.dumps(result)}:\n{mission_items}\nДействия:\n{action_sequence}\n")
            self._colored_output(f'Требуемые ресурсы и последовательность действия для выполнения заданий:\n{mission_items}\nДействия:\n{action_sequence}\nпродолжаем [y/n]?', Colors.BLINK, end=" ")
            user_task = input().strip()
            if user_task != "y":
                continue
            
            speaking_club = self._setup_allocation_enviroment(task_info["Mission items"])
            try:
                asyncio.run(self._begin_allocation(speaking_club))
            except Exception as exc:
                self._colored_output(f"Something went wrong: {exc}!", color=Colors.RED)
                for v in self._voyagers.values():
                    v.env.close()
                
            tasks = speaking_club.get_allocation_result()
            self._colored_output(json.dumps(tasks), Colors.BLINK)
            
            
            working_agents = [WorkingAgent(name, tasks[name], v, self._mission_analyzer) 
                              for v, name in zip(self._voyagers.values(), self._agents_names)]
            for w in working_agents:
                w.voyager.bot_write_to_chat(f"/give {w.name} minecraft:crafting_table")
            res = asyncio.run(self.complete_tasks(working_agents))
            print(res)   
            print(json.dumps([agent.observe() for agent in self._voyagers.values()]))
            
            coordinates = f"{self._base_coordinates[0]}, {self._base_coordinates[1]}, {self._base_coordinates[2]}"
            for v in self._voyagers.values():
                res = v.bot_execute_code(self._storeItems.format(base=coordinates))
                print(res)
            _ = input()    
            
    async def complete_tasks(self, working_agents: list[WorkingAgent]):
        dependencies = []
        failed_tasks = []
        courutines = []
        free_agents = working_agents[:]

        while True:
            for agent in free_agents[:]:
                task = agent.manage_tasks()
                if not task or self._check_dependencies(agent.name, dependencies):
                    continue
                
                if task["Name"].startswith("Give"):
                    master_name = task["Name"].split('to ')[1]
                    dependencies.append((agent.name, master_name, task["Action"]))
                    
                free_agents.remove(agent)
                courutines.append(asyncio.create_task(agent.execute_task(task["Action"])))
            
            if len(free_agents) == len(working_agents) and len(courutines) == 0 and len(dependencies) == 0:
                break
            if (courutines):
                done, pending = await asyncio.wait(courutines, return_when=asyncio.FIRST_COMPLETED)
                for ready_task in done:
                    if not ready_task.result()["Done"]:
                        failed_tasks.append(ready_task.result()["Task"])
                        if len(list(filter(lambda x: x[2] == ready_task.result()["Task"], dependencies))) > 0:
                            raise RuntimeError("Key mission failed!")
                    agents = ready_task.result()["Agents"]
                    free_agents.extend(agents)                
                
            # resolve dependencies
            agent_names = [a.name for a in free_agents]
            for dependency in dependencies:
                if dependency[0] in agent_names and dependency[1] in agent_names:
                    index1, index2 = agent_names.index(dependency[0]), agent_names.index(dependency[1])
                    item = dependency[2][7:].strip()
                    # Working only for one item
                    pending = list(pending)
                    pending.append(asyncio.create_task(self._agents_cooperation(free_agents[index1], free_agents[index2], item)))
                    free_agents = list(np.delete(free_agents, (index1, index2)))
                    dependencies.remove(dependency)
                    break
            
            courutines = [] if not pending else list(pending)
        return failed_tasks
    
    def _check_dependencies(self, name, dependencies):
        return any([dep[0] == name or dep[1] == name for dep in dependencies])
            
    async def _agents_cooperation(self, slave, master, item):
        master_data = master.voyager.observe()
        master_pos = master_data[0][1]["status"]["position"]  
        
        res = slave.voyager.bot_execute_code(self._moveToPosition.format(coordinates=f"{master_pos['x']}, {master_pos['y']}, {master_pos['z']}", distance=3))
        while (self._distance_between_agents(slave, master) > 4):
            await asyncio.sleep(4)
        res = await slave.execute_task(f"Drop your {item} to a player named {master.name}, which is located close to you.")
        res["Agents"].append(master)
        master.voyager.bot_write_to_chat(f"Thank you for giving {item}!")
        master.get_instrument(item)
        return res
        
    def _distance_between_agents(self, agent1, agent2) -> float:
        agent1_data = agent1.voyager.observe()
        agent1_position = agent1_data[0][1]["status"]["position"]        
        agent2_data = agent2.voyager.observe()
        agent2_position = agent2_data[0][1]["status"]["position"]
        agent1_position, agent2_position = ([agent1_position["x"], agent1_position["y"], agent1_position["z"]],
                                        [agent2_position["x"], agent2_position["y"], agent2_position["z"]])
        
        return np.linalg.norm(np.array(agent2_position) - np.array(agent1_position))       
        
                
    async def _begin_allocation(self, allocator):
        _ = await self._agents_gathering()
        
        def bot_write(text: str):
            name_end = text.index(": ")
            name, message = text[:name_end], text[name_end+2:]
            print(name, message)
            self._voyagers[name].bot_write_to_chat(message)
        allocator.subscribe_on_chat_updating(bot_write)
        list(self._voyagers.values())[0].bot_write_to_chat(await allocator.greeting_message())
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
        _ = [(bot.create_embodiment(), bot.bot_execute_code(code)) for bot in self._voyagers.values()]
        for v in self._voyagers.values():
            for _ in range(2):
                try:
                    v.observe()
                    break
                except RuntimeError:
                    v.bot_execute_code(code)
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
             
    
def fermat_point(A, B, C):
    a = np.linalg.norm(np.array(C) - np.array(B))
    b = np.linalg.norm(np.array(C) - np.array(A))
    c = np.linalg.norm(np.array(B) - np.array(A))
    cos_A = (b**2 + c**2 - a**2) / (2 * b * c)
    cos_B = (a**2 + c**2 - b**2) / (2 * a * c)
    cos_C = (a**2 + b**2 - c**2) / (2 * a * b)

    if cos_A < -0.5:
        return A
    if cos_B < -0.5:
        return B
    if cos_C < -0.5:
        return C

    cot_A = cos_A / np.sqrt(1 - cos_A**2)
    cot_B = cos_B / np.sqrt(1 - cos_B**2)
    cot_C = cos_C / np.sqrt(1 - cos_C**2)

    x = (A[0] * cot_A + B[0] * cot_B + C[0] * cot_C) / (cot_A + cot_B + cot_C)
    y = (A[1] * cot_A + B[1] * cot_B + C[1] * cot_C) / (cot_A + cot_B + cot_C)

    return (x, y)




# import time
# import aioconsole    

# async def async_user_input() -> str:
#     result = await aioconsole.ainput("Give me some numbers")
#     return result

# async def async_user_output() -> str:
#     await asyncio.sleep(4)
#     print("Here my numbers: 1223")


# async def test_ainput():
#     tasks = [asyncio.create_task(async_user_input()), asyncio.create_task(async_user_output())]
#     res = await asyncio.wait(tasks, return_when=asyncio.FIRST_COMPLETED)
    
# asyncio.run(test_ainput())        

