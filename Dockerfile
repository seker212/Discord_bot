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
        apk add opus && \
        apk add opusfile && \
        apk add libsodium && \
        apk add icu-dev && \
        apk add ffmpeg
COPY --from=build /app/publish .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENTRYPOINT [ "dotnet", "DiscordBot.dll" ]