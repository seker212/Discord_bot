using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands.Core.CommandAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GuildIdAttribute : Attribute
    {
        public GuildIdAttribute(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; }
    }
}
