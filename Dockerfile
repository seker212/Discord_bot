FROM python:3.9-slim-buster
WORKDIR /app
COPY requirements.txt .
RUN python -m pip install -r requirements.txt
COPY core.py .
COPY funny.py .
COPY tables.py .
COPY /audio/ ./audio
COPY start_bot.py .
CMD [ "python", "start_bot.py" ]