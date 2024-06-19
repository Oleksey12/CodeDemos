from Project.MissionDecomposition.code.openai_clients import EmbeddingClient, ChatClient
from Project.MissionDecomposition.code.mongodb_search_client import MongoDBCollectionSearch
from Project.MissionDecomposition.code.api_keys import openai_key, mongodb_key 
from Project.MissionDecomposition.code.task_specifiers import TaskSpecifier 
from Project.MissionDecomposition.code.main import TaskDekompositionAgent
from Project.MissionDecomposition.code.mission_analyzer import MissionAnalyzer
from Project.MissionDecomposition.code.item_analyzer import ItemAnalyzer
from gradio.themes.base import Base
import gradio as gr
import pytest
import logging
import json
import os

@pytest.fixture(scope="session")
def decomposition_module() -> TaskDekompositionAgent.__annotations__:
    test_dir = os.path.join("logs", "tests.log")    
    
    chat_client = ChatClient(openai_key, log_file_path=test_dir, log_level=logging.DEBUG)
    embedding_client = EmbeddingClient(openai_key, log_file_path=test_dir, log_level=logging.DEBUG)
    mongodb_client = MongoDBCollectionSearch(embedding_client, 
                                            mongodb_key, 
                                            "MinecraftHelper",
                                            "Items", 
                                            log_file_path=test_dir,
                                            log_level=logging.DEBUG)
    
    item_analyzer = ItemAnalyzer(chat_client, 
                                 mongodb_client, 
                                 use_cache=False, 
                                 log_file_path=test_dir, 
                                 log_level=logging.DEBUG)
    
    task_specifier = TaskSpecifier(chat_client)
    mission_analyzer = MissionAnalyzer(item_analyzer, 
                                        task_specifier,
                                        log_file_path=test_dir, 
                                        log_level=logging.DEBUG)
    
    return TaskDekompositionAgent(mission_analyzer, max_iterations=10, instrumnets_as_items=False)

def test_no_error_simple_mission(decomposition_module: TaskDekompositionAgent.__annotations__):
    """Проверка модуля декомпозиции на простом добываемом предмете  
    Args:
        decomposition_module (TaskDecompositionAlgorithm.__annotations__): _description_
    """
    input_mission = ["Get 1 diamond"]
    assert_test = "Функция завершила работу с ошибкой = {}"
    try:
        result = decomposition_module.run(input_mission)
        with open("result0.txt", 'w') as f:
            json.dump(result["Beutified mission items"], f)
            json.dump(result["Action sequence"], f)
            json.dump(result["Required tools"], f)
    except Exception as exc:
        assert False, assert_test.format(str(exc))

def test_no_error_composite_mission(decomposition_module: TaskDekompositionAgent.__annotations__):
    """Определение ресурсов и действий, необходимых для создания предмета
    Args:
        decomposition_module (MissionAnalyzer.__annotations__): _description_
    """
    input_mission = ["Get 1 piston"]
    assert_test = "Функция завершила работу с ошибкой = {}"
    try:
        result = decomposition_module.run(input_mission)
        with open("result1.txt", 'w') as f:
            json.dump(result["Beutified mission items"], f)
            json.dump(result["Action sequence"], f)
    except Exception as exc:
        assert False, assert_test.format(str(exc))
        
        
def test_recreate_inventory(decomposition_module: TaskDekompositionAgent.__annotations__):
    """Определяем ресурсы и действия, необходимые для получения этого инвентаря
        https://ibb.co/DQ6GMDv
    Args:
        decomposition_module (MissionAnalyzer.__annotations__): _description_
    """
    input_mission = ["Get 1 diamond sword", 
                     "Get 1 enchanting table",
                     "Get 40 lapis lazurite",
                     "Get 4 book",
                     "Get 4 golden apple", 
                     "Get 1 diamond chestplate",
                     "Get 1 iron leggings",
                     "Get 1 iron boots",
                     "Get 1 bow",
                     "Get 40 arrow"]
    
    assert_test = "Module ended with error={}"
    try:
        result = decomposition_module.run(input_mission)
        with open("result2.txt", 'w') as f:
            json.dump(result["Beutified mission items"], f)
            json.dump(result["Mission items"], f)
    except Exception as exc:
        assert False, assert_test.format(str(exc))
        
        

        