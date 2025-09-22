using MWMP;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MWMP_Servers
{
    public class ServerInfo : INotifyPropertyChanged
    {
        private string name;
        private string ip;
        private int maxPlayers;
        private string mode;
        private string tag;
        private string ping;
        public string PlayerCountDisplay => $"{Players}/{MaxPlayers}";

        // Use ObservableCollection directly
        [JsonProperty("players")]
        public ObservableCollection<PlayerInfo> PlayersList { get; set; } = new ObservableCollection<PlayerInfo>();

        [JsonIgnore]
        public int Players => PlayersList?.Count ?? 0;

        [JsonProperty("maxPlayers")]
        public int MaxPlayers
        {
            get => maxPlayers;
            set { maxPlayers = value; OnPropertyChanged(nameof(MaxPlayers)); OnPropertyChanged(nameof(PlayerCountDisplay)); }
        }

        [JsonProperty("name")]
        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }

        [JsonProperty("ip")]
        public string IP
        {
            get => ip;
            set { ip = value; OnPropertyChanged(nameof(IP)); }
        }

        [JsonProperty("mode")]
        public string Mode
        {
            get => mode;
            set { mode = value; OnPropertyChanged(nameof(Mode)); }
        }

        [JsonProperty("tag")]
        public string Tag
        {
            get => tag;
            set { tag = value; OnPropertyChanged(nameof(Tag)); }
        }

        [JsonProperty("ping")]
        public string Ping
        {
            get => ping;
            set { ping = value; OnPropertyChanged(nameof(Ping)); }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
