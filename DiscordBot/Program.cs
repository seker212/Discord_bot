using Autofac;
using DiscordBot.Core;

namespace DiscordBot
{
    public static class Program
    {
        public static async Task Main(string[] args)
            => await new Startup().GetAutofacContainer().Resolve<BotClientRunner>().Run();
    }
}