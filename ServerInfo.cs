using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFSMW12_Custom_Servers
{
    public class ServerInfo : INotifyPropertyChanged
    {
        private string name;
        private string IP;
        [JsonIgnore] private int players;
        [JsonIgnore] private int maxPlayers;
        [JsonIgnore] private string mode;
        [JsonIgnore] private string tag;

        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }

        public int Players
        {
            get => players;
            set { players = value; OnPropertyChanged(nameof(Players)); OnPropertyChanged(nameof(PlayerCountDisplay)); }
        }

        public int MaxPlayers
        {
            get => maxPlayers;
            set { maxPlayers = value; OnPropertyChanged(nameof(MaxPlayers)); OnPropertyChanged(nameof(PlayerCountDisplay)); }
        }

        public string Ip
        {
            get => IP;
            set { IP = value; OnPropertyChanged(nameof(IP)); }
        }

        public string Mode
        {
            get => mode;
            set { mode = value; OnPropertyChanged(nameof(Mode)); }
        }

        public string Tag
        {
            get => tag;
            set { tag = value; OnPropertyChanged(nameof(tag)); }

        }

        public string PlayerCountDisplay => $"{Players}/{MaxPlayers}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
