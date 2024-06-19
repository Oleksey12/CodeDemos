from Project.MissionDecomposition.code.openai_clients import ChatClient
from Project.MissionDecomposition.code.api_keys import openai_key
from Project.MissionInterpretation.code.Interpretation import InterpretationAgent
import logging
import pytest
import os

@pytest.fixture
def log_path():
    return os.path.join("logs", "tests.log")    
    
@pytest.fixture
def log_level():
    return logging.DEBUG

@pytest.fixture
def interpretation_agent(log_path, log_level):
    openai_client = ChatClient(openai_key, log_file_path=log_path, log_level=log_level)
    agent = InterpretationAgent(openai_client)
    yield agent


@pytest.fixture
def tasks_without_conditons():
    yield {
        "in" : ["Mine diamond", "Obtain a gold block", "Kill 10 skeletons"],
        "out" : [["Mine 1 diamond"], ["Obtain 1 gold block"], ["Kill 10 skeleton"]],
    }
    
@pytest.fixture
def multiple_tasks():
    yield {
        "in" : ["Craft a stone sword and axe", "Mine 10 gold and iron ores"],
        "out" : [["Craft 1 stone sword", "Craft 1 stone axe"], ["Mine 10 gold ore", "Mine 10 iron ore"]],
    }

@pytest.fixture
def wrong_sentences():
    yield {
        "in": ["Say hello", "Walk in a park", "How to make a bread"],
        "out": [['Unknown'], ['Unknown'], ['Unknown']],
    }
    

def test_basic_tasks(interpretation_agent, tasks_without_conditons):
    for i, sent in enumerate(tasks_without_conditons["in"]):
        assert interpretation_agent.format_task(sent) == tasks_without_conditons["out"][i]

def test_exception_tasks(interpretation_agent, wrong_sentences):
    for i, sent in enumerate(wrong_sentences["in"]):
        assert wrong_sentences["out"][i] == interpretation_agent.format_task(sent)

def test_multiple_tasks(interpretation_agent, multiple_tasks):
    for i, sent in enumerate(multiple_tasks["in"]):
        res = interpretation_agent.format_task(sent)
        assert all([multiple_tasks["out"][i][j] == res[j] for j in range(len(res))])
    


