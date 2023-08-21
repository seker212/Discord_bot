# Discord bot

[![Docker image build](https://github.com/seker212/Discord_bot/actions/workflows/container-image-update.yml/badge.svg?branch=master-2.0)](https://github.com/seker212/Discord_bot/actions/workflows/container-image-update.yml)

## Table of contents

1. [Running bot](#tc1)
    1. [Important information](#tc11)
    2. [Doker run](#tc13)
        1. [Enviroment information](#tc131)
        2. [Using docker cli](#tc132)
        3. [Using docker compose](#tc133)
    3. [Local run](#tc12)
2. [Debug and development](#tc2)
    1. [Database schemas](#tc21)

## Running bot <a id="tc1"></a>

This part is about how to run bot depending on enviroment used.

### Imporatnt informations <a id="tc11"></a>

Before trying to run you need to aquire token from [Discord](https://discord.com/developers/applications).

Project supports use of [`Seq`](https://datalust.co/seq) server which can be run locally or remotly for storing logs.

### Docker run <a id="tc13"></a>

Here are the steps to use a deployable version of DiscordBot. Default Docker Hub image can be found [here](https://hub.docker.com/r/seker212/discord_bot)

#### Enviroment informations <a id="tc131"></a>

Running in docker requires setting important environmental variables as well as some volumes for files that should be accessible after shutdown of container.

Variables marked with `Seq server` are referenced to different image for running Seq toghether with DiscordBot more details [here](https://datalust.co/seq)

Enviroment variables:

| Name |Value|Required | Purpose  |
|----------|:-------------:|:----:|----|
| TOKEN |  string (long chain) | X |  Discord bot token can't be run without it. |
| SEQ_URL |  string (url) |  | Seq application url, local or remote |
| SEQ_KEY | string (long chain)|| Key used for connection to seq
| ACCEPT_EULA |  Y or N (single char) |  | `Seq server` container acceptance of eula if run with DiscordBot together|

Volumes:

| Name |      Container Path      | Required | Purpose |
|----------|:-------------:|:----:|-----|
| Audio |  `/app/audio` | X | All sounds files are stored here |
| Data |  `/app/data` | | Database file is stored here with all servers configurations |
| Seq/logs |  `/data` | | `Seq server` container data folder for logs storage if run with DiscordBot together|

#### Using docker cli  <a id="tc132"></a>

Pulling and running container can be done using command below, simply place the TOKEN and name, and set the volume path in PWD.

```bash
docker run --rm -d -e "TOKEN=<discord app client secret here>" --name <name> -v <PWD>/audio:/app/audio seker212/discord_bot:2.0
```

#### Using docker compose <a id="tc133"></a>

Use provided [`docker-compose-template.yaml`](docker-compose-template.yaml) and rename it to `docker-compose.yaml`. Or create your own `docker-compose.yaml` file based on example below and [enviroment variables](#tc131). Replace required values and run by using `docker compose up -d` command for Docker v2 or `docker-compose up -d` for v1.

```yaml

version: "3.9"
services:
  DiscordBot:
    image: seker212/discord_bot:2.0
    environment:
      # Set this a discord token generated on discord page
      - TOKEN=0
    volumes: 
      # Where audio files from sound commands should be placed
      - ./audio:/app/audio

```

### Local run <a id="tc12"></a>

For running locally it is required to install in your system .Net 6.
For managing project Visual Studio is recommended but any editor will do with sufficient knowledge.

Before running the project, [`ffmpeg`](https://www.ffmpeg.org/download.html) should be added to PATH (available from cli) if any music/sound related commands will be run. Additionally [`yt-dlp`](https://github.com/yt-dlp/yt-dlp/releases) also needs to be added for commands connected to YouTube servicer will be used.

Steps:

1. Clone this repo with checkout to branch `master-2.0`.
2. Open `DiscordBot.sln` with Visual studio (or editor of your choice)
3. Place `token.txt` file in main folder with token aqured earlier.
4. Run the `DiscordBot` project

If there are some issues with packages try reimporting NuGet.

## Debug and development <a id="tc2"></a>

Here are stored important informations for debugging and development of bot.

### Database schemas <a id="tc21"></a>

Here are table schemase currently implemented. Detailed short names:

- PK -> Primary key
- FK -> Foreign key
- NN -> Not null
- AI -> Auto increment

`Config` table

| Name | Type | PK | FK | NN | AI |
|------|------|------|------|------|------|
| Id | Integer | X |  | X | X |
| GuildId | Text |   |  |   |   |
| ParameterName | Text |   |  |   |   |
| ParameterValue | Text |   |  |   |   |

`Responses` table

| Name | Type | PK | FK | NN | AI |
|------|------|------|------|------|------|
| Id | Integer | X |  | X | X |
| GuildId | Text |   |  |   |   |
| Response | Text |   |  |   |   |

`Facts` table

| Name | Type | PK | FK | NN | AI |
|------|------|------|------|------|------|
| Id | Integer | X |  | X | X |
| GuildId | Text |   |  |   |   |
| Fact | Text |   |  |   |   |
