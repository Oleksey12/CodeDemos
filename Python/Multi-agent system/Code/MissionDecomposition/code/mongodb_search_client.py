from Project.MissionDecomposition.code.openai_clients import EmbeddingClient
from Project.MissionDecomposition.code.custom_loggers import create_logger
import voyager.utils as U
import pymongo
import logging
import atexit
import os
import re



class MongoDBCollectionSearch:
    def __init__ (self,
        openai_embedding_client: EmbeddingClient.__annotations__,
        mongodb_auth_string: str,
        db_name: str,
        db_collection: str,
        db_path: str = "embeding",
        db_index: str = "default",
        log_file_path = os.path.join("logs", "main.log"),
        log_level = logging.INFO
    ):
        
        U.f_mkdir(os.path.dirname(log_file_path))
        self._logger = create_logger(logging.getLogger(f"{__name__}.MongoDBCollectionSearch"), 
                                                   log_file_path, 
                                                   log_level)
        
        atexit.register(self.log_on_exit, self._logger, lambda: self._calls)
        
        self._calls = 0
        self._openai_embedding = openai_embedding_client
        self._collection = pymongo.MongoClient(mongodb_auth_string)[db_name][db_collection]
        self._db_path = db_path
        self._db_index = db_index
    
    def increase_call_count(self):
        self._calls += 1
        
    def log_on_exit(self, logger: logging.Logger.__annotations__, get_count_function: callable): 
        logger.info(f"Суммарное количество обращений к базе данных: {get_count_function()}")
    
    async def smart_search(self, text: str) -> str:
        result = ""
        try: 
            result = self.regex_search(text)
        except StopIteration:
            result = await self.vector_search(text, 2, self._db_path, self._db_index)
            result = result[0]
            self._logger.warning(f"Не удалось найти статью по названию. Использовано статья для {result['Name']} вместо {text}")
        
        return result
    
    def regex_search(self, text) -> dict:
        self.increase_call_count()
        elems = re.findall("\w+", text)
        name = elems[0] if len(elems) == 1 else " ".join([x.capitalize() for x in elems]) 
        search_template = re.compile("".join(["^", name, "$"]),re.I)
        search_result = self._collection.find({"Name": search_template})
        return search_result.next()


    def _clearArticle(x):
        x.pop("_id"),
        x.pop("embedding")
        x.pop("description_embedding")
        x.pop("summary")
        return x
    
    async def vector_search(self, text, lim=2, path="embedding", index="default") -> list[dict]:
        self.increase_call_count()
        embeddings =  await self._openai_embedding.generate_embeddings(text)
        results = self._collection.aggregate([
            {"$vectorSearch": {
                    "queryVector": embeddings,
                    "path": path,
                    "numCandidates": 100,
                    "limit": lim,
                    "index": index
                }
            }])
        
            
        return list(results)

    def _format_article_name(self, text) -> str:
        capitalized_array = [item.strip().capitalize() for item in text.strip().split(' ')]
        capitilized_text = " ".join(capitalized_array)
        return capitilized_text

    def primitive_search_by_name(self, text, lim, format_func: callable = _format_article_name) -> list:
        self.increase_call_count()
        text = format_func(text)
        result = self._collection.find({"Name": text}).limit(lim)
        res = [item for item in result]
        return res

    def _extract_sections(self, articles : list[dict], sections: list) -> list[dict]:
        res = []
        for article in articles:
            extracted_dict = {}
            for key, value in article.items():
                if (key in sections):
                    extracted_dict[key] = value
            res.append(extracted_dict)
        return res
