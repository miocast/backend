from app.business.base_schema import BaseSchema

class Npa(BaseSchema):
    npas: list[str]
    sources: list[str]
    distances: list[float]