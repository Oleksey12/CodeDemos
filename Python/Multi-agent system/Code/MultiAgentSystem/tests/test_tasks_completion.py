from voyager import Voyager
from Project.MissionInterpretation.code.Interpretation import InterpretationAgent

from Project.MissionDecomposition.code.main import TaskDekompositionAgent 
from Project.MissionDecomposition.code.mission_analyzer import MissionAnalyzer

from Project.MissionDecomposition.code.openai_clients import EmbeddingClient, ChatClient
from Project.MissionDecomposition.code.mongodb_search_client import MongoDBCollectionSearch
from Project.MissionDecomposition.code.api_keys import openai_key, mongodb_key 
from Project.MissionDecomposition.code.task_specifiers import TaskSpecifier 
from Project.MissionDecomposition.code.item_analyzer import ItemAnalyzer

from Project.MultiAgentSystem.oldCode.OperationCenterOld import AgentGroupOperator
from Project.MultiAgentSystem.code.WorkingAgent import WorkingAgent
import logging
import pytest
import os

@pytest.fixture()
def voyagers():
    return {name : Voyager(bot_name=name,
                                    enable_pauses=False,
                                    server_port=3000 + num,
                                    mc_port=25565,
                                    openai_api_key=openai_key,
                                    ckpt_dir="test1",
                                    resume=True) for num, name in enumerate(["Anna", "Nikita", "Steve"])}

@pytest.fixture()
def operation_center(voyagers):
    errors_dir = os.path.join("logs", "main.log")
    center_dir = os.path.join("logs", "center.log")
    chat_dir = os.path.join("logs", "chat.log")
    log_level = logging.INFO
    
    chat_client = ChatClient(openai_key, temprature=0.5, log_file_path=errors_dir, log_level=log_level)
    code_client = ChatClient(openai_key, temprature=0, log_file_path=errors_dir, log_level=log_level)
    embedding_client = EmbeddingClient(openai_key, log_file_path=errors_dir, log_level=log_level)
    mongodb_client = MongoDBCollectionSearch(embedding_client, 
                                            mongodb_key, 
                                            "MinecraftHelper",
                                            "Items", 
                                            log_file_path=errors_dir,
                                            log_level=log_level)

    interpretation_agent = InterpretationAgent(chat_client)
    
    item_analyzer = ItemAnalyzer(chat_client, 
                                 mongodb_client, 
                                 use_cache=True, 
                                 log_file_path=errors_dir, 
                                 log_level=log_level)
    
    task_specifier = TaskSpecifier(chat_client)
    mission_analyzer = MissionAnalyzer(item_analyzer, 
                                       task_specifier,
                                       log_file_path=errors_dir, 
                                       log_level=log_level)
    
    task_decomposition = TaskDekompositionAgent(mission_analyzer, max_iterations=4)
    
    base = [3030, 80, 3030]
    return AgentGroupOperator(interpretation_agent, 
                            task_decomposition, 
                            mission_analyzer, 
                            code_client, 
                            chat_client,
                            openai_key,
                            25565,
                            base,
                            voyagers=voyagers,
                            operator_log_path=center_dir,
                            allocation_agent_log_path=chat_dir,
                            allocation_manager_log_path=chat_dir)
    
@pytest.mark.asyncio
async def test_no_errors(operation_center, voyagers):
    tasks = {"Anna": [],
            "Steve": [],
            "Nikita": [{"Name": "Mine 6 stripped log", "Instruments": ["axe"], "Action": "Mine 6 any wood log to get 6 matching log"}]}
    
    working_agents = [WorkingAgent(name, tasks[name], v, None) for v, name in zip(voyagers.values(), ["Anna", "Nikita", "Steve"])]
    try:
        voyagers["Nikita"].create_embodiment()
        voyagers["Nikita"].bot_write_to_chat("/give @p minecraft:crafting_table")
        await operation_center.complete_tasks(working_agents)
        for v in voyagers.values():
            v.env.close()
    except Exception as err:
        for v in voyagers.values():
            v.env.close()
        raise RuntimeError("FAILED")
