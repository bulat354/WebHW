using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyServer
{
    [Serializable]
    public class Configs
    {
        public static readonly Configs DefaultConfigs = new Configs(8080, "/google", "./source", "/index.html");
        [JsonPropertyName("port")]
        public int Port { get; set; }
        [JsonPropertyName("webRoot")]
        public string WebRoot { get; set; }
        [JsonPropertyName("localRoot")]
        public string LocalRoot { get; set; }
        [JsonPropertyName("defaultFile")]
        public string DefaultFile { get; set; }

        [JsonConstructor]
        public Configs(int port, string webRoot, string localRoot, string defaultFile)
        {
            Port = port;
            WebRoot = webRoot;
            LocalRoot = localRoot;
            DefaultFile = defaultFile;
        }

        public static Configs Load(string jsonPath)
        {
            if (File.Exists(jsonPath))
            {
                var json = File.ReadAllText(jsonPath);
                var config = JsonSerializer.Deserialize<Configs>(json);
                Debug.ConfigsLoadedMsg();
                return config;
            }
            else
            {
                var json = JsonSerializer.Serialize(DefaultConfigs);
                File.WriteAllText(jsonPath, json);
                Debug.ConfigFileNotFoundMsg(jsonPath);
                return DefaultConfigs;
            }
        }
    }
}
