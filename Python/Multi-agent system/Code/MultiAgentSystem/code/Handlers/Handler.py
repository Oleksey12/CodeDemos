from abc import ABCMeta, abstractmethod

class Handler(metaclass=ABCMeta):
    def __init__(self, number, nextHandler = None) -> None:
        self.number = number
        self.name = "AbstractHandler" 
        self._nextHandler = nextHandler
    
    def handle_request(self, request: dict):
        modified_request = self.modify_request(request)
        yield modified_request
        if (self._nextHandler and modified_request["Success"]):
           yield from self._nextHandler.handle_request(modified_request)

    @abstractmethod
    def modify_request(self, request) -> dict:
        pass
