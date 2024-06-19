from Project.MissionDecomposition.code.mission_analyzer import MissionAnalyzer
from voyager import Voyager



class WorkingAgent():
    def __init__(self,
                 name: str,
                 tasks: list[dict],
                 voyager: Voyager.__annotations__,
                 missionAnalyzer: MissionAnalyzer.__annotations__
    ):
        self.name = name
        self.voyager = voyager
        self._missionAnalyzer = missionAnalyzer
        # Filter task to remove the waiting tasks
        self._tasks = tasks
        
        self._potential_instruments = self.find_possible_instruments()
    
    def get_instrument(self, name: str):
        if name in self._potential_instruments:
            self._potential_instruments.remove(name)
            
    def find_possible_instruments(self) -> list:
        instruments = []
        for task in self._tasks:
            if task.get("Type") and any([t.startswith("Cooperative instruments") for t in task["Type"]]):
                instruments.extend(task["Instruments"])
        return instruments
                
    def manage_tasks(self) -> dict:
        for task in self._tasks:
            if task["Name"].startswith("Give"):
                self._tasks.remove(task)
                return task
            
        for task in self._tasks:
            if not self._potential_instruments:
                self._tasks.remove(task)
                return task
            
            instruments_info = [instrument not in self._potential_instruments for instrument in task["Instruments"]]
            if not instruments_info or all(instruments_info):
                self._tasks.remove(task)
                return task
        return None
    
    async def execute_task(self, task, retries=3):
        json_result = {"Task": task, "Agents": [self], "Done": True}
        try: 
            for _ in range(retries):
                # self._voyager.observe()
                context = await self.voyager.curriculum_agent.get_task_context(task)
                *_, info = await self.voyager.rollout(
                    task=task,
                    context=context,
                    reset_env=True,
                )
                if (info["success"]):
                    break
            else:
                json_result["Done"] = False
        except RuntimeError as exc:
            json_result["Done"] = False
        return json_result
    
    

    
    