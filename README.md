# Discord bot

[![Chi-chan bot Container](https://github.com/seker212/Discord_bot/actions/workflows/Chi-chan_Container.yml/badge.svg?branch=master&event=push)](https://github.com/seker212/Discord_bot/actions/workflows/Chi-chan_Container.yml)

Discord bot written in python using discord.py library, used in a private server.

Docker hub link: [here](https://hub.docker.com/r/seker212/discord_bot)

## Running in docker

### Using docker compose

Container can be run using provided docker-compose-template.yaml. Simply download this file into a machine or copy paste its content to a yaml file on machine.
Change fields depending on template.

```yml
version: "3.9"
services:
  Chi-chan:
    image: seker212/discord_bot
    environment:
    # Set this a discord token generated on discord page
      - TOKEN=0
    volumes: 
      # Where audio files from .sound commands should be placed
      - ./audio:/app/audio
      # Where settings folder should be placed 
      - ./settings:/app/settings
      # Where logs will be stored
      - ./logs:/app/logs
```

Execute this command while in foler with docker-compose.yaml and the container should be up and running.

```bash
docker-compose up -d
```

### Using docker commands

#### How to install

1. Pull image from docker hub

```bash
docker pull seker212/discord_bot
```

2. Run the image using

```bash
docker run --rm -d -e "TOKEN=<discord app client secret here>" --name <name> seker212/discord_bot:latest
```

If neccesary docker container can be run with a folder sharded with operating system by to run command parameter

```bash
-v <os path>:/app/audio
```

#### How to build docker image

```bash
docker build -t seker212/discord_bot:latest .
```

## Running normally

### How to install

1. Clone repository from github

```bash
git clone https://github.com/seker212/Discord_bot.git
```

2. Create and run virtual enviroment

```bash
python3 -m venv env
#Linux version
source env/bin/activate
#Windows version
env\Scripts\activate
```

3. Install neccesary packages

```shell
python -m pip install -r requirements.txt
```

### How to run

To run your virtual enviroment must be enabled and folder `settings/` should contain a file named `token.txt` with the discord secret token. Also file `settings.json` should be in the same folder with the values specified below.

```json
{
    "log_channel_id": <int: channel id on which logs will be printed>,
    "timezone": <string: any valid timezone e.g "Europe/Warsaw">
}
```

To run after creating necessary files use this command

```bash
python start_bot.py
```

Token can be also a enviroment variable or passed directly in command

```bash
python start_bot.py <discord token here>
```
