using Autofac;
using Discord;
using Discord.WebSocket;
using DiscordBot.Commands;
using DiscordBot.Commands.Core;
using DiscordBot.Core;
using DiscordBot.Core.Helpers;
using DiscordBot.Core.Providers;
using Serilog;
using Serilog.Extensions.Autofac.DependencyInjection;
using System.Reflection;

namespace DiscordBot
{
    public class Startup
    {
        public IContainer GetAutofacContainer()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console();

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
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(OnCommand))!).Where(x => x.IsClass && !x.IsAbstract && x.IsAssignableTo<ICommand>()).As<ICommand>();
            builder.RegisterType<SlashCommandsManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SlashCommandHandlerProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DiscordLoggingHelper>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterSerilog(loggerConfiguration);
            return builder.Build();
        }
    }
}
