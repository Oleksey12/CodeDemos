from Project.TaskAllocation.code.action_enviroment import TaskAllocationEnvironment
from Project.TaskAllocation.code.agent import Agent
from Project.MissionDecomposition.code.openai_clients import ChatClient
from Project.MissionDecomposition.code.api_keys import openai_key

import pytest
import logging
import os

@pytest.fixture
def log_path():
    return os.path.join("logs", "tests.log")    
    
@pytest.fixture
def log_level():
    return logging.DEBUG

@pytest.fixture
def actions():
    return """1) Give your opinion about the speaker decision. To improve clarity include in your response words, describing emotions and opinion about the chosen task
            2) Ask the speaker, if you can join him making the task together. To improve clarity include in your message one of this phrases: "i want to make this task with you", "join you"
            3) Skip discussion, if you have already gave your opinion. Answer with one word - "Skip"
            4) Ask the speaker to take his mission. To improve clarity include in your message one of this phrases: "take your mission", "do it by myself"
            5) Ask the speaker, if he needs help with instruments for his mission. To improve clarity include in your message one of this phrases: "make instruments for you", "help you with instruments"
            """

def create_tasks() -> list[list]:
    test_cases =  [
        [{"Item_data": ["lapis lazurite", 40], "Type": "Obtaining", "Action": "Mine 40 lapis lazuli ore with stone pickaxe to get 40 lapis lazuli", "Instruments": ["stone pickaxe"]}, {"Item_data": ["leather", 5], "Type": "Obtaining", "Action": "Kill cow to get 5 leather", "Instruments": ["Stone sword"]}, {"Item_data": ["flint", 10], "Type": "Obtaining", "Action": "Mine 10 gravel with a shovel to get 10 flint", "Instruments": ["a shovel"]}, {"Item_data": ["obsidian", 4], "Type": "Obtaining", "Action": "Mine 4 obsidian with diamond pickaxe to get 4 obsidian", "Instruments": ["diamond pickaxe"]}, {"Item_data": ["apple", 4], "Type": "Obtaining", "Action": "Mine 4 oak and dark oak leaves with shears to get 4 apple", "Instruments": ["shears"]}, {"Item_data": ["feather", 10], "Type": "Obtaining", "Action": "Kill adult chickens to get 10 feather", "Instruments": ["Stone sword"]}, {"Item_data": ["diamond", 12], "Type": "Obtaining", "Action": "Mine 12 diamond ore with iron pickaxe to get 12 diamond", "Instruments": ["iron pickaxe"]}, {"Item_data": ["string", 3], "Type": "Obtaining", "Action": "Kill spiders and cave spiders to get 3 string", "Instruments": ["Stone sword"]}, {"Item_data": ["iron ore", 11], "Type": "Obtaining", "Action": "Mine 11 stone with stone pickaxe to get 11 iron ore", "Instruments": ["stone pickaxe"]}],
        [{"Item_data": ["string", 3], "Type": "Obtaining", "Action": "Kill spiders and cave spiders to get 3 string", "Instruments": ["Stone sword"]}, {"Item_data": ["iron ore", 11], "Type": "Obtaining", "Action": "Mine 11 iron ore with stone pickaxe to get 11 iron ore", "Instruments": ["stone pickaxe"]}, {"Item_data": ["sugar cane", 15], "Type": "Obtaining", "Action": "Mine 15 sugar cane with any tool to get 15 sugar cane", "Instruments": ["any tool"]}, {"Item_data": ["gold ore, nether gold ore, or deepslate gold ore", 32], "Type": "Obtaining", "Action": "Mine 32 gold ore with iron pickaxe to get 32 gold ore\nmine nether gold ore with iron pickaxe to get 32 nether gold ore\nmine deepslate gold ore with iron pickaxe to get 32 deepslate gold ore", "Instruments": ["iron pickaxe"]}, {"Item_data": ["matching log", 2], "Type": "Obtaining", "Action": "Mine 2 stripped log with any axe to get 2 matching log", "Instruments": ["any axe"]}],
        [{"Item_data": ["lapis lazurite", 40], "Type": "Obtaining", "Action": "Mine 40 lapis lazuli ore with stone pickaxe to get 40 lapis lazuli", "Instruments": ["stone pickaxe"]}, {"Item_data": ["leather", 5], "Type": "Obtaining", "Action": "Kill cow to get 5 leather", "Instruments": ["Stone sword"]}, {"Item_data": ["flint", 10], "Type": "Obtaining", "Action": "Mine 10 gravel with a shovel to get 10 flint", "Instruments": ["a shovel"]}, {"Item_data": ["obsidian", 4], "Type": "Obtaining", "Action": "Mine 4 obsidian with diamond pickaxe to get 4 obsidian", "Instruments": ["diamond pickaxe"]}, {"Item_data": ["apple", 4], "Type": "Obtaining", "Action": "Mine 4 oak and dark oak leaves with shears to get 4 apple", "Instruments": ["shears"]}, {"Item_data": ["feather", 10], "Type": "Obtaining", "Action": "Kill adult chickens to get 10 feather", "Instruments": ["Stone sword"]}, {"Item_data": ["diamond", 12], "Type": "Obtaining", "Action": "Mine 12 diamond ore with iron pickaxe to get 12 diamond", "Instruments": ["iron pickaxe"]}, {"Item_data": ["string", 3], "Type": "Obtaining", "Action": "Kill spiders and cave spiders to get 3 string", "Instruments": ["Stone sword"]}, {"Item_data": ["iron ore", 11], "Type": "Obtaining", "Action": "Mine 11 stone with stone pickaxe to get 11 iron ore", "Instruments": ["stone pickaxe"]}, {"Item_data": ["sugar cane", 15], "Type": "Obtaining", "Action": "Mine 15 sugar cane with any tool to get 15 sugar cane", "Instruments": ["any tool"]}, {"Item_data": ["gold ore, nether gold ore, or deepslate gold ore", 32], "Type": "Obtaining", "Action": "Mine 32 gold ore with iron pickaxe to get 32 gold ore\nmine nether gold ore with iron pickaxe to get 32 nether gold ore\nmine deepslate gold ore with iron pickaxe to get 32 deepslate gold ore", "Instruments": ["iron pickaxe"]}, {"Item_data": ["matching log", 2], "Type": "Obtaining", "Action": "Mine 2 stripped log with any axe to get 2 matching log", "Instruments": ["any axe"]}]    
    ]
    return test_cases

def enviroment(tasks: list[dict], log_path, log_level) -> TaskAllocationEnvironment.__annotations__:
    return TaskAllocationEnvironment(tasks, ["Anna", "Nikita", "Steve"])

def agent(enviroment: TaskAllocationEnvironment.__annotations__, role: str, log_path, log_level, name="Nikita") -> TaskAllocationEnvironment.__annotations__:
    chat_client = ChatClient(openai_key, temprature=0.5, log_file_path=log_path, log_level=log_level)
    code_client = ChatClient(openai_key, log_file_path=log_path, log_level=log_level)
    
    chat = os.path.join("logs", "chat.log")    
    return Agent(name, chat_client, code_client, enviroment, role, log_file_path=chat, log_level=log_level)

# Discussion error!!!!! Агенты отвечают join it и не создают полноценное предложение

"""Nikita - Anna"""
"""Choosing a task? I don't even know if I should be excited or scared. I won't take any of those, that's too bad."""
"""Choosing a task? I don't even know if I should be excited or scared. But hey, I think I'll make mission 2 - Kill cows. Time to show those cows who's the boss!"""

""" Well done, Nikita! Killing cows is a daring task, but I believe you can handle it. Need any help with instruments?"""
"""request_take_instruments()"""
"""Sure, you can help me with instruments! I will be happy if you make a lasagna sword for me."""

"""Player Anna disscussed: Well done, Nikita! Killing cows can be quite a task, but I believe in your skills. Do you need any help with instruments for this mission?"""
"""Speaker responded: That's too bad, I have to get another task. Don't think that it's a good idea, Anna. The best option for you will be to chill and not do that."""

"""Steve - Anna"""
"""Speaker Steve said: What? What should I do with that? Ah, I see a perfect task for me - mission 3, to mine 10 gravel. I'll grab a shovel and get to work!"""
"""Speaker Steve said: What? What should I do with that? Let me take mission 2 - Kill cow. It's a simple task that I can handle easily."""

"""Well done, Steve! That sounds like an adventurous task. I fully support your decision to take on the challenge of killing cows. Good luck showing those cows who's boss!"""
"""comment()"""

"""Join it! Hey Steve, that sounds like an exciting task! I want to make this task with you. Can I join you?"""
"""request_join()"""
"""Speaker responded: That is a nice idea from you, Anna! Sure, I will be happy if you join me in killing cows. Let's show those cows who's the boss together!"""
"""accept_request()"""

"""Well done, Steve! Killing cows can be quite exciting in Minecraft. Do you need any help with instruments for your mission?"""
"""That is a great idea, Anna! I will be happy if you will help me with instruments for my mission. Thank you!"""

""" Well done, Steve! Taking on the task of killing cows sounds like an exciting challenge. Do you need any help with instruments for your mission?"""
"""Sure, you can help me with instruments for my mission! I will be happy if you provide some support."""

@pytest.mark.parametrize(
    "tasks",
    create_tasks()
)
@pytest.mark.asyncio
async def test_agent_selection_funcions(log_path, log_level, tasks):
    env = enviroment(tasks, log_path, log_level)
    role = """
    Your are funny, kind of strange, broad-minded guy. You are from Egypt and, despite being just an student, you know English language at a highest level and make your sentences sharp and beutiful.
    You have strong opinions for some questions and usually, give your words as much meaning as you could. Your hobbies are food, history (can tell many facts about today Egyps culture), news (you have facts about many events). Here are some example of what he can say if he was choosing a minecraft task:
    "do you understand the idea of taking this task?", "A task? What?", "What? What i should do with that?"
    """
    agent_entity = agent(env, role, log_path, log_level, name="Steve")
    await agent_entity.select_task()
    
@pytest.mark.asyncio
async def test_agent_think_funcion(log_path, log_level):
    tasks = [{"Item_data": ["lapis lazurite", 40], "Type": "Obtaining", "Action": "Mine 40 lapis lazuli ore with stone pickaxe to get 40 lapis lazuli", "Instruments": ["stone pickaxe"]}, {"Item_data": ["leather", 5], "Type": "Obtaining", "Action": "Kill cow to get 5 leather", "Instruments": ["Stone sword"]}, {"Item_data": ["flint", 10], "Type": "Obtaining", "Action": "Mine 10 gravel with a shovel to get 10 flint", "Instruments": ["a shovel"]}, {"Item_data": ["obsidian", 4], "Type": "Obtaining", "Action": "Mine 4 obsidian with diamond pickaxe to get 4 obsidian", "Instruments": ["diamond pickaxe"]}, {"Item_data": ["apple", 4], "Type": "Obtaining", "Action": "Mine 4 oak and dark oak leaves with shears to get 4 apple", "Instruments": ["shears"]}, {"Item_data": ["feather", 10], "Type": "Obtaining", "Action": "Kill adult chickens to get 10 feather", "Instruments": ["Stone sword"]}, {"Item_data": ["diamond", 12], "Type": "Obtaining", "Action": "Mine 12 diamond ore with iron pickaxe to get 12 diamond", "Instruments": ["iron pickaxe"]}, {"Item_data": ["string", 3], "Type": "Obtaining", "Action": "Kill spiders and cave spiders to get 3 string", "Instruments": ["Stone sword"]}, {"Item_data": ["iron ore", 11], "Type": "Obtaining", "Action": "Mine 11 stone with stone pickaxe to get 11 iron ore", "Instruments": ["stone pickaxe"]}]
    env = enviroment(tasks, log_path, log_level)
    env.take_task(2, "Anna")
    
    role = ""
    agent_entity = agent(env, role, log_path, log_level, "Nikita")
    test_input = """Previous actions:
Anna (Speaker): take_task(2, "Kill cow")
Select your next action:
def comment() - Give your opinion about the speaker decision
def request_join() - Ask the speaker, if you can join him making the task together
def skip() - Gives another collegue, who didn't take his action, opportunity to speak
def request_take_task() - If you disagree and don't support speaker decision, you can ask him to take the mission
def request_take_instruments() - Help the speaker with instruments for his mission
    """      
    
    await agent_entity.think("Anna", test_input)