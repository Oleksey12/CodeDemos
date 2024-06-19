from Project.MissionDecomposition.code.openai_clients import EmbeddingClient, ChatClient
from Project.MissionDecomposition.code.mongodb_search_client import MongoDBCollectionSearch
from Project.MissionDecomposition.code.api_keys import openai_key, mongodb_key 
from Project.MissionDecomposition.code.task_specifiers import TaskSpecifier 
from Project.MissionDecomposition.code.item_analyzer import ItemAnalyzer
from Project.MissionDecomposition.code.custom_loggers import create_logger
import voyager.utils as U
from math import ceil
import logging
import re
import os

{
    "Obtaining" : "Task that includes obtaining items by crafting, killing monster, breaking blocks and looting chests",
    "Killing": "Tasks that are connected with destroying and killing living entities with weapons",
    "Smelting": "Tasks connected with furnace usage",
    "Mining": "Tasks about breaking and removing blocks",
    "Other": "Tasks about item usage, exploring and other strange things"
}


    
class MissionAnalyzer():
    def __init__(self,
        item_analyzer: ItemAnalyzer.__annotations__,
        task_specifier: TaskSpecifier.__annotations__,
        include_instruments = True,
        log_file_path = os.path.join("logs", "main.log"),
        log_level = logging.INFO
    ):
        self._logger = create_logger(logging.getLogger(f"{__name__}.MissionAnalyzer"), 
                                                   log_file_path, 
                                                   log_level)
        U.f_mkdir(os.path.dirname(log_file_path))
        
        self._item_analyzer = item_analyzer
        self._task_specifier = task_specifier
        self._include_instruments = include_instruments
    
    
    async def analyze_mission(self, task: str) -> dict:
        """
        Kill 1 zombie; Get 10 obsidian; Get 10 blaze powder
        Достаёт из предложения-миссии:
        1) Действие
        2) Инструменты
        3) Предметы и их количество*
        4) Длительность переплавки*
        
        1) Копия миссии
        2) ...
        3) Только для заданий связанных с предметами или переплавкой
        4) Только для заданий связанных с предметом или переплавкой
        """
        task_type = await self._task_specifier.determine_task_mission(task)
        
        function_list: dict = {
            "Obtaining": self.analyze_obtaining,
            "Smelting": self._analyze_smelting
        }
        
        default = self._analyze_other
        
        task_process_fucntion: callable = function_list.get(task_type, default)
        
        task_info = await task_process_fucntion(task, task_type)
        return task_info
        
    
    # TODO:
    # Add analyzer for mining section
    async def _analyze_other(self, task: str, task_type: str) -> dict:
        action = self.set_task_type_verb(task, task_type)
        task_info = {"Action": action}
        task_info["Type"] = task_type
        if re.search("\bwith\b", task):
            self._logger.warning(f"Задание со специальными условиями: {task}")
            instruments_list = task[task.find("with") + 5].split(", ")
            task_info["Instruments"] = instruments_list
            
        if task_type == "Killing":
            task_info["Instruments"] = ["Stone sword"]
            
        return task_info
    
    async def _analyze_building(self, task: str, task_type: str) -> dict:
        raise NotImplementedError("hello")
    
    async def _analyze_smelting(self, task: str, task_type: str = "Smelting") -> dict:
        task_info = await self._analyze_other(task, task_type)
        smelt_duration = int(re.search(r"\d+", task).group(0))
        
        item_index = re.search(r"\d+", task).end() + 1
        item_end = task.find("with") 
        item = task[item_index:] if item_end == -1 else task[item_index: item_end - 1]
        
        task_info["Items"] = [(item, smelt_duration)]
        task_info["Instruments"] = task_info.get("Instruments", [])
        task_info["Instruments"].append("Furnace")
        task_info["Smelting"] = smelt_duration
        
        return task_info
        
    async def analyze_obtaining(self, task: str, task_type: str = "Obtaining") -> dict:
        task_info = {}
        total_count = int(re.search(r'\d+', task).group(0))
        
        item_start_index = re.search(r'\d+', task).end() + 1
        item_end_index = task.find("with") 
        item_name = task[item_start_index:] if item_end_index == -1 else task[item_start_index: item_end_index - 1]
        
        item_obtaining_guide: str = await self._item_analyzer.get_item_guide(item_name)
        
        task_info["Item_data"] = [item_name, total_count]
        task_info["Type"] = task_type
        
        
        if item_obtaining_guide == "Unknown":
            self._logger.error(f"Игровой предмет {item_name} не существует!")
            raise Exception("Неизвестный предмет")
        
        obtain_method = re.search(r"\w+", item_obtaining_guide).group(0)
        
        # Вычисляем желаемое количество предметов
        item_obtaining_guide = self.adjust_item_count(total_count, item_obtaining_guide, obtain_method)
        task_info["Action"] = item_obtaining_guide
        task_info["Instruments"] = []
        # Обрабатываем каждый вид инструкций по получению предмета отдельно
        if obtain_method == "Mine":
            if (item_obtaining_guide.find("with") != -1):
                start = item_obtaining_guide.find("with") + 5 
                end = item_obtaining_guide.find("to get") - 1
                if(item_obtaining_guide[start:end] not in ["hand", "any tool", "any"]):
                    task_info["Instruments"].append(item_obtaining_guide[start:end])
        elif obtain_method == "Kill":
            task_info["Instruments"].append("Stone sword")
        elif obtain_method == "Craft":
            start = item_obtaining_guide.find("from") + 5 
            end = item_obtaining_guide.find("to get") - 1
            task_info["Instruments"].append("Workbench")
            resources_list = list(map(lambda x: x.lower().strip(), item_obtaining_guide[start: end].split(", ")))
            task_info["Items"] = [] 
            for resource in resources_list:
                index =  re.search("\d+", resource)
                task_info["Items"].append((resource[index.end() + 1:], int(resource[:index.end()])))
        elif obtain_method == "Smelt":
            start = re.search(r"\d+", item_obtaining_guide).end() + 1
            end = item_obtaining_guide.find("to get") - 1
            task_info["Items"] = [(item_obtaining_guide[start: end], total_count)]
            task_info["Instruments"].append("Furnace")
            task_info["Smelting"] = total_count
        else:
            self._logger.error(f"Игровой предмет {item_name} нельзя добыть!")
            raise Exception("Невозможно добыть предмет из мисси", item_obtaining_guide)
        
        return task_info

    def adjust_item_count(self, total_count: int, item_obtaining_guide: str, obtain_method: str):
        if obtain_method.strip().capitalize() != "Craft":
            index = item_obtaining_guide.find(" ")
            if obtain_method == "Kill":
                index = item_obtaining_guide.find("to get") + 6
            item_obtaining_guide = self.replace_array_slice(item_obtaining_guide, " 1 ", index, index + 1)
            item_obtaining_guide = self.multiply_task_numbers(item_obtaining_guide, total_count)
        else:
            index = item_obtaining_guide.find("to get") + 7
            recipe_count = int(re.search("\d+", item_obtaining_guide[index:]).group(0))
            item_obtaining_guide = self.multiply_task_numbers(item_obtaining_guide, ceil(total_count / recipe_count))
        return item_obtaining_guide
            
    def set_task_type_verb(self, task: str, task_type: str) -> str:
        if task_type != "Other":
            verb = task_type[:-3]
            if task_type == "Mining":
                verb += "e"
            index = re.search(r"\d+", task).start() - 1
            task = self.replace_array_slice(task, verb, 0 , index) 

        return task

    def multiply_task_numbers(self, sentence: str, multiplyer: int) -> str:
        numbers_positions = re.finditer(r"\d+", sentence)
        for index in list(numbers_positions)[::-1]:
            number = int(sentence[index.start():index.end()])
            sentence = self.replace_array_slice(sentence, str((int)(number * multiplyer)), index.start(), index.end())
        return sentence        
        
    def replace_array_slice(self, text: str, replace_text: str, start_index: int, end_index: int) -> str:
        return text[:start_index] + replace_text + text[end_index:]    




if __name__ == "__main__":
    chat_client = ChatClient(openai_key)
    embedding_client = EmbeddingClient(openai_key)
    mongodb_client = MongoDBCollectionSearch(embedding_client, 
                                                mongodb_key, 
                                                "MinecraftHelper",
                                                "Items")
    
    item_analyzer = ItemAnalyzer(chat_client, mongodb_client)
    task_specifier = TaskSpecifier(chat_client)
    
    mission_client = MissionAnalyzer(item_analyzer, task_specifier)
    test_list = ["Kill 1 wither", 
                 "Get 1 beacon", 
                 "Get 88 netherite ingot", 
                 "Get 200 netherite scrap", 
                 "Get 5 bone", 
                 "Get 14 blaze rod"]
    
    for item in test_list:
        print(mission_client.analyze_mission(item))
        
        