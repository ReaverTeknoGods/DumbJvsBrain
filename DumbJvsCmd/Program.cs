using System;
using System.IO;
using System.Threading;
using DumbJvsBrain.Common;

namespace DumbJvsCmd
{
    class Program
    {
        private static SettingsData _settingsData;
        /// <summary>
        /// Loads the settings data file.
        /// </summary>
        private static bool LoadSettingsData()
        {
            try
            {
                if (!File.Exists("SettingsData.xml"))
                {
                    Console.WriteLine("Please use the UI to set config!");
                    return false;
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
                Console.WriteLine($"Exception happened during loading SettingsData.xml! Generate new one by saving!{Environment.NewLine}{Environment.NewLine}{e}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates DirectInput thread.
        /// </summary>
        /// <param name="joystickGuid">Joysticks GUID.</param>
        /// <param name="playerNumber">Player number.</param>
        /// <param name="directInputListener">Direct Input listener class.</param>
        /// <returns>Thread id.</returns>
        private static Thread CreateDirectInputThread(Guid joystickGuid, int playerNumber, DirectInputListener directInputListener)
        {
            if (joystickGuid == Guid.Empty)
                return null;
            var dinputThread = new Thread(() => directInputListener.Listen(joystickGuid, playerNumber, _settingsData.UseSto0ZDrivingHack));
            dinputThread.Start();
            return dinputThread;
        }

        static void Main(string[] args)
        {
            if (!LoadSettingsData())
                return;
            var _serialPortHandler = new SerialPortHandler();
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
            if (_settingsData.UseKeyboard)
            {
                kc.Subscribe(directInputThreadP1 == null, directInputThreadP2 == null);
            }
            var gameThread = new Thread(() =>
            {
                while (true)
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
            });
            gameThread.Start();
            while (gameThread.IsAlive)
                Thread.Sleep(1000);
        }
    }
}
