from app.business.base_schema import BaseSchema

class DocumentModel(BaseSchema):
    id: str
    name: str
    type: str
    file_path: str
    text:str    