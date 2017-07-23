using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DumbJvsBrain.Common;
using Microsoft.Win32;

namespace DumbJVSManager
{
    /// <summary>
    /// Interaction logic for EditGamesWindow.xaml
    /// </summary>
    public partial class EditGamesWindow : Window
    {
        private SettingsData _settingsData;
        private bool _xinputMode;
        public EditGamesWindow(SettingsData settingsData)
        {
            InitializeComponent();
            _settingsData = settingsData;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateJoysticks();
            ChkUseKeyboard.IsChecked = _settingsData.UseKeyboard;
            TxtSrcLocation.Text = _settingsData.SegaRacingClassicDir;
            TxtVt4Location.Text = _settingsData.VirtuaTennis4Dir;
            TxtMeltyBloodLocation.Text = _settingsData.MeltyBloodDir;
            TxtLgiLocation.Text = _settingsData.LgiDir;
            ChkUseSto0ZCheckBox.IsChecked = _settingsData.UseSto0ZDrivingHack;
            TxtSegaSonicLocation.Text = _settingsData.SegaSonicDir;
            ChkUseMouse.IsChecked = _settingsData.UseMouse;
            TxtSdrLocation.Text = _settingsData.SegaDreamRaidersDir;
            TxtGoldenGunLocation.Text = _settingsData.GoldenGunDir;
            TxtInitialD6Location.Text = _settingsData.InitialD6Dir;
            CmbJoystickInterface.SelectedIndex = _settingsData.XInputMode ? 1 : 0;
        }

        private void BtnHelpJvs(object sender, RoutedEventArgs e)
        {
            Process.Start("https://teknogods.com/phpbb/viewtopic.php?f=83&t=38555");
        }

        private void BtnRefreshJoysticks(object sender, RoutedEventArgs e)
        {
            PopulateJoysticks();
        }

        private void PopulateJoysticks()
        {
            P1JoystickComboBox.Items.Clear();
            P2JoystickComboBox.Items.Clear();
            if (_settingsData != null)
            {
                if (_settingsData.PlayerOneGuid != Guid.Empty)
                    P1JoystickComboBox.Items.Add(CreateJoystickItem(_settingsData.PlayerOneGuid, "Saved Joystick"));

                if (_settingsData.PlayerTwoGuid != Guid.Empty)
                    P2JoystickComboBox.Items.Add(CreateJoystickItem(_settingsData.PlayerTwoGuid, "Saved Joystick"));
            }
            CreateJoystickProfileAndAdd(Guid.Empty, "No joystick");
            var joysticks = JoystickHelper.GetAvailableJoysticks();
            foreach (var joystickProfile in joysticks)
            {
                CreateJoystickProfileAndAdd(joystickProfile.InstanceGuid, joystickProfile.ProductName);
            }
            P1JoystickComboBox.SelectedIndex = 0;
            P2JoystickComboBox.SelectedIndex = 0;
        }

        private void BtnTestJvs(object sender, RoutedEventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            if (ports.All(x => x != "COM14"))
            {
                var availablePorts = ports.Aggregate("", (current, t) => current + t + Environment.NewLine);
                MessageBox.Show($"Cannot find such Serial Port: COM14{Environment.NewLine}{Environment.NewLine}Available ports are:{Environment.NewLine}{availablePorts}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var result = SerialPortHelper.TestComPortEmulation("COM13", "COM14");
            MessageBox.Show(
                result
                    ? "JVS emulation seems to be functioning properly"
                    : "JVS emulation failed, please check com0com settings and tutorial at TeknoGods.com", "Test Complete", MessageBoxButton.OK,
                result ? MessageBoxImage.Information : MessageBoxImage.Error);
        }

        private void CreateJoystickProfileAndAdd(Guid guid, string productName)
        {
            P1JoystickComboBox.Items.Add(CreateJoystickItem(guid, productName));
            P2JoystickComboBox.Items.Add(CreateJoystickItem(guid, productName));
        }

        private ComboBoxItem CreateJoystickItem(Guid guid, string productName)
        {
            return new ComboBoxItem
            {
                Tag = guid,
                Content = guid + " - " + Helper.ExtractWithoutZeroes(productName)
            };
        }

        private void BtnSaveSettings(object sender, RoutedEventArgs e)
        {
            try
            {
                var settingsData = new SettingsData
                {
                    PlayerOneGuid = FetchGuidFromComboBoxItem(P1JoystickComboBox.SelectedItem),
                    PlayerTwoGuid = FetchGuidFromComboBoxItem(P2JoystickComboBox.SelectedItem),
                    UseKeyboard = ChkUseKeyboard.IsChecked != null && ChkUseKeyboard.IsChecked.Value,
                    SegaRacingClassicDir = TxtSrcLocation.Text,
                    VirtuaTennis4Dir = TxtVt4Location.Text,
                    MeltyBloodDir = TxtMeltyBloodLocation.Text,
                    LgiDir = TxtLgiLocation.Text,
                    SegaSonicDir = TxtSegaSonicLocation.Text,
                    SegaDreamRaidersDir = TxtSdrLocation.Text,
                    UseSto0ZDrivingHack = ChkUseSto0ZCheckBox.IsChecked != null && ChkUseSto0ZCheckBox.IsChecked.Value,
                    UseMouse = ChkUseMouse.IsChecked != null && ChkUseMouse.IsChecked.Value,
                    InitialD6Dir = TxtInitialD6Location.Text,
                    XInputMode = _xinputMode,
                    GoldenGunDir = TxtGoldenGunLocation.Text
                };
                JoystickHelper.Serialize(settingsData);
                _settingsData = settingsData;
                MessageBox.Show("Generation of SettingsData.xml was succesful!", "Save Complete", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Exception happened during SettingsData.xml saving!{Environment.NewLine}{Environment.NewLine}{exception}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            Hide();
        }

        private Guid FetchGuidFromComboBoxItem(object comboItem)
        {
            return (Guid)((ComboBoxItem)comboItem).Tag;
        }

        private void BtnSetupCom0Com(object sender, RoutedEventArgs e)
        {
            var comLocation = @"C:\Program Files\com0com\setupc.exe";
            var comLocationx86 = @"C:\Program Files (x86)\com0com\setupc.exe";
            var comLocationFound1 = File.Exists(comLocation);
            var comLocationFound2 = File.Exists(comLocationx86);

            if (comLocationFound1)
            {
                if (SetupComPorts(comLocation))
                {
                    MessageBox.Show("JVS Emulation ports setup succesfully", "Success!", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }
            }
            else if (comLocationFound2)
            {
                if (SetupComPorts(comLocationx86))
                {
                    MessageBox.Show("JVS Emulation ports setup succesfully", "Success!", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show(
                    "Cannot locate com0com with setupc.exe, please reinstall com0com to default directory in c:", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("JVS Emulation port setup failed!", "Fail", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private bool SetupComPorts(string applicationLocation)
        {
            var portList = StartProcess(applicationLocation, "list", true).Split("\n".ToCharArray());
            var portCnt = portList.Count(x => x.Contains("PortName")) / 2;
            for (var i = portCnt - 1; i != -1; i--)
            {
                StartProcess(applicationLocation, $"remove {i}", true);
            }
            StartProcess(applicationLocation, $"install PortName=COM13 PortName=COM14", true);
            portList = StartProcess(applicationLocation, "list", true).Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (portList.Length == 2 && portList[0].Contains("CNCA0 PortName=COM13") &&
                portList[1].Contains("CNCB0 PortName=COM14"))
            {
                return true;
            }
            return false;
        }

        private string StartProcess(string exeLocation, string arguments, bool admin)
        {
            const int errorCancelled = 1223;
            string output = string.Empty;
            ProcessStartInfo info = new ProcessStartInfo(exeLocation, arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.WindowStyle = ProcessWindowStyle.Normal;
            info.WorkingDirectory = Path.GetDirectoryName(exeLocation);
            if (admin) info.Verb = "runas";
            try
            {
                var proc = Process.Start(info);
                using (StreamReader myOutput = proc.StandardOutput)
                {
                    output = myOutput.ReadToEnd();
                    proc.Close();
                    return output;
                }
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == errorCancelled)
                    MessageBox.Show("Could not get admin!", "Running process failed", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    throw;
                return output;
            }
        }

        private void EditGamesWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void SelectExecutableForTextBox(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                Title = "Please select Ring game executable",
                Filter = "Executables (*.exe) | *.exe"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ((TextBox) sender).Text = openFileDialog.FileName;
            }
        }

        private void BtnKeyboardRemap_OnClick(object sender, RoutedEventArgs e)
        {
            var remap = new KeyboardRemap();
            remap.ShowDialog();
        }

        private void BtnRemapJoysticks(object sender, RoutedEventArgs e)
        {
            if (!_settingsData.XInputMode)
            {
                if (_settingsData.PlayerOneGuid == Guid.Empty)
                {
                    MessageBox.Show("Please save your selected joystick first.", "Information", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }
                var remap = new DirectInputRemap(_settingsData, true, 1);
                remap.ShowDialog();
                return;
            }
            var xremap = new XInputRemap(true, 1);
            xremap.ShowDialog();
        }

        private void BtnRemapJoysticks2(object sender, RoutedEventArgs e)
        {
            if (!_settingsData.XInputMode)
            {
                if (_settingsData.PlayerOneGuid == Guid.Empty)
                {
                    MessageBox.Show("Please save your selected joystick first.", "Information", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }
                var remap = new DirectInputRemap(_settingsData, false, 2);
                remap.ShowDialog();
                return;
            }
            var xremap = new XInputRemap(false, 2);
            xremap.ShowDialog();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox) e.Source).SelectedIndex == 0)
            {
                P1JoystickLabel.Visibility = Visibility.Visible;
                P1JoystickComboBox.Visibility = Visibility.Visible;
                P2JoystickLabel.Visibility = Visibility.Visible;
                P2JoystickComboBox.Visibility = Visibility.Visible;
                BtnJoystickRefresh.Visibility = Visibility.Visible;
                _xinputMode = false;
                _settingsData.XInputMode = false;
            }
            if (((ComboBox) e.Source).SelectedIndex == 1)
            {
                P1JoystickLabel.Visibility = Visibility.Collapsed;
                P1JoystickComboBox.Visibility = Visibility.Collapsed;
                P2JoystickLabel.Visibility = Visibility.Collapsed;
                P2JoystickComboBox.Visibility = Visibility.Collapsed;
                BtnJoystickRefresh.Visibility = Visibility.Collapsed;
                _xinputMode = true;
                _settingsData.XInputMode = true;
            }
        }
    }
}
