from Project.MissionInterpretation.code.Interpretation import InterpretationAgent

from Project.MissionDecomposition.code.main import TaskDekompositionAgent 
from Project.MissionDecomposition.code.mission_analyzer import MissionAnalyzer

from Project.MissionDecomposition.code.openai_clients import EmbeddingClient, ChatClient
from Project.MissionDecomposition.code.mongodb_search_client import MongoDBCollectionSearch
from Project.MissionDecomposition.code.api_keys import openai_key, mongodb_key 
from Project.MissionDecomposition.code.task_specifiers import TaskSpecifier 
from Project.MissionDecomposition.code.item_analyzer import ItemAnalyzer

from Project.MultiAgentSystem.oldCode.OperationCenterOld import AgentGroupOperator
import logging
import os


if __name__ == "__main__":
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

    interpretation_agent = InterpretationAgent(code_client)
    
    item_analyzer = ItemAnalyzer(code_client, 
                                 mongodb_client, 
                                 use_cache=True, 
                                 log_file_path=errors_dir, 
                                 log_level=log_level)
    
    task_specifier = TaskSpecifier(code_client)
    mission_analyzer = MissionAnalyzer(item_analyzer, 
                                       task_specifier,
                                       log_file_path=errors_dir, 
                                       log_level=log_level)
    
    task_decomposition = TaskDekompositionAgent(mission_analyzer, max_iterations=10, instrumnets_as_items=False)
    
    base = [3120, 87, 3035]
    agent_operator = AgentGroupOperator(interpretation_agent, 
                                        task_decomposition, 
                                        mission_analyzer, 
                                        code_client, 
                                        chat_client,
                                        openai_key,
                                        25565,
                                        base,
                                        operator_log_path=center_dir,
                                        allocation_agent_log_path=chat_dir,
                                        allocation_manager_log_path=chat_dir)
    
    res = agent_operator.start()
    
    
    