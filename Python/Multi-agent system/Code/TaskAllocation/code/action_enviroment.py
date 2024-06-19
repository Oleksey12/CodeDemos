from dataclasses import dataclass
import voyager.utils as U
import json
import os
import re

# Выбирающий
# Говорящий
# Отвечающий

    
# request_type = "join" | "take" | "create"    
    
@dataclass
class InformativeTaskInfo():
    """Позволяет отслеживать какие задания кому принадлежат"""
    task_number: int
    task_name: str
    task_data: tuple[str, int]
    task_action: str
    owners: list[str] | None
    instruments: list[str] | None
    instruments_maker: str | None
    
    def set_owner(self, new_owner: str):
        self.owners = [new_owner]
        
    def set_maker(self, new_maker: str):
        self.instruments_maker = new_maker
    
    def add_owner(self, new_owner: str):
        self.owners.append(new_owner)
    
    def set_instrument_maker(self, new_maker: str):
        self.instruments_maker = [new_maker]

@dataclass
class TaskRequest():
    addresser: str
    sender: str
    task: InformativeTaskInfo.__annotations__
    request_type: str
    callback_func: callable

class TaskAllocationEnvironment():
    def __init__(self,
        task_list: list[dict],
        agents: list[str],
    ):
        self._requests = []
        self._round_number = 0
        
        self._agents = agents
        
        self._task_allocation: dict[str, list] = {agent: [] for agent in agents}
        self._task_storage: list = self._init_task_storage(task_list)
        self._task_list = [self.generate_readable_task_info(task) for task in self._task_storage]
    
    
    def task_count(self) -> int:
        return len(self._task_list)
    
    def _init_task_storage(self, task_list: list[dict]) -> list[InformativeTaskInfo.__annotations__]:
        task_storage = []
        cases = [r"^any$", r"hand", r"any tool"]
        for num, task in enumerate(task_list):
            tools = []
            for tool in task["Instruments"]:
                if not any([re.search(case, tool, re.I) for case in cases]):
                    tools.append(tool)
            
            if len(tools) == 0:
                tools = None
            
            # Создаём название задания из сокращения действия
            additional_info_index = task["Action"].find("with")
            if additional_info_index == -1:
                additional_info_index = task["Action"].find("to get")
            task_short_name = task["Action"][:additional_info_index - 1].capitalize()
            
            task_storage.append(InformativeTaskInfo(
                num + 1,
                task_short_name,
                tuple(task["Item_data"]),
                task["Action"],
                None,
                tools,
                None
            ))
        return task_storage
    
    def generate_readable_task_info(self, task_info: InformativeTaskInfo.__annotations__) -> dict:
        return {"Number": task_info.task_number, "Name": task_info.task_name, "Instruments": task_info.instruments}
    
    def get_task_allocation_result(self) -> dict:
        for tasks in self._task_allocation.values():
            for task in tasks:
                similar_task = list(filter(lambda t_info: t_info.task_name == task["Name"], self._task_storage))
                if similar_task and len(similar_task) > 0:
                    task["Action"] = similar_task[0].task_action
                else:
                    end = task["Name"].index("to")
                    task["Action"] = task["Name"][:end].replace("Give", "Make")
        
        return(self._task_allocation)
                    
    def generate_task_list_for_agent(self) -> str:
        return json.dumps(self._task_list)
    
    def generate_task_allocation_for_agent(self, agent_name, speaker_name = "") -> str:
        res = ""
        for name, tasks in self._task_allocation.items():
            formatted_name = f"{name}: "
            if name == speaker_name:
                formatted_name = f"{name}(Speaker): "
            if name == agent_name:
                formatted_name = f"{name}(You): "
                
            res += formatted_name + json.dumps(tasks) + "\n"
        res = res.strip()
        return res
    
    def update_task_info(self, task_number: int, agent_name: str) -> None:
        index = [task["Number"] for task in self._task_list].index(task_number)
        selected_task = self._task_list.pop(index)
        selected_task.pop("Number")
        self._task_allocation[agent_name].append(selected_task)
    
    # Выбирающий 1
    def take_task(self, task_number: int, agent_name: str) -> None:
        if agent_name not in self._agents:
            raise IndexError(f"There is no {agent_name} in agent list! Correct yourself!")
        
        if task_number > len(self._task_storage):
            raise IndexError(f"Task with number of {task_number} doesn't exist! Correct yourself!")
        
        task = self._task_storage[task_number - 1]
        # if task.task_name != task_name:
        #     raise RuntimeError(f"Task №{task_number} name is not {task_name}, but {task.task_name}! Correct yourself!")
        
        if task.owners:
            raise RuntimeError(f"Task №{task_number} {task.task_name} is already occupied! Correct yourself!")
        
        task.owners = [agent_name]
        self.update_task_info(task_number, agent_name)

    # Говорящий 2
    def request_take_instruments(self, task_number: int, agent_name: str) -> None:
        if agent_name not in self._agents:
            raise IndexError(f"There is no {agent_name} in agent list! Correct yourself!")
        
        if task_number > len(self._task_storage):
            raise IndexError(f"Task with number of {task_number} doesn't exist! Correct yourself!")
        
        task = self._task_storage[task_number - 1]
        if not task.instruments:
            raise RuntimeError(f"Task №{task_number} {task.task_name} doesn't have instruments! Correct yourself!")

        if agent_name in task.owners:
            raise RuntimeError("Your can't make instruemnts for your own task! Correct yourself!")
        
        if task.instruments_maker:
            raise RuntimeError(f"Making instruments for task №{task_number} {task.task_name} is already occupied! Correct yourself!")
        
        task_join_request = TaskRequest(task.owners[0], 
                                        agent_name, 
                                        task, 
                                        "make_instruments", 
                                        lambda: task.set_maker(agent_name))
        
        self._requests.append(task_join_request)
        
    def request_change_task(self, task_number: int, agent_name: str) -> None:
        if agent_name not in self._agents:
            raise IndexError(f"There is no {agent_name} in agent list! Correct yourself!")
        
        if task_number > len(self._task_storage):
            raise IndexError(f"Task with number of {task_number} doesn't exist! Correct yourself!")
        
        task = self._task_storage[task_number - 1]
        
        if not task.owners:
            raise RuntimeError(f"Task №{task_number} {task.task_name} is not occupied! Correct yourself!")
        
        if agent_name in task.owners:
            raise RuntimeError("Your can't take your own task! Correct yourself!")
        
        task_take_request = TaskRequest(task.owners[0], 
                                        agent_name, 
                                        task, 
                                        "take", 
                                        lambda: task.set_owner(agent_name))
        
        self._requests.append(task_take_request)
        
    def request_join(self, task_number: int, agent_name: str) -> None:
        if agent_name not in self._agents:
            raise IndexError(f"There is no {agent_name} in agent list! Correct yourself!")
        
        if task_number > len(self._task_storage):
            raise IndexError(f"Task with number of {task_number} doesn't exist! Correct yourself!")
        
        task = self._task_storage[task_number - 1]
        if not task.owners:
            raise RuntimeError(f"Task №{task_number} {task.task_name} is not occupied! Correct yourself!")
        
        if agent_name in task.owners:
            raise RuntimeError("Your are already member of this task! Correct yourself!")
        
        task_take_request = TaskRequest(task.owners[0], 
                                        agent_name, 
                                        task, 
                                        "join", 
                                        lambda: task.add_owner(agent_name))
        
        self._requests.append(task_take_request)
    
    # Отвечающий 3
    def accept_request(self, addresser_name: str) -> None:
        if len(self._requests) == 0:
            raise IndexError("There are no pending requests! Correct yourself!")
        
        request: TaskRequest = self._requests.pop()
        if request.addresser != addresser_name:
            self._requests.append(request)
            raise RuntimeError("There are no input requests for you! Correct yourself!")
        
        if request.request_type == "make_instruments":
            if (not request.task.instruments):
                raise RuntimeError("There are no instruments!")
            
            task = list(filter(lambda x: x["Name"] == request.task.task_name, self._task_allocation[request.addresser]))[0]
            task["Type"] = task.get("Type",[])
            task["Type"].append(f"Cooperative instruments made by {request.sender}")
            self._task_allocation[request.sender].append({"Name": f"Give 1 {', '.join(request.task.instruments)} to {request.addresser}"})
            # self._task_allocation[request.addresser].append({"Name": f"Get 1 {','.join(request.task.instruments)} from {request.sender}"})
            
        elif request.request_type == "join":
            task = list(filter(lambda x: x["Name"] == request.task.task_name, self._task_allocation[request.addresser]))[0]
            task["Type"] = task.get("Type", [])
            task["Type"].append(f"Cooperative {request.sender} + {request.addresser}")
            self._task_allocation[request.sender].append(task)
        elif request.request_type == "take":
            task = list(filter(lambda x: x["Name"] == request.task.task_name, self._task_allocation[request.addresser]))[0]
            self._task_allocation[request.addresser].remove(task)
            self._task_allocation[request.sender].append(task)
        
        request.callback_func()
        
    def decline_request(self, addresser_name: str) -> None:
        if len(self._requests) == 0:
            raise IndexError("There are no pending requests! Correct yourself!")
        
        request = self._requests.pop()
        if request.addresser != addresser_name:
            self._requests.append(request)
            raise RuntimeError("There are no input requests for you! Correct yourself!")
        
    
    