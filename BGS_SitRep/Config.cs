using System.IO;
using Newtonsoft.Json;

namespace BGS_SitRep
{
    class Config
    {
        //Config file and folder
        private const string ConfigFile = "config.json";
        private const string ConfigFolder = "Resources";
        private static readonly string ConfigFullPath = Path.Combine(ConfigFolder, ConfigFile);

        public static BotConfig Bot;

        static Config()
        {
            //Check for the config directory and make it if it doesn't exist.
            if (!Directory.Exists(ConfigFolder)) Directory.CreateDirectory(ConfigFolder);

            if (!File.Exists(ConfigFullPath))
            {
                Bot = new BotConfig();
                string json = JsonConvert.SerializeObject(Bot, Formatting.Indented);
                File.WriteAllText(ConfigFullPath, json);
            }
            else
            {
                string json = File.ReadAllText(ConfigFullPath);
                Bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        /*
         * token: Discord bot token
         * cmdPrefix: The prefix the bot looks for to determine if a command is given, e.g., '$'
         * faction: The faction of the group using the bot, used for expansion queries.
         */
        public struct BotConfig
        {
            public string Token;
            public string CmdPrefix;
            public string Faction;
        }
    }
}
