from Project.MissionDecomposition.code.openai_clients import EmbeddingClient, ChatClient
from Project.MissionDecomposition.code.mongodb_search_client import MongoDBCollectionSearch
from Project.MissionDecomposition.code.api_keys import openai_key, mongodb_key 
from Project.MissionDecomposition.code.task_specifiers import TaskSpecifier
from Voyager.Diplom.Voyager.Project.MissionDecomposition.code.mission_analyzer import MissionAnalyzer 
from Project.MissionDecomposition.code.item_analyzer import ItemAnalyzer

import logging
import pytest
import os


@pytest.fixture(scope="session")
def analyzerModule() -> MissionAnalyzer.__annotations__:
    test_dir = os.path.join("logs", "tests.log")    
    
    chat_client = ChatClient(openai_key, log_file_path=test_dir, log_level=logging.DEBUG)
    embedding_client = EmbeddingClient(openai_key, log_file_path=test_dir, log_level=logging.DEBUG)
    mongodb_client = MongoDBCollectionSearch(embedding_client, 
                                                mongodb_key, 
                                                "MinecraftHelper",
                                                "Items", 
                                                log_file_path=test_dir,
                                                log_level=logging.DEBUG)

    item_analyzer = ItemAnalyzer(chat_client, mongodb_client, use_cache=False, log_file_path=test_dir, log_level=logging.DEBUG)
    task_specifier = TaskSpecifier(chat_client)
    
    return MissionAnalyzer(item_analyzer, task_specifier, log_file_path=test_dir, log_level=logging.DEBUG)

@pytest.mark.asyncio
async def test_no_error(analyzerModule: MissionAnalyzer.__annotations__):
    input_mission = "Get 10 oak wood"
    assert_test = "Module ended with error={}"
    try:
        await analyzerModule.analyze_mission(input_mission)
    except Exception as exc:
        assert False, assert_test.format(str(exc))


@pytest.mark.asyncio
async def test_smelting_sections(analyzerModule: MissionAnalyzer.__annotations__):
    input_mission = "Smelt 10 iron ore"
    expected_section_list = ["Action", "Type", "Items", "Instruments", "Smelting"]
    res = await analyzerModule.analyze_mission(input_mission)
    assert list(res.keys()) == expected_section_list 

@pytest.mark.asyncio
async def test_killing_sections(analyzerModule: MissionAnalyzer.__annotations__):
    input_mission = "Kill 10 zombie"
    expected_section_list = ["Action", "Type", "Instruments"]
    res = await analyzerModule.analyze_mission(input_mission)
    assert list(res.keys()) == expected_section_list 


@pytest.mark.asyncio
async def test_mining_sections(analyzerModule: MissionAnalyzer.__annotations__):
    input_mission = "Mine 10 stone"
    expected_section_list = ["Action", "Type"]
    res = await analyzerModule.analyze_mission(input_mission)
    assert list(res.keys()) == expected_section_list 


@pytest.mark.asyncio
async def test_obtaining_mining_sections(analyzerModule: MissionAnalyzer.__annotations__):
    input_mission = "Get 10 diamond"
    expected_section_list = ["Item_data", "Type", "Action", "Instruments"]
    res = await analyzerModule.analyze_mission(input_mission)
    assert list(res.keys()) == expected_section_list 


@pytest.mark.asyncio
async def test_obtaining_crafting_sections(analyzerModule: MissionAnalyzer.__annotations__):
    input_mission = "Get 1 beacon"
    expected_section_list = ["Item_data", "Type", "Action", "Instruments", "Items"]
    res = await analyzerModule.analyze_mission(input_mission)
    assert list(res.keys()) == expected_section_list 


@pytest.mark.asyncio
async def test_obtaining_killing_sections(analyzerModule: MissionAnalyzer.__annotations__):
    input_mission = "Get 1 bone"
    expected_section_list = ["Item_data", "Type", "Action", "Instruments"]
    res = await analyzerModule.analyze_mission(input_mission)
    assert list(res.keys()) == expected_section_list 

@pytest.mark.asyncio
async def test_obtaining_smelting_sections(analyzerModule: MissionAnalyzer.__annotations__):
    input_mission = "Get 1 charcoal"
    expected_section_list = ["Item_data", "Type", "Action", "Instruments", "Items", "Smelting"]
    res = await analyzerModule.analyze_mission(input_mission)
    assert list(res.keys()) == expected_section_list 

