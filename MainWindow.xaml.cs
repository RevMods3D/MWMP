using Microsoft.Win32;
using NFSServer.UI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media;
using System.Windows.Threading;


namespace NFSLauncher
{
    public partial class MainWindow : Window
    {
        private readonly string configFile = "config.txt";
        private string gamePath;
        private NFSMWServer server; // <-- Add this here
        private readonly DispatcherTimer statusTimer;

        public MainWindow()
        {
            InitializeComponent();
            LoadGamePath();

            statusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            statusTimer.Tick += async (s, e) => await CheckServerStatus();
            statusTimer.Start();
        }

        public enum ServerStatus
        {
            Unknown,
            Online,
            Offline
        }

        // Path to the configuration file//
        private void LoadGamePath()
        {
            if (File.Exists(configFile))
            {
                gamePath = File.ReadAllText(configFile);
                if (!File.Exists(gamePath))
                {
                    gamePath = null;
                    MessageBox.Show("Saved game path not found. Please select the game location.");
                }
            }
            else
            {
                MessageBox.Show("Please select the game location.");
            }
        }

        private void SaveGamePath()
        {
            if (!string.IsNullOrEmpty(gamePath))
            {
                File.WriteAllText(configFile, gamePath);
            }
        }
        ////////////////////////////////////
        private void BtnSelectGame_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Select NFS13.exe",
                Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                gamePath = dlg.FileName;
                SaveGamePath();
                MessageBox.Show($"Game path saved:\n{gamePath}");
            }
        }

        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            if (server == null)
            {
                server = new NFSMWServer();
                server.Start(); // Defaults to http://+:80/
                lblStatus.Content = "Server Status: ONLINE";
                SetStatus(ServerStatus.Online);
            }
            else
            {
                MessageBox.Show("Server is already running.");
            }
        }


        private void BtnStartGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(gamePath) && File.Exists(gamePath))
                {
                    Process.Start(gamePath);
                }
                else
                {
                    MessageBox.Show("Game path not set or file not found. Please select the game location.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start game: {ex.Message}");
            }
        }
        /////////////////UI buttons////////////////////

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click (Object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        //////////////////////////////////////////////
        private async Task CheckServerStatus()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage res = await client.GetAsync("http://localhost:80/");
                    if (res.IsSuccessStatusCode)
                        lblStatus.Content = "Server Status: ONLINE";
                    else
                        lblStatus.Content = "Server Status: OFFLINE";
                }
            }
            catch
            {
                lblStatus.Content = "Server Status: OFFLINE";
            }
        }

        private void SetStatus(ServerStatus status)
        {
            switch (status)
            {
                case ServerStatus.Online:
                    lblStatus.Content = "Server Status: ONLINE";
                    lblStatus.Foreground = Brushes.LimeGreen;
                    break;

                case ServerStatus.Offline:
                    lblStatus.Content = "Server Status: OFFLINE";
                    lblStatus.Foreground = Brushes.Red;
                    break;

                case ServerStatus.Unknown:
                default:
                    lblStatus.Content = "Server Status: UNKNOWN";
                    lblStatus.Foreground = Brushes.White;
                    break;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            server?.Stop();  // Stops your NFSMWServer if running
            base.OnClosed(e); // Always call base method
        }
        private void BtnStopServer_Click(object sender, RoutedEventArgs e)
        {
            server?.Stop();
            SetStatus(ServerStatus.Offline); // after stop
        }
    }
}
