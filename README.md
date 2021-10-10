# Discord bot

Discord bot written in python using discord.py library, used in a private server.

Docker hub link: [here](https://hub.docker.com/r/seker212/discord_bot)

## Running in docker

### How to install

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

### How to build docker image

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

To run your virtual enviroment must be enabled and folder should contain a file named `token.txt` with the discord secret token. Also file `settings.json` should be in the same folder with the values specified below.

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
