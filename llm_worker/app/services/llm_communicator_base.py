from abc import ABC, abstractmethod

from app.business.npa import Npa


class LlmCommunicatorBase(ABC):
    def __init__(self, api_key: str):
        if api_key is None:
            raise Exception("Api key cannot be empty")
        
        self.api_key = api_key
        
    @abstractmethod
    def ask(tz_text: str, npa: Npa):
        pass