from Project.MissionDecomposition.code.openai_clients import ChatClient
from voyager.prompts import load_prompt


class TaskSpecifier():
    def __init__(self,
        openAI: ChatClient.__annotations__,
        recognition_prompt: str = "taskAnalyzer",
    ):
        self._client = openAI
        self._prompt = load_prompt(recognition_prompt)
    
    
    async def determine_task_mission(self, task: str) -> str:
        return await self._client.chat_llm(self._prompt, task)

        
            
    