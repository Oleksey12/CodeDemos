from Project.MissionDecomposition.code.openai_clients import ChatClient
import voyager.utils as U
from voyager.prompts import load_prompt

class InterpretationAgent:
    def __init__(self,
        chat_client: ChatClient.__annotations__):
        self._prompt = load_prompt("interpretate")
        self._chat_client = chat_client
    
    def format_task(self, sentence: str) -> list[str]:
        response = self._chat_client.chat_llm_sync(self._prompt, sentence)
        res = U.fix_and_parse_json(response)
        return res
            
    
    
    
    
    
    
    