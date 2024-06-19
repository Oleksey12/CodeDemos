from Project.MissionInterpretation.code.Interpretation import InterpretationAgent
from Project.MissionDecomposition.code.mongodb_search_client import MongoDBCollectionSearch
from Project.MissionDecomposition.code.openai_clients import ChatClient, EmbeddingClient

from Project.MissionDecomposition.code.main import TaskDekompositionAgent 
from Project.MissionDecomposition.code.mission_analyzer import MissionAnalyzer
from Project.MissionDecomposition.code.task_specifiers import TaskSpecifier 
from Project.MissionDecomposition.code.item_analyzer import ItemAnalyzer

from Project.MultiAgentSystem.code.Handlers.DecompositionHandler import DecompositionHandler
from Project.MultiAgentSystem.code.Handlers.InputHandler import InputHandler
from Project.MultiAgentSystem.code.Handlers.AllocationHandler import AllocationHandler
from Project.MultiAgentSystem.code.Handlers.ExecutionHandler import ExecutionHandler
from Project.MultiAgentSystem.code.Handlers.GatherHandler import GatherHandler

from voyager import Voyager
import logging
import os

class InitializeSystem:
    def __init__(self,
                 base: list[int],
                 openai_key: str,
                 mongodb_key: str,
                 ckpt_dir,
                 enable_pauses = False,
                 mc_port=25565,
                 agent_names = ["Anna", "Steve", "Nikita"],
                 analyze_instruments: bool = False,
                 erros_log_dir: str = os.path.join("logs", "main.log"),
                 errors_log_level: int = logging.INFO,
                 allocation_log_dir: str = os.path.join("logs", "allocation.log"),
                 allocation_log_level: int = logging.INFO,
                 main_log_dir: str = os.path.join("logs", "handlers.log"),
                 main_log_level: int = logging.INFO):
        
        self._base = base
        
        self._allocation_log_dir = allocation_log_dir
        self._allocation_log_level = allocation_log_level
        self._main_log_dir = main_log_dir
        self._main_log_level = main_log_level
        
        self._code_client = ChatClient(openai_key, temprature=0, log_file_path=erros_log_dir, log_level=errors_log_level)
        self._chat_client = ChatClient(openai_key, temprature=0.5, log_file_path=erros_log_dir, log_level=errors_log_level)

        self.voyagers = {name : Voyager(self._code_client,
                            self._code_client,
                            self._code_client,
                            self._code_client,
                            self._code_client,
                            bot_name=name,
                            enable_pauses=enable_pauses,
                            server_port=3000 + num,
                            mc_port=mc_port,
                            openai_api_key=openai_key,
                            ckpt_dir=ckpt_dir,
                            resume=True) for num, name in enumerate(agent_names)}
        
        self._interpretation_agent = InterpretationAgent(self._code_client)
        self._embedding_client = EmbeddingClient(openai_key, log_file_path=erros_log_dir, log_level=errors_log_level)

        self._mongodb_client = MongoDBCollectionSearch(self._embedding_client, 
                                                mongodb_key, 
                                                "MinecraftHelper",
                                                "Items", 
                                                log_file_path=erros_log_dir,
                                                log_level=errors_log_level)

        self._item_analyzer = ItemAnalyzer(self._code_client, 
                                    self._mongodb_client, 
                                    use_cache=True, 
                                    log_file_path=erros_log_dir, 
                                    log_level=errors_log_level)

        self._task_specifier = TaskSpecifier(self._code_client)
        self._mission_analyzer = MissionAnalyzer(self._item_analyzer, 
                                            self._task_specifier,
                                            log_file_path=erros_log_dir, 
                                            log_level=errors_log_level)
        
        self._task_decomposition = TaskDekompositionAgent(self._mission_analyzer, 
                                                          max_iterations=10, 
                                                          instrumnets_as_items=analyze_instruments)
    
    def get_handlers(self) -> dict[str, list]:
        handler5 = GatherHandler(self.voyagers, self._base, 5, log_path=self._main_log_dir)
        handler4 = ExecutionHandler(self._mission_analyzer, self.voyagers, 4, next_handler=handler5, log_path=self._main_log_dir)
        handler3 = AllocationHandler(self.voyagers, 
                                     3, 
                                     self._code_client, 
                                     self._chat_client, 
                                     self._base, 
                                     handler4, 
                                     allocation_agent_log_path=self._allocation_log_dir,
                                     allocation_manager_log_path=self._allocation_log_dir,
                                     log_path=self._main_log_dir)
        
        handler2 = DecompositionHandler(self._task_decomposition, 2, handler3, log_path=self._main_log_dir)
        handler1 = InputHandler(self._interpretation_agent, 1, handler2, log_path=self._main_log_dir)
        
        result = {"Main": [handler1, handler2, handler3, handler4, handler5], 
                  "Fix": [], 
                  "Other": []}
        return result

        
        