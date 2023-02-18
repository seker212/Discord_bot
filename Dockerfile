FROM python:3.9-alpine
WORKDIR /app
RUN apk update && \
        apk add gcc && \
        apk add libc-dev && \
        apk add libxml2-dev && \
        apk add libxslt-dev && \
        apk add libffi-dev && \
        apk add make && \
        apk add opus-dev && \
        apk add opus && \
        apk add ffmpeg && \
        apk add espeak && \
        apk add git
COPY requirements.txt .
RUN python -m pip install -r requirements.txt
COPY . .
CMD [ "python", "start_bot.py" ]