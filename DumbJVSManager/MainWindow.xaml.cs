using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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
            switch ((GameProfiles)Convert.ToInt32(((ComboBoxItem)GameListComboBox.SelectedItem).Tag))
            {
                case GameProfiles.LetsGoIsland:
                    ValidateAndRun(_settingsData.LgiDir, GameProfiles.LetsGoIsland, "-TestMode");
                    break;
                case GameProfiles.MeltyBlood:
                    ValidateAndRun(_settingsData.MeltyBloodDir, GameProfiles.MeltyBlood, "/T");
                    break;
                case GameProfiles.SegaRacingClassic:
                    ValidateAndRun(_settingsData.SegaRacingClassicDir, GameProfiles.SegaRacingClassic, "-t");
                    break;
                case GameProfiles.VirtuaTennis4:
                    ValidateAndRun(_settingsData.VirtuaTennis4Dir, GameProfiles.VirtuaTennis4, "-t");
                    break;
                case GameProfiles.SegaSonicAllStarsRacing:
                    ValidateAndRun(_settingsData.SegaSonicDir, GameProfiles.SegaSonicAllStarsRacing, "", true, "gametest.exe");
                    break;
                case GameProfiles.SegaDreamRaiders:
                    ValidateAndRun(_settingsData.SegaDreamRaidersDir, GameProfiles.SegaDreamRaiders, "T");
                    break;
                case GameProfiles.InitialD6Aa:
                    ValidateAndRun(_settingsData.InitialD6Dir, GameProfiles.InitialD6Aa, "-t");
                    break;
                case GameProfiles.GoldenGun:
                    ValidateAndRun(_settingsData.GoldenGunDir, GameProfiles.GoldenGun, "-t", true, "TestMode.exe");
                    break;
            }
        }

        /// <summary>
        /// Validates that the game exists and then runs it with the emulator.
        /// </summary>
        /// <param name="gameLocation">Game executable location.</param>
        /// <param name="gameProfile">Input profile.</param>
        /// <param name="testMenuString">Command to run test menu.</param>
        /// <param name="testMenuIsExe">If uses separate exe.</param>
        /// <param name="exeName">Test menu exe name.</param>
        private void ValidateAndRun(string gameLocation, GameProfiles gameProfile, string testMenuString, bool testMenuIsExe = false, string exeName = "")
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

            JoystickMapping jmap1 = null;
            JoystickMapping jmap2 = null;

            XInputMapping xmap1 = null;
            XInputMapping xmap2 = null;

            try
            {
                if (File.Exists("JoystickMapping1.xml"))
                {
                    jmap1 = JoystickHelper.DeSerialize(1);
                }

                if (File.Exists("JoystickMapping2.xml"))
                {
                    jmap2 = JoystickHelper.DeSerialize(2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loading joystick mapping failed with error: {ex.InnerException} {ex.Message}", "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            try
            {
                if (File.Exists("XInputMapping1.xml"))
                {
                    xmap1 = JoystickHelper.DeSerializeXInput(1);
                }

                if (File.Exists("XInputMapping2.xml"))
                {
                    xmap2 = JoystickHelper.DeSerializeXInput(2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loading joystick mapping failed with error: {ex.InnerException} {ex.Message}", "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (jmap1 == null)
                jmap1 = new JoystickMapping();

            if (jmap2 == null)
                jmap2 = new JoystickMapping();

            if (xmap1 == null)
                xmap1 = new XInputMapping();

            if (xmap2 == null)
                xmap2 = new XInputMapping();

            var gameRunning = new GameRunning(gameLocation, gameProfile, testMenu, _settingsData, testMenuString, jmap1, jmap2, xmap1, xmap2, testMenuIsExe, exeName);
            gameRunning.ShowDialog();
            gameRunning.Close();
        }

        private void BtnSettings(object sender, RoutedEventArgs e)
        {
            _settingsWindow.ShowDialog();
            LoadSettingsData();
        }

        private void BtnQuit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _settingsWindow.Close();
            Application.Current.Shutdown(0);
        }

        private void BtnAbout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "TeknoParrot by Reaver / TeknoGods\nSpecial Thanks to Patreons and especially:\n- Enrique Rivera\n- Eden Aharoni",
                "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://www.patreon.com/Teknogods");
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("https://teknogods.com");
        }

        private void BtnHelp(object sender, RoutedEventArgs e)
        {
            Process.Start("https://teknogods.com/phpbb/viewforum.php?f=83");
        }
    }
}
