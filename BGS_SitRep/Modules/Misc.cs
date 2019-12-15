using System.Threading.Tasks;
using Discord.Commands;

namespace BGS_SitRep.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary("Shows the available commands.")]
        public async Task help()
        {
            await Context.Channel.SendMessageAsync("```COMMANDS:\n"
                                                   + "* " + Config.Bot.CmdPrefix + "sitrep [system] : Displays a Situation Report of a system.\n"
                                                   + "* " + Config.Bot.CmdPrefix + "traffic [system] : Displays the ship traffic of a system.\n"
                                                   + "* " + Config.Bot.CmdPrefix + "exrep [system] : Displays the expansion candidate for " + Config.Bot.Faction + " from the system.\n"
                                                   + "* " + Config.Bot.CmdPrefix + "help : Displays this text.```");

        }
    }
}