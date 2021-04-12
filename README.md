Discord bot written in python using discord.py library, used in a private server.

Docker hub link: https://hub.docker.com/r/seker212/discord_bot

## Running in docker
### How to install
1. Pull image from docker hub
```
docker pull seker212/discord_bot
```
2. Run the image using
```
docker run --rm -d -e "TOKEN=<discord app client secret here>" --name <name> seker212/discord_bot:latest
```
If neccesary docker container can be run with a folder sharded with operating system by to run command parameter
```
-v <os path>:/app/audio
```


### How to build docker image
```
docker build -t seker212/discord_bot:latest .
```

## Running normally
### How to install
1. Clone repository from github
```
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
```
python -m pip install -r requirements.txt
```

## How to run
To run your virtual enviroment must be enabled and folder should contain a file named `token.txt` with the discord secret token. To run use command
```
python start_bot.py
```
Token can be also a enviroment variable or passed directly in command
```
python start_bot.py <discord token here>
```