FROM tracktennis/ffmpeg-python38:v4.2.2
WORKDIR /app
RUN apk update && \
        apk add gcc && \
        apk add libc-dev && \
        apk add libxml2-dev && \
        apk add libxslt-dev && \
        apk add libffi-dev && \
        apk add make && \
        apk add opus-dev && \
        apk add opus
COPY . .
RUN python -m pip install -r requirements.txt
CMD [ "python", "start_bot.py" ]