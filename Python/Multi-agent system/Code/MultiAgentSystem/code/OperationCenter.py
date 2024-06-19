from Project.MultiAgentSystem.code.ColoredPrint import Colors, colored_output     
from Project.MultiAgentSystem.code.InitializeSystem import InitializeSystem
from Project.MissionDecomposition.code.api_keys import openai_key, mongodb_key
import asyncio

base = [3120, 87, 3035]


system_handlers = InitializeSystem(base, openai_key, mongodb_key, "test1")
handlers = system_handlers.get_handlers()
start_handler = handlers["Main"][0]

colored_output('Введите задание на английском языке, или команду "/stop"', Colors.BLINK, end=" ")
# user_task = input().strip()
user_task = "Get 3 cobblestone"
if user_task == "/stop":
    raise RuntimeError("SystemClose")

for result in start_handler.handle_request({"Input": user_task}):
    # colored_output(f'Требуемые ресурсы и последовательность действия для выполнения заданий:\n{mission_items}\nДействия:\n{action_sequence}', Colors.BLINK, end=" ")
    print(result, end="\n\n")

tasks = []
for v in system_handlers.voyagers.values():
    tasks.append(asyncio.create_task(v.env.close()))

_ = asyncio.run(asyncio.wait(tasks))
        
