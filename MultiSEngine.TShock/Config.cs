using Newtonsoft.Json;
using System.IO;
using TShockAPI;

namespace MultiSEngine.TShock
{
    internal class Config
    {
        [JsonIgnore]
        public static readonly string ConfigPath = Path.Combine(TShockAPI.TShock.SavePath, "MultiSEngine.TShock.json");
        [JsonIgnore]
        internal static Config _instance;
        [JsonIgnore]
        public static Config Instance
        {
            get
            {
                if (_instance is null)
                {
                    FileTools.CreateIfNot(ConfigPath, JsonConvert.SerializeObject(new Config()));
                    _instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath));
                }
                return _instance;
            }
        }
        public string Token { get; set; } = "114514";
    }
}
