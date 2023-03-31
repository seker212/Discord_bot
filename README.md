# Discord bot

# !!! Currently developing version 2.0 with rewritting code to C# !!!

[![Chi-chan bot Container](https://github.com/seker212/Discord_bot/actions/workflows/Chi-chan_Container.yml/badge.svg?branch=master&event=push)](https://github.com/seker212/Discord_bot/actions/workflows/Chi-chan_Container.yml)

## Running locally

Clone this repo with checkout to branch `master-2.0`. Run the `DiscordBot` project. :bangbang: Before running remember to add to the main direcotry file `token.txt` with discord bot token.

## Running in docker

### Using docker compose

Container can be run using provided docker compose file provided example provided in `docker-compose-template.yaml`, or from the example below.

```yaml

version: "3.9"
services:
  Chi-chan:
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
docker run --rm -d -e "TOKEN=<discord app client secret here>" --name <name> -v <PWD>/audio:/app/audio chi-chan2 seker212/discord_bot:2.0
```

## Running in local enviroment

### How to clone and install

### How to run