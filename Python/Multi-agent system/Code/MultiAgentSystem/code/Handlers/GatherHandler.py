from Project.MissionDecomposition.code.custom_loggers import create_logger
from Project.MultiAgentSystem.code.ColoredPrint import Colors, colored_output
from Project.MultiAgentSystem.code.Handlers.Handler import Handler

from voyager import Voyager
import logging
import asyncio
import os

class GatherHandler(Handler):
    def __init__(self, 
                 voyagers: dict[Voyager],
                 base: list[int],
                 number: int,
                 next_handler = None,
                 log_path: str = os.path.join("logs", "operator.log"),
                 log_level = logging.INFO):
        
        super(GatherHandler, self).__init__(number, next_handler)
        self._logger = create_logger(logging.getLogger(f"{__name__}.GatherHandler"), 
                                            log_path, 
                                            log_level)
        self.name = "GatherHandler"
        self._voyagers = voyagers
        self._base_coordinates = base
        self._storeItems = """async function storeInventory(bot) {{
    await bot.pathfinder.goto(new GoalNear({base}, 5));
    
    const chestBlock = bot.findBlock({{
        point: bot.entity.position,
        matching: block => block.name === 'chest',
        maxDistance: 64,
        minCount: 1,
    }});
    
    await bot.pathfinder.goto(new GoalNear(chestBlock.position.x, chestBlock.position.y, chestBlock.position.z, 2));
    
    const chest = await bot.openChest(chestBlock);
    for (const item of bot.inventory.items()) {{
        await chest.deposit(item.type, null, item.count);
    }}
    await chest.close();
}}
await storeInventory(bot);"""
    
    def modify_request(self, request) -> dict:
        coordinates = f"{self._base_coordinates[0]}, {self._base_coordinates[1]}, {self._base_coordinates[2]}"
        try:
            request["Input"] = asyncio.run(self._store_all_resources(coordinates))
        except RuntimeError as err:
            colored_output("Ошибка при загрузке добытых предметов!", color=Colors.RED)
            self._logger.error(f"Ошибка агенты не смогли выложить свою добычу в сундук! Ошибка:\n{err}!")
            request["Error"] = err
            request["Success"] = False
            return request
        
        request["Success"] = True
        return request  

    async def _store_all_resources(self, coordinates):
        tasks = []
        for v in self._voyagers.values():
            tasks.append(asyncio.create_task(v.bot_execute_code(self._storeItems.format(base=coordinates))))
        return await asyncio.wait(tasks)