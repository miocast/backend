import json
from app.business.npa import Npa
from app.services.llm_communicator_base import LlmCommunicatorBase
import requests
from mistralai import Mistral

class MistralCommunicator(LlmCommunicatorBase):
    def __init__(self, api_key: str):
        self.client = Mistral(api_key=api_key)
        super().__init__(api_key)

    def ask(self, tz_text: str, npa: Npa):
        query = f"""
                    Ты опытный юрист и аналитик. Проанализируй текст ниже на соответствие Нормативно-Правовым Актам РФ (они прикреплены ниже). Отвечай в формате JSON. Вот пример ответа: 
            ```
            # Текст
            Пример технического задания для новой системы управления данными клиентов с аутентификацией пользователей, шифрованием данных и облачным хранилищем. Система будет обрабатывать персональные данные, включая имена, адреса и платежные реквизиты.

            # НПА
            //Здесь будет текст НПА
            ```
            Вот такой ответ ты должен вернуть (пиши все в одну строчку):
            [
            {{
                id: "1",
                text: "персональные данные",
                explanation:
                'Упоминание обработки персональных данных требует соответствия ФЗ-152 "О персональных данных"',
                regulation: 'ФЗ-152 "О персональных данных"',
            }},
            {{
                id: "2",
                text: "шифрованием данных",
                explanation: "Использование шифрования требует соответствия Приказу ФСБ России № 378",
                regulation: "Приказ ФСБ России № 378",
            }},
            ]

          
        """
        
        user_query = f"""
          Вот твои данные:
            # Текст 
            {tz_text}

            # НПА
            {npa.npas[0]}
        """
        
        response = self.client.chat.complete(
            model="mistral-large-latest",
            messages=[
                {"role": "system", "content": query},
                {"role": "user", "content": user_query},
            ],
            response_format= {
                "type": "json_object"
            }
        )
        
        
        return json.loads(response.choices[0].message.content.replace("\n", ""))
