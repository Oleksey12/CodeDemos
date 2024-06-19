from Project.MissionDecomposition.code.custom_loggers import create_logger
from openai import AsyncOpenAI
from openai import OpenAI
import voyager.utils as U
import asyncio
import logging
import atexit
import os


class EmbeddingClient():
    def __init__(self,  
        api_key,
        model="text-embedding-3-small",
        log_file_path = os.path.join("logs", "main.log"),
        log_level = logging.INFO) -> None:

        self._calls = 0
        U.f_mkdir(os.path.dirname(log_file_path))
        self._logger = create_logger(logging.getLogger(f"{__name__}.EmbeddingClient"), 
                                                   log_file_path, 
                                                   log_level)
        
        atexit.register(self._log_on_exit, self._logger, lambda: self._calls)
        
        self._client = AsyncOpenAI(api_key=api_key)
        self._model = model
    
    def _log_on_exit(self, logger: logging.Logger.__annotations__, call_count: callable): 
        logger.info(f"Суммарное количество запросов к OpenAI API: {call_count()}")
    
    def _increase_call_count(self):
        self._calls += 1
        
    async def generate_embeddings(self, text: str, timeout=25) -> list[float]:
        self._increase_call_count()
        response = await asyncio.wait_for(self._client.embeddings.create(
            model=self._model,
            input = [text]
        ), timeout)
        return response.data[0].embedding


class ChatClient():
    calls = 0
    def __init__(
        self,
        api_key,
        model="gpt-3.5-turbo-0125",
        temprature=0,
        request_timeout=120,
        log_file_path = os.path.join("logs", "main.log"),
        log_level = logging.INFO) -> None:

        self._calls = 0
        self._logger = create_logger(logging.getLogger(f"{__name__}.ChatClient"), 
                                                   log_file_path, 
                                                   log_level)
        U.f_mkdir(os.path.dirname(log_file_path))
        atexit.register(self._log_on_exit, self._logger, lambda: self._calls)

        self._model = model
        self._temperature = temprature
        self._async_llm = AsyncOpenAI(
            api_key=api_key,
            timeout=request_timeout
        )
        self._sync_llm = OpenAI(
            api_key=api_key,
            timeout=request_timeout
        )

    def _log_on_exit(self, logger: logging.Logger.__annotations__, call_count: callable): 
        logger.info(f"Суммарное количество запросов к OpenAI API: {call_count()}")

    def _increase_call_count(self):
        self._calls += 1
        
    def chat_llm_sync(self, system_prompt: str, human_prompt: str, assistant_prompt: str=None) -> str:
        self._increase_call_count()
        messages = [{"role" : "system", "content": system_prompt}, {"role" : "user", "content": human_prompt}]
        if assistant_prompt:
            messages.append({"role": "assistant", "content": assistant_prompt})
        
        response = self._sync_llm.chat.completions.create(
            model=self._model,
            messages=messages,
            temperature=self._temperature
        )

        return response.choices[0].message.content
    
    async def chat_llm(self, system_prompt: str, human_prompt: str, assistant_prompt: str=None, timeout=30) -> str:
        self._increase_call_count()
        messages = [{"role" : "system", "content": system_prompt}, {"role" : "user", "content": human_prompt}]
        if assistant_prompt:
            messages.append({"role": "assistant", "content": assistant_prompt})
            
        response = await asyncio.wait_for(self._async_llm.chat.completions.create(
            model=self._model,
            messages=messages,
            temperature=self._temperature
        ), timeout)

        return response.choices[0].message.content
