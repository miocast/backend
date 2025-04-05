from app.business.base_schema import BaseSchema
from app.business.npa import Npa


class LlmRelevants(BaseSchema):
    analys: object
    npa: Npa