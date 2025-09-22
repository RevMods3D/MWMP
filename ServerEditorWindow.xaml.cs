using MWMP_Servers;
using System.Windows;

namespace MWMP
{
    public partial class ServerEditorWindow : Window
    {
        private readonly ServerInfo server;
        public bool IsNewServer { get; set; } = false;

        public ServerEditorWindow(ServerInfo server = null)
        {
            InitializeComponent();

            if (server == null)
            {
                // Creating a new server
                this.server = new ServerInfo();
                IsNewServer = true;
            }
            else
            {
                // Editing an existing server
                this.server = server;
            }

            // Load values into textboxes
            NameTextBox.Text = this.server.Name;
            IPTextBox.Text = this.server.IP;
            ModeTextBox.Text = this.server.Mode;
            TagTextBox.Text = this.server.Tag;

            // Button handlers
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += (s, e) => Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            server.Name = NameTextBox.Text;
            server.IP = IPTextBox.Text;
            server.Mode = ModeTextBox.Text;
            server.Tag = TagTextBox.Text;

            // Close window
            DialogResult = true;
            Close();
        }

        public ServerInfo GetServer()
        {
            return server;
        }
    }
}
