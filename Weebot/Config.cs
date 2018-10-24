using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Weebot
{
    public class Config
    {
        private const string configFile = "config.json";
        private const string configDir = "Resources";

        public static BotConfig bot;

        static Config()
        {
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            if (!File.Exists(configDir + "/" + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configDir + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configDir + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig
    {
        public string token;
        public string prefix;
    }
}
