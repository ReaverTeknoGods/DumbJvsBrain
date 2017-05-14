using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using DumbJvsBrain.Common;

namespace DumbJVSManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SettingsData _settingsData;
        private EditGamesWindow _settingsWindow;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettingsData();
            _settingsWindow = new EditGamesWindow(_settingsData);
        }

        /// <summary>
        /// Loads the settings data file.
        /// </summary>
        private void LoadSettingsData()
        {
            try
            {
                if (!File.Exists("SettingsData.xml"))
                {
                    MessageBox.Show("Seems this is first time you are running me, please set settings.",
                        "Hello World", MessageBoxButton.OK, MessageBoxImage.Information);
                    _settingsData = new SettingsData();
                    JoystickHelper.Serialize(_settingsData);
                    return;
                }
                _settingsData = JoystickHelper.DeSerialize();
                if (_settingsData == null)
                {
                    _settingsData = new SettingsData();
                    JoystickHelper.Serialize(_settingsData);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Exception happened during loading SettingsData.xml! Generate new one by saving!{Environment.NewLine}{Environment.NewLine}{e}",
                    "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnStartGame(object sender, RoutedEventArgs e)
        {
            switch ((GameProfiles)GameListComboBox.SelectedIndex)
            {
                case GameProfiles.MeltyBlood:
                    ValidateAndRun(_settingsData.MeltyBloodDir, GameProfiles.MeltyBlood);
                    break;
                case GameProfiles.SegaRacingClassic:
                    ValidateAndRun(_settingsData.SegaRacingClassicDir, GameProfiles.SegaRacingClassic);
                    break;
                case GameProfiles.VirtuaTennis4:
                    ValidateAndRun(_settingsData.VirtuaTennis4Dir, GameProfiles.VirtuaTennis4);
                    break;
            }
        }

        /// <summary>
        /// Validates that the game exists and then runs it with the emulator.
        /// </summary>
        /// <param name="gameLocation">Game executable location.</param>
        /// <param name="gameProfile">Input profile.</param>
        private void ValidateAndRun(string gameLocation, GameProfiles gameProfile)
        {
            if (!File.Exists(gameLocation))
            {
                MessageBox.Show($"Cannot find game exe at: {gameLocation}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            if (!File.Exists("ParrotLoader.exe"))
            {
                MessageBox.Show($"Cannot find ParrotLoader.exe", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            if (!File.Exists("TeknoParrot.dll"))
            {
                MessageBox.Show($"Cannot find TeknoParrot.dll", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            var testMenu = ChkTestMenu.IsChecked != null && ChkTestMenu.IsChecked.Value;
            var gameRunning = new GameRunning(gameLocation, gameProfile, testMenu, _settingsData);
            gameRunning.ShowDialog();
            gameRunning.Close();
        }

        private void BtnSettings(object sender, RoutedEventArgs e)
        {
            _settingsWindow.ShowDialog();
            MessageBox.Show("Settings saved, please restart me.", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
            Application.Current.Shutdown(0);
        }

        private void BtnQuit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _settingsWindow.Close();
        }
    }
}
