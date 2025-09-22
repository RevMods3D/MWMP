using MWMP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace NFSMW12_Custom_Servers
{
    /// <summary>
    /// Interaction logic for LauncherWindow.xaml
    /// </summary>
    public partial class LauncherWindow : Window
    {

        public ObservableCollection<ServerInfo> Servers { get; set; } = new ObservableCollection<ServerInfo>();
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public LauncherWindow()
        {
            InitializeComponent();
            this.Focus(); // Ensures window is focused
            Servers = new ObservableCollection<ServerInfo>();
            FavoriteListView.ItemsSource = Servers;
            GlobalListView.ItemsSource = Servers;
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var editor = new ServerEditorWindow()
            {
                Owner = this
            };

            if (editor.ShowDialog() == true)
            {
                Servers.Add(editor.GetServer());
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

        private void ListViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                item.IsSelected = true;
            }
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked?.Tag == null) return;

            string sortBy = headerClicked.Tag.ToString();
            ListSortDirection direction;

            // Toggle sort direction if clicking the same header
            if (headerClicked == _lastHeaderClicked)
            {
                direction = _lastDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

            // Determine which ListView the header belongs to
            ListView listView = null;
            DependencyObject parent = headerClicked;
            while (parent != null)
            {
                if (parent is ListView lv)
                {
                    listView = lv;
                    break;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (listView != null)
            {
                var dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                dataView.SortDescriptions.Clear();
                dataView.SortDescriptions.Add(new SortDescription(sortBy, direction));
                dataView.Refresh();
            }

            _lastHeaderClicked = headerClicked;
            _lastDirection = direction;
        }

        private void ExportServers_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                FileName = "servers.json"
            };

            if (dialog.ShowDialog() == true)
            {
                var servers = (ObservableCollection<ServerInfo>)GlobalListView.ItemsSource;
                string json = JsonConvert.SerializeObject(servers, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(dialog.FileName, json);
                MessageBox.Show("Servers exported successfully!");
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
                var importedServers = JsonConvert.DeserializeObject<ObservableCollection<ServerInfo>>(json);
                Servers.Clear();
                if (importedServers != null)
                {
                    foreach (var server in importedServers)
                        Servers.Add(server);
                }
                MessageBox.Show("Servers imported successfully!");
            }
        }
    }
}
