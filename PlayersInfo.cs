using Newtonsoft.Json;

namespace MWMP_Servers
{
    public class PlayerInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pping")]
        public int PPing { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
