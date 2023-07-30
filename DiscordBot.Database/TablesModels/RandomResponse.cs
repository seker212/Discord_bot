using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Database.TablesModels
{
    public record RandomResponse
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string Response { get; set; }
    }
}
