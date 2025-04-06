import asyncio
import threading
from fastapi import APIRouter, BackgroundTasks, Response, UploadFile

from app.services.act_parser import ActParser
from app.services.llm_service import LlmService

router = APIRouter(tags=['app'])

@router.post('/v1/documents/tz-check')
async def send_doc_to_llm_check(document: UploadFile, document_id, background_tasks: BackgroundTasks):
    llmService = LlmService()
    doc_bytes = await document.read()
    background_tasks.add_task(llmService.check_tz, doc_bytes, document.filename, document_id)
    
    return Response(status_code=200)

@router.post('/v1/documets/parse-by-theme')
def parse_by_theme(theme: str,  background_tasks: BackgroundTasks):
    actParser = ActParser()

    background_tasks.add_task(actParser.parse_by_theme, theme)
    
    return Response(status_code=200)