FROM tracktennis/ffmpeg-python38:v4.2.2
WORKDIR /app
COPY requirements.txt .
RUN apk update && \
        apk add gcc && \
        apk add libc-dev && \
        apk add libxml2-dev && \
        apk add libxslt-dev && \
        apk add libffi-dev && \
        apk add make && \
        apk add opus-dev && \
        apk add opus
RUN python -m pip install -r requirements.txt
COPY core.py .
COPY stuff.py .
COPY tables.py .
COPY /audio/ ./audio
COPY start_bot.py .
CMD [ "python", "start_bot.py" ]