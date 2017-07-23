using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using DumbJvsBrain.Common;

namespace DumbJVSManager
{
    /// <summary>
    /// Interaction logic for GameRunning.xaml
    /// </summary>
    public partial class GameRunning : Window
    {
        private readonly bool _isTest;
        private readonly string _gameLocation;
        private bool _gameRunning;
        private readonly SerialPortHandler _serialPortHandler;
        private readonly SettingsData _settingsData;
        private string _testMenuString;
        private bool _testMenuIsExe;
        private string _testMenuExe;
        private JoystickMapping _joystickMapping1;
        private JoystickMapping _joystickMapping2;
        private XInputMapping _xinputMapping1;
        private XInputMapping _xinputMapping2;

        public GameRunning(string gameLocation, GameProfiles gameProfile, bool isTest, SettingsData settingsData, string testMenuString, JoystickMapping joystickMapping1, JoystickMapping joystickMapping2, XInputMapping xinputMapping1, XInputMapping xinputMapping2, bool testMenuIsExe = false, string testMenuExe = "")
        {
            InitializeComponent();
            _gameLocation = gameLocation;
            InputCode.ButtonMode = gameProfile;
            _isTest = isTest;
            _serialPortHandler = new SerialPortHandler();
            _settingsData = settingsData;
            _testMenuString = testMenuString;
            _joystickMapping1 = joystickMapping1;
            _joystickMapping2 = joystickMapping2;
            _xinputMapping1 = xinputMapping1;
            _xinputMapping2 = xinputMapping2;
            _testMenuIsExe = testMenuIsExe;
            _testMenuExe = testMenuExe;
        }

        private void GameRunning_OnLoaded(object sender, RoutedEventArgs e)
        {
            var rawInput = new RawInputListener();
            if(_settingsData.UseMouse && (InputCode.ButtonMode == GameProfiles.LetsGoIsland || InputCode.ButtonMode == GameProfiles.SegaDreamRaiders || InputCode.ButtonMode == GameProfiles.GoldenGun))
                rawInput.ListenToDevice(InputCode.ButtonMode == GameProfiles.GoldenGun);
            var directInputListener = new DirectInputListener();
            var xinputListener = new XInputListener();
            KeyboardController kc = new KeyboardController();
            var jvsThread = new Thread(() => _serialPortHandler.ListenSerial("COM14"));
            jvsThread.Start();
            var processQueueThread = new Thread(_serialPortHandler.ProcessQueue);
            processQueueThread.Start();
            Thread directInputThreadP1;
            if (_settingsData.UseMouse && (InputCode.ButtonMode == GameProfiles.LetsGoIsland || InputCode.ButtonMode == GameProfiles.SegaDreamRaiders || InputCode.ButtonMode == GameProfiles.GoldenGun))
            {
                directInputThreadP1 = null;
            }
            else
            {
                directInputThreadP1 = CreateDirectInputThread(_settingsData.PlayerOneGuid, 1, directInputListener, xinputListener, _joystickMapping1, _xinputMapping1, _settingsData.XInputMode);
            }

            // Wait before launching second thread.
            Thread.Sleep(1000);
            var directInputThreadP2 = CreateDirectInputThread(_settingsData.PlayerTwoGuid, 2, directInputListener, xinputListener, _joystickMapping2, _xinputMapping2, _settingsData.XInputMode);
            _gameRunning = true;
            if (_settingsData.UseKeyboard && (InputCode.ButtonMode == GameProfiles.MeltyBlood || InputCode.ButtonMode == GameProfiles.VirtuaTennis4))
            {
                kc.Subscribe(directInputThreadP1 == null, directInputThreadP2 == null);
            }
            var gameThread = new Thread(() =>
            {
                ProcessStartInfo info;
                if (_isTest)
                {
                    if (_testMenuIsExe)
                    {
                        info = new ProcessStartInfo("ParrotLoader.exe", $"\"{Path.Combine(Path.GetDirectoryName(_gameLocation), _testMenuExe)}\" {_testMenuString}");
                    }
                    else
                    {
                        info = new ProcessStartInfo("ParrotLoader.exe", $"\"{_gameLocation}\" {_testMenuString}");
                    }
                }
                else
                {
                    info = new ProcessStartInfo("ParrotLoader.exe", $"\"{_gameLocation}\"");
                }
                info.UseShellExecute = false;
                info.WindowStyle = ProcessWindowStyle.Normal;
                var process = Process.Start(info);
                while (!process.HasExited)
                {
                    // We only resurrect this since I had no crashes ever in the other threads. Feel free to improve!
                    if (directInputThreadP1 != null && !directInputThreadP1.IsAlive)
                    {
                        directInputThreadP1 = CreateDirectInputThread(_settingsData.PlayerOneGuid, 1, directInputListener, xinputListener, _joystickMapping1, _xinputMapping1, _settingsData.XInputMode);
                    }

                    if (directInputThreadP2 != null && !directInputThreadP2.IsAlive)
                    {
                        directInputThreadP2 = CreateDirectInputThread(_settingsData.PlayerTwoGuid, 2, directInputListener, xinputListener, _joystickMapping2, _xinputMapping2, _settingsData.XInputMode);
                    }
                    Thread.Sleep(5000);
                }
                _serialPortHandler.KillMe = true;
                directInputListener.KillMe = true;
                xinputListener.KillMe = true;
                _gameRunning = false;
                Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(this.Close));
                if (_settingsData.UseKeyboard && (InputCode.ButtonMode == GameProfiles.MeltyBlood || InputCode.ButtonMode == GameProfiles.VirtuaTennis4)) kc.Unsubscribe();
                if (_settingsData.UseMouse && (InputCode.ButtonMode == GameProfiles.LetsGoIsland || InputCode.ButtonMode == GameProfiles.SegaDreamRaiders || InputCode.ButtonMode == GameProfiles.GoldenGun))
                    rawInput.StopListening();
            });
            gameThread.Start();
        }

        /// <summary>
        /// Creates DirectInput thread.
        /// </summary>
        /// <param name="joystickGuid">Joysticks GUID.</param>
        /// <param name="playerNumber">Player number.</param>
        /// <param name="directInputListener">Direct Input listener class.</param>
        /// <param name="joystickMapping"></param>
        /// <param name="xinputMapping"></param>
        /// <param name="useXinput">If we use xinput instead.</param>
        /// <returns>Thread id.</returns>
        private Thread CreateDirectInputThread(Guid joystickGuid, int playerNumber, DirectInputListener directInputListener, XInputListener xinputListener, JoystickMapping joystickMapping, XInputMapping xinputMapping, bool useXinput)
        {
            Thread inputThread;
            if (!useXinput)
            {
                if (joystickGuid == Guid.Empty)
                    return null;
                inputThread = new Thread(() => directInputListener.Listen(joystickGuid, playerNumber,
                    _settingsData.UseSto0ZDrivingHack, joystickMapping));
                inputThread.Start();
            }
            else
            {
                inputThread = new Thread(() => xinputListener.Listen(playerNumber, _settingsData.UseSto0ZDrivingHack, xinputMapping));
                inputThread.Start();
            }
            return inputThread;
        }

        /// <summary>
        /// Prevent closing if game is running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameRunning_OnClosing(object sender, CancelEventArgs e)
        {
            if (_gameRunning)
                e.Cancel = true;
        }
    }
}
