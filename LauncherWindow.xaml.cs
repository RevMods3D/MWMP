using MWMP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace MWMP_Servers
{
    /// <summary>
    /// Interaction logic for LauncherWindow.xaml
    /// </summary>
    public partial class LauncherWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<ServerInfo> GlobalServers { get; set; } = new ObservableCollection<ServerInfo>();
        public ObservableCollection<ServerInfo> FavoriteServers { get; set; } = new ObservableCollection<ServerInfo>();
        public ObservableCollection<PlayerInfo> Players { get; } = new ObservableCollection<PlayerInfo>();
        public ObservableCollection<ServerInfo> Servers { get; set; } = new ObservableCollection<ServerInfo>();
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly HttpClient httpClient = new HttpClient();

        private ServerInfo _selectedServer;
        public ServerInfo SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (_selectedServer != value)
                {
                    _selectedServer = value;
                    OnPropertyChanged(nameof(SelectedServer));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        // Then in your selection changed event:
        private void GlobalListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GlobalListView.SelectedItem is ServerInfo server)
            {
                SelectedServer = server;
            }
        }

        public LauncherWindow()
        {
            InitializeComponent();
            _ = LoadGlobalServersAsync();
            this.Focus(); // Ensures window is focused
            Servers = new ObservableCollection<ServerInfo>();

            GlobalListView.ItemsSource = GlobalServers;
            FavoriteListView.ItemsSource = FavoriteServers;

            DataContext = this;
        }
        ////// UI //////
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            /*
            Preferences prefsWindow = new Preferences
            {
                Owner = this 
            };

            prefsWindow.ShowDialog();*/
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("MW12 Launcher\nVersion 1.0\nCreated by RevMods3D");
        }
        

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }
        /////////////////

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is ServerInfo selectedServer)
            {
                var editor = new ServerEditorWindow(selectedServer) { Owner = this };

                if (editor.ShowDialog() == true)
                {
                    // Optionally update something if needed
                    // ObservableCollection updates automatically
                }
            }
        }

        private void ServerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GlobalListView.SelectedItem is ServerInfo server)
            {
                Players.Clear();
                foreach (var player in server.PlayersList)
                    Players.Add(player);
            }
            else
            {
                Players.Clear();
            }
        }

        private void FavoriteListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavoriteListView.SelectedItem is ServerInfo server)
                SelectedServer = server;
        }

        private async Task LoadGlobalServersAsync()
        {
            try
            {
                string url = "https://nodejs-production-d173.up.railway.app/api/servers";
                string json = await _httpClient.GetStringAsync(url);

                var servers = JsonConvert.DeserializeObject<List<ServerInfo>>(json);

                if (servers != null)
                {
                    GlobalServers.Clear();
                    foreach (var server in servers)
                    {
                        // Convert PlayersList from List<PlayerInfo> to ObservableCollection<PlayerInfo> if necessary
                        if (server.PlayersList == null)
                            server.PlayersList = new ObservableCollection<PlayerInfo>();
                        else if (!(server.PlayersList is ObservableCollection<PlayerInfo>))
                            server.PlayersList = new ObservableCollection<PlayerInfo>(server.PlayersList);

                        foreach (var player in server.PlayersList)
                        {
                            Console.WriteLine($"{player.Name} - PPing: {player.PPing}, Time: {player.Time}");
                        }

                        GlobalServers.Add(server);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load servers: {ex.Message}");
            }
        }

        private async Task<ObservableCollection<PlayerInfo>> GetPlayersFromApiAsync(string serverIp)
        {
            try
            {
                string url = "https://nodejs-production-d173.up.railway.app/api/servers";
                string json = await _httpClient.GetStringAsync(url);

                var servers = JsonConvert.DeserializeObject<List<ServerInfo>>(json);

                var server = servers?.Find(s => s.IP == serverIp);

                // Already ObservableCollection<PlayerInfo>
                return server?.PlayersList ?? new ObservableCollection<PlayerInfo>();
            }
            catch
            {
                return new ObservableCollection<PlayerInfo>();
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var json = await httpClient.GetStringAsync("https://nodejs-production-d173.up.railway.app/api/servers");
                var serversFromApi = JsonConvert.DeserializeObject<List<ApiServer>>(json);

                GlobalServers.Clear();

                foreach (var apiServer in serversFromApi)
                {
                    var server = new ServerInfo
                    {
                        Name = apiServer.Name,
                        IP = apiServer.IP,
                        MaxPlayers = apiServer.MaxPlayers,
                        Mode = apiServer.Mode,
                        Tag = apiServer.Tag,
                        Ping = apiServer.Ping
                    };

                    foreach (var p in apiServer.Players)
                        server.PlayersList.Add(new PlayerInfo { Name = p.Name, PPing = p.PPing, Time = p.Time });

                    GlobalServers.Add(server);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to refresh servers: " + ex.Message);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var editor = new ServerEditorWindow() { Owner = this };

            if (editor.ShowDialog() == true)
            {
                FavoriteServers.Add(editor.GetServer());
            }
        }

        private void EditServer_Click(object sender, RoutedEventArgs e)
        {
            if (FavoriteListView.SelectedItem is ServerInfo server)
            {
                var editor = new ServerEditorWindow(server) { Owner = this };
                editor.ShowDialog();
            }
        }

        private void DeleteServer_Click(object sender, RoutedEventArgs e)
        {
            if (FavoriteListView.SelectedItem is ServerInfo server)
            {
                Servers.Remove(server);
            }
        }

        private void AddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalListView.SelectedItem is ServerInfo server)
            {
                // Avoid duplicates
                if (!FavoriteServers.Any(s => s.IP == server.IP))
                {
                    FavoriteServers.Add(server);
                    MessageBox.Show($"{server.Name} added to favorites!");
                }
                else
                {
                    MessageBox.Show($"{server.Name} is already in favorites.");
                }
            }
        }

        private void RemoveFromFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (FavoriteListView.SelectedItem is ServerInfo server)
            {
                FavoriteServers.Remove(server);
                MessageBox.Show($"{server.Name} removed from favorites.");
            }
        }

        private void ListViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                item.IsSelected = true;
            }
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is GridViewColumnHeader headerClicked) || headerClicked.Column == null)
                return;

            string sortBy = headerClicked.Column.Header as string;
            if (string.IsNullOrEmpty(sortBy))
                return;

            ListSortDirection direction = ListSortDirection.Ascending;
            if (_lastHeaderClicked == headerClicked && _lastDirection == ListSortDirection.Ascending)
                direction = ListSortDirection.Descending;

            var dataView = CollectionViewSource.GetDefaultView(PlayerListView.ItemsSource);

            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(new SortDescription(sortBy, direction));
            dataView.Refresh();

            _lastHeaderClicked = headerClicked;
            _lastDirection = direction;
        }

        private void ExportServers_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                FileName = "favorite_servers.json"
            };

            if (dialog.ShowDialog() == true)
            {
                string json = JsonConvert.SerializeObject(FavoriteServers, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(dialog.FileName, json);
                MessageBox.Show("Favorite servers exported successfully!");
            }
        }
        private void ImportServers_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json"
            };

            if (dialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(dialog.FileName);
                var importedServers = JsonConvert.DeserializeObject<List<ServerInfo>>(json);

                FavoriteServers.Clear();
                if (importedServers != null)
                {
                    foreach (var server in importedServers)
                        FavoriteServers.Add(server);
                }
                MessageBox.Show("Favorite servers imported successfully!");
            }
        }
        public class SecondsToMinutesConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is int seconds)
                    return $"{seconds / 60:D2}:{seconds % 60:D2}";
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                => throw new NotImplementedException();
        }
        public class ApiServer
        {
            public string Name { get; set; }
            public string IP { get; set; }
            public int MaxPlayers { get; set; }
            public string Mode { get; set; }
            public string Tag { get; set; }
            public string Ping { get; set; }
            public List<ApiPlayer> Players { get; set; } = new List<ApiPlayer>();
        }

        public class ApiPlayer
        {
            public string Name { get; set; }
            public int PPing { get; set; }
            public int Time { get; set; }
        }
    }
}
