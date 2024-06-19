from Project.MissionDecomposition.code.openai_clients import EmbeddingClient, ChatClient
from Project.MissionDecomposition.code.mongodb_search_client import MongoDBCollectionSearch
from Project.MissionDecomposition.code.api_keys import openai_key, mongodb_key 
from Project.MissionDecomposition.code.custom_loggers import create_logger

from voyager.prompts import load_prompt
import voyager.utils as U
import logging
import os


class ItemAnalyzer():
    def __init__(
      self,
      chat_client: ChatClient.__annotations__,
      database_client: MongoDBCollectionSearch.__annotations__,
      use_cache = True,
      ckpt_dir="cache",
      prompt_name: str = "ItemAnalyzer",
      log_file_path = os.path.join("logs", "main.log"),
      log_level = logging.INFO
    ):
        U.f_mkdir(os.path.dirname(log_file_path))
        self._logger = create_logger(logging.getLogger(f"{__name__}.ItemAnalyzer"), 
                                                   log_file_path, 
                                                   log_level)
        
        self._use_cache = use_cache
        if(self._use_cache):
            self._ckpt_dir = ckpt_dir
            U.f_mkdir(os.path.join(f"{ckpt_dir}", "items"))
            self._obtain_methods_file_path = os.path.join(f"{ckpt_dir}", "items", "info.json")
            self._items_dict = self._update_file_data()
        
        
        self._chat_client = chat_client
        self._database_client = database_client
        self._system_prompt = load_prompt(prompt_name)
        self._message_template = "How i can obtain {item}?"

    def _update_file_data(self):
        res = U.load_json(self._obtain_methods_file_path)
        return res if res else {}
        
    async def get_item_guide(self, item_name: str, use_RAG: bool = True) -> str:
        if (self._use_cache):
            self._items_dict = self._update_file_data()
            if item_name in self._items_dict:
                self._logger.debug(f"Использован кэшированный ответ для {item_name}: \n {self._items_dict[item_name]}")
                return self._items_dict[item_name]
        
        article = await self.get_item_article(item_name)
        RAG_data = self.json_data_to_text(article)
        self._logger.debug(f"Дополнительная информация о получении {item_name}: \n {RAG_data}")
        
        human_message = self._message_template.format(item=item_name.capitalize())
        system_message = self._system_prompt.format(context=(RAG_data if (use_RAG) else ""))
        
        res = await self._chat_client.chat_llm(system_message, human_message)
        res = res.capitalize()
        self._logger.debug(f"Предложенный БЯМ метод получения предмета {item_name}: \n {res}")
        
        if self._use_cache:
            self._items_dict[item_name] = res
            U.json_dump(self._items_dict, self._obtain_methods_file_path)
            
        return res
        
    async def get_item_article(self, article_name: str) -> str:
        article = await self._database_client.smart_search(article_name) 
        return self._database_client._extract_sections([article], ["description", "Obtaining"])
        
    def json_data_to_text(self, json_article: list | dict) -> str:
        res = ""
        
        if (isinstance(json_article, list)):
            for i, item in enumerate(json_article):
                res += f"{i + 1}) " + self.json_data_to_text(item)
        elif (isinstance(json_article, dict)):
            for entry in json_article.items():
                section_name = entry[0].capitalize() + (": " if isinstance(entry[1], str) else ":\n")
                res += section_name + self.json_data_to_text(entry[1])
        elif (isinstance(json_article, str)):
            res += json_article + "\n"
        
        return res

if __name__ == "__main__":
    embedding_client = EmbeddingClient(openai_key)
    mongoDB_client = MongoDBCollectionSearch(embedding_client, mongodb_key, "MinecraftHelper", "Items")
    chat_client = ChatClient(openai_key, temprature=0)
    
    analyzer = ItemAnalyzer(chat_client, mongoDB_client, "ItemAnalyzer")    
    
    print(analyzer.get_item_guide("Emerald ore", True))




