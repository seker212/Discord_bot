﻿using Autofac;
using Discord;
using Discord.WebSocket;
using DiscordBot.Commands;
using DiscordBot.Commands.Core;
using DiscordBot.Core;
using DiscordBot.Core.Helpers;
using DiscordBot.Core.Interfaces;
using DiscordBot.Core.Providers;
using DiscordBot.Core.Voice;
using DiscordBot.MessageHandlers;
using DiscordBot.ActivityLogging;
using Serilog;
using Serilog.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using DiscordBot.Commands.Core.Helpers;

namespace DiscordBot
{
    public class Startup
    {
        public IContainer GetAutofacContainer()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

            var builder = new ContainerBuilder();
            builder.Register(_ => new DiscordSocketConfig { MessageCacheSize = 100, GatewayIntents = GatewayIntents.All }).AsSelf().SingleInstance();
            builder.RegisterType<BotTokenProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<BotClientRunner>().AsSelf().SingleInstance();
            builder.RegisterType<DiscordSocketClient>()
                .AsSelf().As<IDiscordClient>().SingleInstance()
                .WithParameter(
                    (pi, ctx) => pi.ParameterType == typeof(DiscordSocketConfig),
                    (pi, ctx) => ctx.Resolve<DiscordSocketConfig>());
            builder.Register(_ => new ActivityProvider(ActivityType.Playing, "WEEEEEEEEEEEEEEEEEEEEEEEEE")).AsImplementedInterfaces().SingleInstance();
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(OnCommand))!).Where(x => x.IsClass && !x.IsAbstract && x.IsAssignableTo<ICommand>()).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SlashCommandsManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SlashCommandHandlerProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DiscordLoggingHelper>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<AudioClientManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DiscordClientLoggingProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StartupTaskProvider>().AsImplementedInterfaces().SingleInstance()
                .WithParameter(
                (pi, ctx) => pi.ParameterType == typeof(IEnumerable<Task>),
                (pi, ctx) => GetStartupTasks(ctx));
            builder.RegisterType<MessageReceivedHandlerProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<OofReactionHandler>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CommandComparer>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ChannelDataProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<VoiceChannelActivityProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<VoiceChannelActivityHandler>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(OofReactionHandler))!).Where(x => x.IsClass && !x.IsAbstract && x.IsAssignableTo<IMessageReceivedHandler>()).AsImplementedInterfaces().SingleInstance();
            builder.RegisterSerilog(loggerConfiguration);
            builder.RegisterType<Commands.Core.Helpers.SlashCommandBuilder>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CommandOptionConverter>().AsImplementedInterfaces().SingleInstance();
            return builder.Build();
        }

        private IEnumerable<Task> GetStartupTasks(IComponentContext componentContext)
        {
            var slashCommandsManager = componentContext.Resolve<ISlashCommandsManager>();

            var actionList = new List<Action>()
            {
                async () => 
                { 
                    await slashCommandsManager.RemoveUnknownCommandsAsync();
                    await slashCommandsManager.RegisterCommandsAsync();
                }
            };

            return actionList.Select(x => new Task(x));
        }
    }
}
