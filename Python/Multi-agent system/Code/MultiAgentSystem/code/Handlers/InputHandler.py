from Project.MissionInterpretation.code.Interpretation import InterpretationAgent
from Project.MultiAgentSystem.code.ColoredPrint import Colors, colored_output
from Project.MissionDecomposition.code.custom_loggers import create_logger
from Project.MultiAgentSystem.code.Handlers.Handler import Handler
import logging
import os



class InputHandler(Handler):
    def __init__(self, 
                 user_agent: InterpretationAgent, 
                 number: int, 
                 next_handler: Handler = None,
                 log_path: str = os.path.join("logs", "operator.log"),
                 log_level = logging.INFO):

        super(InputHandler, self).__init__(number, next_handler)
        self._logger = create_logger(logging.getLogger(f"{__name__}.InputHandler"), 
                                        log_path, 
                                        log_level)
        self.name = "InputHandler"
        self._user_agent = user_agent
        
    
    def modify_request(self, request) -> dict:
        try:
            user_task = request["Input"]
            result = self._user_agent.format_task(user_task)
        except RuntimeError as err:
            colored_output('ОШИБКА! Попробуйте ещё раз', Colors.BLINK)
            self._logger.warning(f"Некорректное задание - {user_task}, ошибка - {err}")
            request["Success"] = False
            request["Error"] = err
            return request

        request["Input"] = result
        request["Success"] = True
        return request
    
    
    