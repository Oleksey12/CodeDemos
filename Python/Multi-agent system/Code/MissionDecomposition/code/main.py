from Project.MissionDecomposition.code.mission_analyzer import MissionAnalyzer
from Project.MissionDecomposition.code.custom_loggers import create_logger
import voyager.utils as U
from copy import deepcopy
import asyncio
import logging
import math
import os
import re


class TaskDekompositionAgent():
    def __init__(self,
      mission_analyzer: MissionAnalyzer.__annotations__,
      max_iterations: int = 9,
      instrumnets_as_items: bool = False,
      log_file_path = os.path.join("logs", "main.log"),
      log_level = logging.INFO):
      
      self._logger = create_logger(logging.getLogger(f"{__name__}.TaskDekompositionAgent"), 
                                                   log_file_path, 
                                                   log_level)
      U.f_mkdir(os.path.dirname(log_file_path))
      self._instruments_as_items = instrumnets_as_items
      self._mission_analyzer = mission_analyzer
      self._lock = asyncio.Lock()
      self._max_iterations = max_iterations
      self._initialize_storages()
      
    def _initialize_storages(self):
      self._items_dict: dict = {}
      self._actions_storage: list[dict] = []
      self._final_items: list[dict] = []
      
      self._smelt_items_count: int = 0
      self._tools: list = []
    
    def run(self, missions: list[str]) -> dict:
      self._initialize_storages()
      _ = asyncio.run(self.decompose_missions(missions))
      _ = asyncio.run(self._recursive_item_decomposition(self._items_dict, max_iterations = self._max_iterations))
      
      res = {}
      res["Mission items"] = self._final_items
      beutified_mission_items = [item["Item_data"] for item in self._final_items]
            
      res["Beutified mission items"] = beutified_mission_items
      res["Action sequence"] = self._actions_storage
      if self._smelt_items_count != 0:
        coal_count = math.ceil(self._smelt_items_count / 9)
        self._tools.append(("Coal", coal_count))
      res["Required tools"] = self._tools
      
      return res
      
    async def decompose_missions(self, missions: list[str]) -> None:
      async_tasks = []
      for mission in missions:
        task = self._analyze_mission(mission)
        async_tasks.append(task)
      
      await asyncio.gather(*async_tasks)  
        
    async def _analyze_mission(self, mission: str) -> None:
        task = await self._mission_analyzer.analyze_mission(mission)
        async with self._lock:
          self._update_storages(task)
      
    async def _recursive_item_decomposition(self, resources: dict, iteration = 0, max_iterations = 9) -> None:
      if iteration >= max_iterations:
        raise RecursionError("Превышено количество итерации")
      iteration += 1
      remaining_items = await self._decompose_resources(list(resources.items()))
      if (len(remaining_items.keys()) != 0):
        await self._recursive_item_decomposition(remaining_items, iteration, max_iterations)
      
    async def _decompose_resources(self, resources: list) -> dict:
      template = "Get {} {}"
      self._items_dict = {}
      async_tasks = []
      
      for item in resources:
        task = asyncio.create_task(self._decompose_item(template.format(item[1], item[0])))
        async_tasks.append(task)
      
      await asyncio.gather(*async_tasks)  
      
      return self._items_dict
    
    async def _decompose_item(self, item: str) -> None:
      try:
        res = await self._mission_analyzer.analyze_obtaining(item)
        async with self._lock:
          self._update_storages(res)
      except RuntimeError as exc:
        self._logger.critical(f"Error with answering question - {item}")   
      
    def _update_storages(self, mission: dict) -> None:
      self._update_action_storage(mission)
      
      if mission.get("Instruments"):
        for tool in mission["Instruments"]:
          if tool not in self._tools:
            self._tools.append(tool)
            
        if self._instruments_as_items:
          mission["Items"] = mission.get("Items", [])
          mission["Items"].extend([(instrument, 1) for instrument in mission["Instruments"] 
                                   if instrument not in ["Workbench", "Furnace"]])
          
      if mission["Type"] == "Obtaining":
        self._update_final_items(mission)
      if mission["Type"] == "Smelting":
        self._smelt_items_count += mission["Smelting"]
        
      self._update_resources_dict(mission)

    def _update_action_storage(self, mission: dict) -> None:
        new_action = mission["Action"]
        non_numeric_action = re.sub(r"\d+", "", new_action)
        for action in self._actions_storage[:]:
          if re.sub(r"\d+", "", action) == non_numeric_action:
            self._actions_storage.remove(action)
            new_action = self._merge_two_actions(new_action, action)
        self._actions_storage.insert(0, new_action)
    
    def _merge_two_actions(self, action1: str, action2: str) -> str:
      matches1, matches2 = list(re.finditer(r"\d+", action1)), list(re.finditer(r"\d+", action2))
      new_values = ([int(m1.group(0)) + int(m2.group(0)) for m1, m2 in zip(matches1, matches2)])
      for m1, new in zip(matches1, new_values):
          action1 = self._mission_analyzer.replace_array_slice(action1, str(new), m1.start(), m1.end())
      return action1

    def _update_resources_dict(self, mission: dict) -> None:
      for item in mission.get("Items", []):
        self._items_dict[item[0]] = self._items_dict.get(item[0], 0)
        self._items_dict[item[0]] += item[1]
          
    def _update_final_items(self, mission: dict) -> None:
      mission_copy = deepcopy(mission)
      for item in mission_copy.get("Items", []):
        if item[0] in self._tools:
          mission_copy["Items"].remove(item)
       
      # if (len(mission_copy.get("Items", [])) == 0 and mission["Action"] != self._actions_storage[-1]):
      if (len(mission_copy.get("Items", [])) == 0):
        existing_info = list(filter(lambda x: x["Item_data"][0] == mission_copy["Item_data"][0], self._final_items))
        if (len(existing_info) == 0):
          self._final_items.append(mission_copy)
        else:
          existing_data, new_data = existing_info[0]["Item_data"], mission_copy["Item_data"]
          existing_info[0]["Action"] = self._mission_analyzer.multiply_task_numbers(existing_info[0]["Action"],
                                                                        (existing_data[1] + new_data[1]) / existing_data[1])
          existing_data[1] += mission_copy["Item_data"][1]
        
        
        
        