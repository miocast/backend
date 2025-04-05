from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
import uvicorn
from app.api import routers

app = FastAPI(description="Rent app api", version="1", openapi_url='/openapi.json')

# origins = [
#     settings.CLIENT_APP_URL,
# ]

app.add_middleware(
    CORSMiddleware,
    allow_origins = ["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"]
)


for router in routers:
    app.include_router(router, prefix='/api')


if __name__ == '__main__':
    uvicorn.run(
        app=app,
        host='0.0.0.0',
        port=8005,
        use_colors=True
    )
    
