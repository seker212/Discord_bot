using System;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Net;
using Newtonsoft.Json;

namespace DiscordBot
{
    public class Program
    {
        private DiscordSocketClient _client;

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task Client_Ready()
        {
            var guild = _client.GetGuild(823611908623958046);
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName("on");
            guildCommand.WithDescription("Check if Chi-chan's alive!");
            try
            {
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
            }
            catch (ApplicationCommandException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }

        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "on":
                    await command.RespondAsync($"I'm alive! " + Emote.Parse("<:kannakms:830517474537373777>"));
                    break;
                default:
                    await command.RespondAsync($"You executed {command.Data.Name}");
                    break;
            }

        }

        public async Task ClientOnMessageReceived(SocketMessage socketMessage)
        {
            await Task.Run(() =>
            {
                if (!socketMessage.Author.IsBot)
                {
                    Emote oof = Emote.Parse("<:oof:830550882138980392>");
                    var message = socketMessage.Content;

                    var channelId = socketMessage.Channel.Id.ToString();
                    var channel = _client.GetChannel(Convert.ToUInt64(channelId));
                    var socketChannel = (ISocketMessageChannel)channel;

                    string pattern = @"^[^a-zA-Z0-9]*[o]+of[^a-zA-Z0-9]*$";
                    Regex rgx = new Regex(pattern);
                    foreach (Match match in rgx.Matches(message))
                    {
                        socketMessage.AddReactionAsync(oof);
                        Console.WriteLine(socketMessage.Content);

                        socketChannel.SendMessageAsync(message);
                        break;
                    }

                    socketChannel.SendMessageAsync(socketMessage.Content + socketMessage.Author + "asdasd");
                }
            });
        }

        public static Task Main(string[] args) => new Program().MainAsync();
        public async Task MainAsync()
        {
            var _config = new DiscordSocketConfig { MessageCacheSize = 100, GatewayIntents = GatewayIntents.All };
            _client = new DiscordSocketClient(_config);

            await _client.SetGameAsync("WEEEEEEEEEEEEEEEEEEEEEEEEE");

            _client.Log += Log;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;
            _client.MessageReceived += ClientOnMessageReceived;

            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("token.txt"));
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}