from Project.MultiAgentSystem.code.Handlers.Handler import Handler
from Project.MultiAgentSystem.code.WorkingAgent import WorkingAgent
from Project.MultiAgentSystem.code.ColoredPrint import Colors, colored_output
from Project.MissionDecomposition.code.mission_analyzer import MissionAnalyzer
from Project.MissionDecomposition.code.custom_loggers import create_logger
from voyager import Voyager
import numpy as np
import asyncio
import json
import logging
import os

class ExecutionHandler(Handler):
    def __init__(self, 
                 mission_analyzer: MissionAnalyzer,
                 voyagers: dict[Voyager], 
                 number: int, 
                 agents_names: list[str] = ["Anna", "Steve", "Nikita"],
                 next_handler: Handler = None,
                 log_path: str = os.path.join("logs", "operator.log"),
                 log_level = logging.INFO):

        super(ExecutionHandler, self).__init__(number, next_handler)
        self._logger = create_logger(logging.getLogger(f"{__name__}.ExecutionHandler"), 
                                        log_path, 
                                        log_level)
        self.name = "ExecutionHandler"
        self._mission_analyzer = mission_analyzer
        self._agents_names = agents_names
        self._voyagers = voyagers
        
        self._moveToPosition = """async function moveToPosition(bot) {{
    point = new Vec3({coordinates});
    distance = {distance};
    bot.pathfinder.goto(new GoalNear(point.x, point.y, point.z, distance))
}}    
await moveToPosition(bot);"""
        
    def modify_request(self, request) -> dict:
        tasks = request["Input"]
        working_agents = [WorkingAgent(name, tasks[name], v, self._mission_analyzer) 
                            for v, name in zip(self._voyagers.values(), self._agents_names)]
        
        try:
            request["Failed"] = asyncio.run(self.complete_tasks(working_agents))
            request["Input"] = asyncio.run(self._get_agents_resources())
        except RuntimeError as err: 
            colored_output(f"Ошибка во время распределения заданий: {err}!", color=Colors.RED)
            self._logger.error(f"Агенты не смогли справиться с потсавленными заданиями - {tasks} Ошибка:\n{err}")
            request["Error"] = err
            request["Success"] = False
            return request
        
        request["Success"] = True
        return request
        
    async def complete_tasks(self, working_agents: list[WorkingAgent]):
        tasks = []
        for w in working_agents:
            tasks.append(asyncio.create_task(w.voyager.bot_write_to_chat(f"/give {w.name} minecraft:crafting_table")))
        await asyncio.wait(tasks)   
            
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
        master_data = await master.voyager.observe()
        master_pos = master_data[0][1]["status"]["position"]  
        
        res = await slave.voyager.bot_execute_code(self._moveToPosition.format(coordinates=f"{master_pos['x']}, {master_pos['y']}, {master_pos['z']}", distance=3))
        while (await self._distance_between_agents(slave, master) > 4):
            await asyncio.sleep(4)
        res = await slave.execute_task(f"Drop your {item} to a player named {master.name}, which is located close to you.")
        res["Agents"].append(master)
        _ = await master.voyager.bot_write_to_chat(f"Thank you for giving {item}!")
        master.get_instrument(item)
        return res
    
    async def _get_agents_resources(self) -> str:
        tasks = [asyncio.create_task(agent.observe()) for agent in self._voyagers.values()]
        res = await asyncio.wait(tasks)
        return res
    
    
    async def _distance_between_agents(self, agent1, agent2) -> float:
        agent1_data = await agent1.voyager.observe()
        agent1_position = agent1_data[0][1]["status"]["position"]        
        agent2_data = await agent2.voyager.observe()
        agent2_position = agent2_data[0][1]["status"]["position"]
        agent1_position, agent2_position = ([agent1_position["x"], agent1_position["y"], agent1_position["z"]],
                                        [agent2_position["x"], agent2_position["y"], agent2_position["z"]])
        
        return np.linalg.norm(np.array(agent2_position) - np.array(agent1_position))       
                
    