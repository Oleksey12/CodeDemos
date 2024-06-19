from Project.MissionDecomposition.code.main import TaskDekompositionAgent
from Project.MultiAgentSystem.code.ColoredPrint import Colors, colored_output
from Project.MissionDecomposition.code.custom_loggers import create_logger
from Project.MultiAgentSystem.code.Handlers.Handler import Handler
import logging
import json
import os


class DecompositionHandler(Handler):
    def __init__(self, 
                decomposition_agent: TaskDekompositionAgent.__annotations__,
                number: int, 
                next_handler: Handler = None,
                log_path: str = os.path.join("logs", "operator.log"),
                log_level = logging.INFO):

        super(DecompositionHandler, self).__init__(number, next_handler)
        self.name = "DecompositionHandler"
        self._decomposition_agent = decomposition_agent
        self._logger = create_logger(logging.getLogger(f"{__name__}.DecompositionHandler"), 
                                            log_path, 
                                            log_level)
    
    
    def modify_request(self, request) -> dict:
        try:
            mission_list = request["Input"]
            task_info: dict = self._decomposition_agent.run(mission_list)
        except RuntimeError as err:
            colored_output('Ошибка при декомпозиции задания!', Colors.BLINK)
            self._logger.error(f"Ошибка при декомпозиции задания - {mission_list}, ошибка - {err}")
            request["Success"] = False
            request["Error"] = err
            return request
        mission_items = "\n".join([f"{num+1}) {item}" for num, item in enumerate(task_info["Beutified mission items"])])
        action_sequence = "\n".join(f"{num+1}) {json.dumps(action)}" for num, action in enumerate(task_info["Action sequence"]))
        self._logger.info(f"Миссии {json.dumps(mission_list)}:\nТребуемые предметы:\n{mission_items}\nДействия:\n{action_sequence}\n")
        
        request["Input"] = task_info
        request["Success"] = True
        return request
    