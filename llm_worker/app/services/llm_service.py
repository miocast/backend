from itertools import count
from json import load
import json
import os
import chromadb
from chromadb.utils import embedding_functions
from io import BytesIO
from docx import Document
from odf import text, teletype
from odf.opendocument import load
from fastapi import  UploadFile
import requests

from app.business.document import DocumentModel
from app.business.llm_relevants import LlmRelevants
from app.business.npa import Npa
from app.config.settings import LLM_TOKEN, MAIN_API
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
        self.llm_communicator: LlmCommunicatorBase = MistralCommunicator(api_key=LLM_TOKEN)

    def save_docs_to_vector_db(self, documents: list[DocumentModel]):
        metadatas = []
        documents_to_vector = []
        ids = []
        
        for doc in documents:
            metadatas.append({"source": doc.file_path, "type": doc.type})
            documents_to_vector.append(doc.text)
            ids.append(doc.id)
            
        self.collection = self.collection.add(documents=documents_to_vector, metadatas=metadatas, ids=ids)
        
    async def check_tz(self, bytes, file_name, document_id: int):
        content_text = self._read_file(bytes, os.path.splitext(file_name)[1])
        
        relevants = self._find_relevants(content_text)
        
        if relevants == None:
            return relevants 
        
        dik_pik_res = self.llm_communicator.ask(content_text, relevants)
        
        self._send_analys_to_web(LlmRelevants(analys=dik_pik_res, npa=relevants, documentId=document_id))
    
    def _find_relevants(self, text: str, top_counter: int = 5) -> Npa | None:
        filtered = self.collection.query(query_texts=[text], n_results=top_counter, include=["documents", "metadatas", "distances"])
        filtered = self._filter_chroma_results(filtered)
        print(filtered["distances"])
        if filtered["distances"] == [[]]:
            return None
        
        return Npa(npas=filtered["documents"][0], sources=[m["source"] for m in filtered["metadatas"][0]], distances=filtered["distances"][0])
    
    def _read_file(self, file_contents: bytes, file_ext: str) -> str:
        if file_ext == ".odt":
            return self._read_odt(file_contents)
        elif file_ext == ".doc" or file_ext == ".docx":
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
    
    def _filter_chroma_results(self, results, max_distance=0.85):
        return {
            "documents": [[doc for i, doc in enumerate(results["documents"][0]) 
                        if results["distances"][0][i] <= max_distance]],
            "metadatas": [[meta for i, meta in enumerate(results["metadatas"][0]) 
                        if results["distances"][0][i] <= max_distance]],
            "distances": [[dist for dist in results["distances"][0] 
                        if dist <= max_distance]],
            "ids": [[id_ for i, id_ in enumerate(results["ids"][0]) 
                    if results["distances"][0][i] <= max_distance]]
        }
        
    def _send_analys_to_web(self, analys: LlmRelevants):
        dict = {}
        
        analys_list = []
        
        for i in analys.analys:
            item = {
                "id": i["id"],
                "text": i["text"],
                "explanation": i["explanation"],
                "regulation": i["regulation"]
            }
            analys_list.append(item)
            
        dict = {
            "analyses": analys_list,
            "npa": {
                "npas": analys.npa.npas,
                "sources": analys.npa.sources,
                "distances": analys.npa.distances
            },
            "documentId": analys.documentId
        }
        
        url = MAIN_API + "/api/v1/TechnicalSpecification/process-from-worker"    
        try:
            headers = {
    "Content-Type": "application/json"}
            response = requests.post(url, data=json.dumps(dict), headers=headers)
        except Exception as e:
            print(e)
