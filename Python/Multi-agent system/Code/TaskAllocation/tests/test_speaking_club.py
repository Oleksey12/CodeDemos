from Project.TaskAllocation.code.speaking_club_manager import SpeekingClubManager
from Project.TaskAllocation.code.action_enviroment import TaskAllocationEnvironment
from Project.TaskAllocation.code.agent import Agent
from Project.MissionDecomposition.code.openai_clients import ChatClient
from Project.MissionDecomposition.code.api_keys import openai_key

import pytest
import logging
import os


@pytest.fixture
def chat_logs():
    return os.path.join("logs", "chat.log")   

@pytest.fixture
def test_logs():
    return os.path.join("logs", "tests.log")   

@pytest.fixture
def process_logs():
    return os.path.join("logs", "decomposition.log")    
    
@pytest.fixture
def log_level():
    return logging.DEBUG

def speaking_club_generator(agents: list[Agent.__annotations__], log_path, log_level, max_iterations=1):
    return SpeekingClubManager(agents, max_iterations=max_iterations, log_file_path=log_path, log_level=log_level)

def agents_generator(enviroment: TaskAllocationEnvironment.__annotations__, test_logs, process_logs, log_level) -> list[Agent.__annotations__]:
    chat_client = ChatClient(openai_key, temprature=0.35, log_file_path=test_logs, log_level=log_level)
    # chat_client = ChatClient(openai_key, log_file_path=test_logs, log_level=log_level)
    code_client = ChatClient(openai_key, log_file_path=test_logs, log_level=log_level)
    
    files = ["Anna.txt", "Steve.txt", "Nikita.txt"]
    names = ["Anna", "Steve", "Nikita"]
    
    roles = [role_reciever(file) for file in files]
    
    return [Agent(name, chat_client, code_client, enviroment, role, log_file_path=process_logs, log_level=log_level) for name, role in zip(names, roles)] 
       
def enviroment(tasks: list[dict], log_path, log_level) -> TaskAllocationEnvironment.__annotations__:
    return TaskAllocationEnvironment(tasks, ["Anna", "Nikita", "Steve"])   
    
    
@pytest.mark.asyncio
async def test_one_iteration(chat_logs, test_logs, process_logs, log_level) -> None: 
    input_tasks = [{"Item_data": ["string", 3], "Type": "Obtaining", "Action": "Kill spiders and cave spiders to get 3 string", "Instruments": ["Stone sword"]}, 
                   {"Item_data": ["iron ore", 11], "Type": "Obtaining", "Action": "Mine 11 iron ore with stone pickaxe to get 11 iron ore", "Instruments": ["stone pickaxe"]}, 
                   {"Item_data": ["sugar cane", 15], "Type": "Obtaining", "Action": "Mine 15 sugar cane with any tool to get 15 sugar cane", "Instruments": ["any tool"]}, 
                   {"Item_data": ["gold ore, nether gold ore, or deepslate gold ore", 32], "Type": "Obtaining", "Action": "Mine 32 gold ore with iron pickaxe to get 32 gold ore\nmine nether gold ore with iron pickaxe to get 32 nether gold ore\nmine deepslate gold ore with iron pickaxe to get 32 deepslate gold ore", "Instruments": ["iron pickaxe"]}, 
                   {"Item_data": ["matching log", 2], "Type": "Obtaining", "Action": "Mine 2 stripped log with any axe to get 2 matching log", "Instruments": ["any axe"]}]

    env = enviroment(input_tasks, process_logs, log_level)
    agents = agents_generator(env, test_logs, process_logs, log_level)
    club_manager = speaking_club_generator(agents, chat_logs, log_level)    
    
    try:
        await club_manager.run_round()
    except Exception as exc:
        assert False, exc
    
    
@pytest.mark.asyncio
async def test_round_iteration(chat_logs, test_logs, process_logs, log_level) -> None: 
    input_tasks = [{"Item_data": ["string", 3], "Type": "Obtaining", "Action": "Kill spiders and cave spiders to get 3 string", "Instruments": ["Stone sword"]}, 
                   {"Item_data": ["iron ore", 11], "Type": "Obtaining", "Action": "Mine 11 iron ore with stone pickaxe to get 11 iron ore", "Instruments": ["stone pickaxe"]}, 
                   {"Item_data": ["sugar cane", 15], "Type": "Obtaining", "Action": "Mine 15 sugar cane with any tool to get 15 sugar cane", "Instruments": ["any tool"]}, 
                   {"Item_data": ["gold ore, nether gold ore, or deepslate gold ore", 32], "Type": "Obtaining", "Action": "Mine 32 gold ore with iron pickaxe to get 32 gold ore\nmine nether gold ore with iron pickaxe to get 32 nether gold ore\nmine deepslate gold ore with iron pickaxe to get 32 deepslate gold ore", "Instruments": ["iron pickaxe"]}, 
                   {"Item_data": ["matching log", 2], "Type": "Obtaining", "Action": "Mine 2 stripped log with any axe to get 2 matching log", "Instruments": ["any axe"]}]

    env = enviroment(input_tasks, process_logs, log_level)
    agents = agents_generator(env, test_logs, process_logs, log_level)
    club_manager = speaking_club_generator(agents, chat_logs, log_level, len(agents))    
    
    try:
        await club_manager.run_round()
    except Exception as exc:
        assert False, exc



@pytest.mark.asyncio
async def test_simple_task_allocation(chat_logs, test_logs, process_logs, log_level) -> None: 
    input_tasks = [{"Item_data": ["string", 3], "Type": "Obtaining", "Action": "Kill spiders and cave spiders to get 3 string", "Instruments": ["Stone sword"]}, 
                   {"Item_data": ["iron ore", 11], "Type": "Obtaining", "Action": "Mine 11 iron ore with stone pickaxe to get 11 iron ore", "Instruments": ["stone pickaxe"]}, 
                   {"Item_data": ["gold ore, nether gold ore, or deepslate gold ore", 32], "Type": "Obtaining", "Action": "Mine 32 gold ore with iron pickaxe to get 32 gold ore\nmine nether gold ore with iron pickaxe to get 32 nether gold ore\nmine deepslate gold ore with iron pickaxe to get 32 deepslate gold ore", "Instruments": ["iron pickaxe"]}]

    env = enviroment(input_tasks, process_logs, log_level)
    agents = agents_generator(env, test_logs, process_logs, log_level)
    club_manager = speaking_club_generator(agents, chat_logs, log_level, len(agents)-1)    
    
    try:
        await club_manager.run_allocation()
    except Exception as exc:
        assert False, exc
        
        
@pytest.mark.asyncio
async def test_task_allocation(chat_logs, test_logs, process_logs, log_level) -> None: 
    input_tasks = [{"Item_data": ["string", 3], "Type": "Obtaining", "Action": "Kill spiders and cave spiders to get 3 string", "Instruments": ["Stone sword"]}, 
                   {"Item_data": ["iron ore", 11], "Type": "Obtaining", "Action": "Mine 11 iron ore with stone pickaxe to get 11 iron ore", "Instruments": ["stone pickaxe"]}, 
                   {"Item_data": ["sugar cane", 15], "Type": "Obtaining", "Action": "Mine 15 sugar cane with any tool to get 15 sugar cane", "Instruments": ["any tool"]}, 
                   {"Item_data": ["gold ore, nether gold ore, or deepslate gold ore", 32], "Type": "Obtaining", "Action": "Mine 32 gold ore with iron pickaxe to get 32 gold ore\nmine nether gold ore with iron pickaxe to get 32 nether gold ore\nmine deepslate gold ore with iron pickaxe to get 32 deepslate gold ore", "Instruments": ["iron pickaxe"]}, 
                   {"Item_data": ["matching log", 2], "Type": "Obtaining", "Action": "Mine 2 stripped log with any axe to get 2 matching log", "Instruments": ["any axe"]}]

    env = enviroment(input_tasks, process_logs, log_level)
    agents = agents_generator(env, test_logs, process_logs, log_level)
    club_manager = speaking_club_generator(agents, chat_logs, log_level, len(agents)-1)    
    
    try:
        await club_manager.run_allocation()
    except Exception as exc:
        assert False, exc





def role_reciever(role_name: str) -> str:
    file_dir = os.path.dirname(__file__)
    previous_dir = os.path.join(file_dir, os.pardir)
    roles_dir = os.path.join(previous_dir, "roles")

    with open(os.path.join(roles_dir, role_name), 'r') as f:
        text = f.read()
    return text
    
    