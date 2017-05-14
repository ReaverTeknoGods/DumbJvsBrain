using System;
using System.ComponentModel;
using System.Diagnostics;
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
        public GameRunning(string gameLocation, GameProfiles gameProfile, bool isTest, SettingsData settingsData)
        {
            InitializeComponent();
            _gameLocation = gameLocation;
            InputCode.ButtonMode = gameProfile;
            _isTest = isTest;
            _serialPortHandler = new SerialPortHandler();
            _settingsData = settingsData;
        }

        private void GameRunning_OnLoaded(object sender, RoutedEventArgs e)
        {
            var directInputListener = new DirectInputListener();
            KeyboardController kc = new KeyboardController();
            var jvsThread = new Thread(() => _serialPortHandler.ListenSerial("COM14"));
            jvsThread.Start();
            var processQueueThread = new Thread(_serialPortHandler.ProcessQueue);
            processQueueThread.Start();
            var directInputThreadP1 = CreateDirectInputThread(_settingsData.PlayerOneGuid, 1, directInputListener);
            // Wait before launching second thread.
            Thread.Sleep(1000);
            var directInputThreadP2 = CreateDirectInputThread(_settingsData.PlayerTwoGuid, 2, directInputListener);
            _gameRunning = true;
            if (_settingsData.UseKeyboard)
            {
                kc.Subscribe(directInputThreadP1 == null, directInputThreadP2 == null);
            }
            var gameThread = new Thread(() =>
            {
                var info = _isTest ? new ProcessStartInfo("ParrotLoader.exe", $"\"{_gameLocation}\" -t") : new ProcessStartInfo("ParrotLoader.exe", $"\"{_gameLocation}\"");
                info.UseShellExecute = false;
                info.WindowStyle = ProcessWindowStyle.Normal;
                var process = Process.Start(info);
                while (!process.HasExited)
                {
                    // We only resurrect this since I had no crashes ever in the other threads. Feel free to improve!
                    if (directInputThreadP1 != null && !directInputThreadP1.IsAlive)
                    {
                        directInputThreadP1 = CreateDirectInputThread(_settingsData.PlayerOneGuid, 1, directInputListener);
                    }

                    if (directInputThreadP2 != null && !directInputThreadP2.IsAlive)
                    {
                        directInputThreadP2 = CreateDirectInputThread(_settingsData.PlayerTwoGuid, 2, directInputListener);
                    }
                    Thread.Sleep(5000);
                }
                _serialPortHandler.KillMe = true;
                directInputListener.KillMe = true;
                _gameRunning = false;
                Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(this.Close));
                if (_settingsData.UseKeyboard) kc.Unsubscribe();
            });
            gameThread.Start();
        }

        /// <summary>
        /// Creates DirectInput thread.
        /// </summary>
        /// <param name="joystickGuid">Joysticks GUID.</param>
        /// <param name="playerNumber">Player number.</param>
        /// <param name="directInputListener">Direct Input listener class.</param>
        /// <returns>Thread id.</returns>
        private Thread CreateDirectInputThread(Guid joystickGuid, int playerNumber, DirectInputListener directInputListener)
        {
            if (joystickGuid == Guid.Empty)
                return null;
            var dinputThread = new Thread(() => directInputListener.Listen(joystickGuid, playerNumber, _settingsData.UseSto0zDrivingHack));
            dinputThread.Start();
            return dinputThread;
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
