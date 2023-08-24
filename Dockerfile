FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as build
WORKDIR /src
COPY . .
RUN dotnet publish DiscordBot/DiscordBot.csproj -c Release -a x64 -o /app/publish -p:DebugType=None -p:DebugSymbols=false

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine
WORKDIR /app
RUN apk update && \
        apk add libc-dev && \
        apk add libxml2-dev && \
        apk add libxslt-dev && \
        apk add libffi-dev && \
        apk add opus-tools && \
        apk add libsodium && \
        apk add icu-dev && \
        apk add ffmpeg && \
        apk add tzdata && \
        apk add yt-dlp
COPY --from=build /app/publish .
RUN ln -s /usr/lib/libopus.so.*.*.* /app/libopus.so
RUN ln -s /usr/lib/libsodium.so.*.*.* /app/libsodium.so
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV DATABASE_PATH=/app/data/data.db
ENV AUDIO_PATH=/app/audio
ENTRYPOINT [ "dotnet", "DiscordBot.dll" ]
