FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as build
WORKDIR /src
COPY . .
RUN dotnet build DiscordBot/DiscordBot.csproj -c Release -o /app/publish

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
        apk add yt-dlp
COPY --from=build /app/publish .
RUN ln -s /usr/lib/libopus.so.0.8.0 /app/libopus.so
RUN ln -s /usr/lib/libsodium.so.23.3.0 /app/libsodium.so
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENTRYPOINT [ "dotnet", "DiscordBot.dll" ]