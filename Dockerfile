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
        apk add ffmpeg
COPY . .
RUN python -m pip install -r requirements.txt
CMD [ "python", "start_bot.py" ]