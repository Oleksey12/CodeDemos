from Project.MissionDecomposition.code.openai_clients import EmbeddingClient, ChatClient
from Project.MissionDecomposition.code.mongodb_search_client import MongoDBCollectionSearch
from Project.MissionDecomposition.code.api_keys import openai_key, mongodb_key 
from Project.MissionDecomposition.code.item_analyzer import ItemAnalyzer
import logging
import pytest
import os

@pytest.fixture(scope="session")
def analyzerModule() -> ItemAnalyzer.__annotations__:
    test_dir = os.path.join("logs", "tests.log")    
    
    embedding_client = EmbeddingClient(openai_key, log_file_path=test_dir, log_level=logging.DEBUG)
    chat_client = ChatClient(openai_key, temprature=0, log_file_path=test_dir, log_level=logging.DEBUG)
    mongoDB_client = MongoDBCollectionSearch(embedding_client, mongodb_key, "MinecraftHelper", "Items", log_file_path=test_dir, log_level=logging.DEBUG)
    return ItemAnalyzer(chat_client, mongoDB_client, use_cache=False, log_file_path=test_dir, log_level=logging.DEBUG)

def hardest_items_testcase() -> list[str]:
    items = ["Nether star",
             "Music disc", 
             "Dragon egg", 
             "Sea lantern", 
             "Ender chest", 
             "Sponge", 
             "Ender pearl", 
             "Beacon", 
             "Diamond Pickaxe", 
             "Golden Hoe"]
    
    sentences = ["Kill wither to get nether star", 
                 "Craft from 9 disc fragment to get 1 music disc", 
                 "Kill ender dragon to get dragon egg", 
                 "Craft from 4 prismarine shard, 5 prismarine crystal to get 1 sea lantern",
                 "Craft from 8 obsidian, 1 eye of ender to get 1 ender chest",
                 "Mine sponge with any tool to get 1 sponge",
                 "Kill enderman to get ender pearl",
                 "Craft from 3 obsidian, 5 glass, 1 nether star to get 1 beacon",
                 "Craft from 3 diamond, 2 stick to get 1 diamond pickaxe",
                 "Craft from 2 gold ingot, 2 stick to get 1 golden hoe"]
    
    return [(i, s) for i,s in zip(items, sentences)]

    
# @pytest.mark.parametrize(
#     "input,expected",
#     hardest_items_testcase()
# )
# def test_hardest_items_without_RAG(analyzerModule: analyzer.ItemAnalyzer.__annotations__, input, expected):
#     assert analyzerModule.get_crafting_guide(input, False).lower() == expected.lower()


@pytest.mark.asyncio
async def test_article_extraction(analyzerModule: ItemAnalyzer.__annotations__):
    item_name = "Wooden pickaxe"
    assert_text = "Предмет не был найден, хотя должен был"
    try:
        await analyzerModule.get_item_article(item_name)
        assert True
    except StopIteration:
        assert False, assert_text
        
def test_case_indepentent_extraction(analyzerModule: ItemAnalyzer.__annotations__):
    item_name = "block Of iron"
    assert_text = "Предмет не был найден, хотя должен был"
    try:
        analyzerModule.get_item_article(item_name)
        assert True
    except StopIteration:
        assert False, assert_text
    
@pytest.mark.parametrize(
    "input,expected",
    hardest_items_testcase()
)
@pytest.mark.asyncio
async def test_hardest_items_with_RAG(analyzerModule: ItemAnalyzer.__annotations__, input, expected):
    """Проверка работы модуля на самых труднодобваемых предметах игры майнкрафт
        https://www.creeperhost.net/blog/10-of-the-rarest-items-in-minecraft/
    Args:
        analyzerModule (ItemAnalyzer.__annotations__): _description_
        input (_type_): _description_
        expected (_type_): _description_
    """
    res = await analyzerModule.get_item_guide(input, True)
    assert res.lower() == expected.lower()

