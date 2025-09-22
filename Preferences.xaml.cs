//Broken for now

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using System.Windows.Shapes;

namespace MWMP
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {
        public Preferences()
        {
            InitializeComponent();

            // Initialize ComboBox
            ThemeComboBox.SelectionChanged += ThemeComboBox_SelectionChanged;

            // Disable custom color pickers at start
            SetCustomColorPickersEnabled(false);
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem item)
            {
                string selectedTheme = item.Content.ToString();

                switch (selectedTheme)
                {
                    case "Dark":
                        ApplyDarkTheme();
                        SetCustomColorPickersEnabled(false);
                        break;

                    case "Light":
                        ApplyLightTheme();
                        SetCustomColorPickersEnabled(false);
                        break;

                    case "Custom":
                        SetCustomColorPickersEnabled(true);
                        break;
                }
            }
        }

        private void ApplyDarkTheme()
        {
            this.Background = new SolidColorBrush(Color.FromRgb(30, 30, 47));
            // Optionally update other defaults for dark theme
        }

        private void ApplyLightTheme()
        {
            this.Background = Brushes.WhiteSmoke;
            // Optionally update other defaults for light theme
        }

        private void SetCustomColorPickersEnabled(bool enabled)
        {
            LauncherBackgroundColorButton.IsEnabled = enabled;
            MenuBackgroundColorButton.IsEnabled = enabled;
            ButtonColorButton.IsEnabled = enabled;
        }

        private void SelectLauncherBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LauncherBackgroundPreview.Fill = new SolidColorBrush(
                    Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B));
            }
        }

        private void SelectMenuBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MenuBackgroundPreview.Fill = new SolidColorBrush(
                    Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B));
            }
        }

        private void SelectButtonColor_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ButtonColorPreview.Fill = new SolidColorBrush(
                    Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B));
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Save theme/color selections to settings or JSON
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}