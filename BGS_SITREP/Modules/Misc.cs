using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BGS_SITREP.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echos whatever the user types in an embedded format.")]
        [Alias("e", "banana")]
        public async Task Echo([Remainder] string msg)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Echoed Message");
            embed.WithDescription(msg);
            embed.WithColor(new Color(0, 255, 0));

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
