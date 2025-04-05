from json import load
import os
import chromadb
from chromadb.utils import embedding_functions
from io import BytesIO
from docx import Document
from odf import text, teletype
from odf.opendocument import load
from fastapi import  UploadFile

from app.business.document import DocumentModel
from app.business.npa import Npa
from app.services.mistral_communicator import MistralCommunicator
from app.services.llm_communicator_base import LlmCommunicatorBase


class LlmService:
    def __init__(self):
        self.vector_client = chromadb.PersistentClient(path="npa_chroma_db")
        self.embed_fn = embedding_functions.SentenceTransformerEmbeddingFunction("all-mpnet-base-v2")
        self.collection = self.vector_client.get_or_create_collection(
            name="npa_collection", 
            embedding_function=self.embed_fn
        )
        # TODO: TO config
        self.llm_communicator: LlmCommunicatorBase = MistralCommunicator(api_key="02hNX3JbMDtRWEQiEoES2XAPFxpIUTxX")

    def save_docs_to_vector_db(self, documents: list[DocumentModel]):
        metadatas = []
        documents_to_vector = []
        ids = []
        
        for doc in documents:
            metadatas.append({"source": doc.file_path, "type": doc.type})
            documents_to_vector.append(doc.text)
            ids.append(doc.id)
            
        self.collection = self.collection.add(documents=documents_to_vector, metadatas=metadatas, ids=ids)
        
    async def check_tz(self, document: UploadFile, user_id: int):
        # TODO: Отправить запрос в векторную бд
        # 2. Отправить дикпику
        
        content = await document.read()

        
        # Декодируем с правильной кодировкой
        # content_text = content.decode(encoding if encoding else "utf-8", errors="ignore")
        content_text = self._read_file(content, os.path.splitext(document.filename))
        
        relevants = self._find_relevants(content_text)
        
        if relevants == None:
            # TODO: Тут парсинг по ключевым словам с дикпика
            print('none')
            return relevants 
        
        print(relevants.npas)
        
        dik_pik_res = self.llm_communicator.ask(content_text, relevants)
        
        print (dik_pik_res)
        
        
        # Если не нашло совпадений. Пытаться спарсить. реализовать в конце
        return dik_pik_res
    
    def _find_relevants(self, text: str, top_counter: int = 5) -> Npa | None:
        filtered = self.collection.query(query_texts=[text], n_results=top_counter, include=["documents", "metadatas", "distances"])
        
        return Npa(npas=filtered["documents"][0], sources=[m["source"] for m in filtered["metadatas"][0]], distances=filtered["distances"][0])
    
    def _read_file(self, file_contents: bytes, file_ext: str) -> str:
        if file_ext == "odt":
            return self._read_docx(file_contents)
        elif file_ext == "doc":
            return self._read_docx(file_contents)
        
        return ""
    
    def _read_docx(self, file_content: bytes) -> str:
        doc = Document(BytesIO(file_content))
        return "\n".join([para.text for para in doc.paragraphs])

    def _read_odt(self, file_content: bytes) -> str:
        """Чтение ODT файла и извлечение текста"""
        doc = load(BytesIO(file_content))
        text_content = []
        for paragraph in doc.getElementsByType(text.P):
            text_content.append(teletype.extractText(paragraph))
        return "\n".join(text_content)
    
    # def _filter_chroma_results(self, results, max_distance=0.85):
    #     return {
    #         "documents": [[doc for i, doc in enumerate(results["documents"][0]) 
    #                     if results["distances"][0][i] <= max_distance]],
    #         "metadatas": [[meta for i, meta in enumerate(results["metadatas"][0]) 
    #                     if results["distances"][0][i] <= max_distance]],
    #         "distances": [[dist for dist in results["distances"][0] 
    #                     if dist <= max_distance]],
    #         "ids": [[id_ for i, id_ in enumerate(results["ids"][0]) 
    #                 if results["distances"][0][i] <= max_distance]]
    #     }
