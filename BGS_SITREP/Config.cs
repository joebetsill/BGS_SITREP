using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace BGS_SITREP
{
    class Config
    {
        //Config file and folder
        private const string _configFile = "config.json";
        private const string _configFolder = "Resources";
        private static readonly string _configFullPath = Path.Combine(_configFolder, _configFile);

        public static BotConfig bot;

        static Config()
        {
            //Check for the config directory and make it if it doesn't exist.
            if (!Directory.Exists(_configFolder)) Directory.CreateDirectory(_configFolder);

            if (!File.Exists(_configFullPath))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(_configFullPath, json);
            }
            else
            {
                string json = File.ReadAllText(_configFullPath);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public struct BotConfig
        {
            public string token;
            public string cmdPrefix;
        }
    }
}
