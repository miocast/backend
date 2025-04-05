from app.business.npa import Npa
from app.services.llm_communicator_base import LlmCommunicatorBase
import requests
from mistralai import Mistral

class MistralCommunicator(LlmCommunicatorBase):
    def __init__(self, api_key: str):
        self.client = Mistral(api_key=api_key)
        super().__init__(api_key)

    def ask(self, tz_text: str, npa: Npa):
        
        # prompt = f"""
        # Проанализируй текст технического задания, указанного после:
        #         Техническое задание: {tz_text}
        #         ---
        #         Релевантные НПА: {npa.npas}
        #         ---
        #         Укажи, какие пункты НПА применимы к ТЗ. Формат:
        #         1. [Название НПА] (п. X.Y): [Цитата] - [Объяснение].
        #     """
        
        # prompt = f"""
        # "Формат общения JSON. Задача: 1. Проанализировать текст технического задания, указанного в поле `tz`. 
        #     2. Проанализировать тексты нормативно правовых актов, указанных в виде массива в поле `npas`. Найди нарушения в ТЗ, согласно нормативно правовым актам и отдай в формате: {{'field': 'error'}}.
        #     Данные, которые ты принимаешь: \n\n```json\n{{"tz": {tz_text}, "npas": {npa.npas}}}\n```\n"
        # """
        
        print(npa.npas)
        
        system_prompt = f"""
Ты — юридический эксперт, который анализирует Техническое Задание (ТЗ) на соответствие нормативно-правовым актам (НПА).  

### Формат входных данных (JSON):  
```json
{{
  "tz": "текст технического задания",
  "npas": "текст НПА 1, текст НПА 2"
}}
```

# Форма выходных данных (JSON):
```json
{{
    "quote": "цитата",
    "error": "В соответствии с законом 159 данный текст противоречит в таких то моментах"
}}
```
        """
        
        user_prompts = f"""
            Проанализируй в соответствии с нормативно правовыми актами следующее ТЗ в формате json
            ### Данные (JSON):
            ```json
            {
                {
                    "tz": tz_text,
                    "npas": ", ".join(npa.npas)
                },
            }
            ```
        """
        
        print(user_prompts)
        
        response = self.client.chat.complete(
            model="mistral-large-latest",
            messages=[
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_prompts},
            ]
        )
        
        print(response)
        
        return response.choices[0].message.content
