from typing import Any
from app.business.base_schema import BaseSchema
from app.business.npa import Npa

class LlmRelevants(BaseSchema):
    analys: Any
    npa: Npa
    documentId: str