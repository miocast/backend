from app.business.base_schema import BaseSchema

class DocumentModel(BaseSchema):
    id: str
    name: str
    type: str
    file_path: str
    text:str
    
    def to_dict(self):
        return {
            "id": self.id,
            "name": self.name,
            "type": self.type,
            "file_path": self.file_path,
            "text": self.text
        }