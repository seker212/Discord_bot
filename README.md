# Discord bot

[![Docker image build](https://github.com/seker212/Discord_bot/actions/workflows/container-image-update.yml/badge.svg?branch=master-2.0)](https://github.com/seker212/Discord_bot/actions/workflows/container-image-update.yml)

## Quick start - Running in docker

### Using docker compose

Container can be run using provided docker compose file provided example provided in [docker-compose-template.yaml](https://github.com/seker212/Discord_bot/blob/master-2.0/docker-compose-template.yaml). Just create a copy of the file named `docker-compose.yaml` and swap the `TOKEN` value from `0` to your bot token. If you don't have one yet, just follow the [official discord documentation](https://discord.com/developers/docs/getting-started#step-1-creating-an-app).

You can also use the simplified version from below:
```yaml
version: "3.9"
services:
  DiscordBot:
    image: seker212/discord_bot:2.0
    environment:
      # Set this a discord token generated on discord page
      - TOKEN=0
    volumes: 
      # Where audio files from .sound commands should be placed
      - ./audio:/app/audio

```

To run it simply execute docker compose command, depending on compose engine installed on your system.

```bash
# V1
docker-compose up -d
# V2
docker compose up -d
```

### Using docker command

Pulling and running container could be done using command below, simply place the TOKEN and name, and set the volume path in PWD.

```bash
docker run --rm -d -e "TOKEN=<discord app client secret here>" --name <name> -v <PWD>/audio:/app/audio seker212/discord_bot:2.0
```

## Running locally

### Requirements

Most dependencies will be resolved by default by restoring nuget pakcages. For playing audio on voice channels [ffmpeg](https://www.ffmpeg.org/download.html) needs to be downloaded and added to PATH. For YouTube player the [yt-dlp](https://github.com/yt-dlp/yt-dlp/releases) is also needed to be added.

For the IDE this project is developed using Visual Studio, but it's just a suggestion.
The project has also a support for Seq server, which can be run locally. We recomend using docker image for creating local server.

### Setup and run

Clone this repo with checkout to branch `master-2.0`. The default launch settings can be adjusted by changing [launchSettings.json](https://github.com/seker212/Discord_bot/blob/master-2.0/DiscordBot/Properties/launchSettings.json).

:bangbang: Before running add to the main direcotry file `token.txt` with discord bot token.

After all the setup the `DiscordBot` project should be runnable. 