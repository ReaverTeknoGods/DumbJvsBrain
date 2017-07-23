using System;
using System.IO;
using System.Linq;
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
        /// <param name="joystickMapping">Joystick map.</param>
        /// <returns>Thread id.</returns>
        private static Thread CreateDirectInputThread(Guid joystickGuid, int playerNumber, DirectInputListener directInputListener, JoystickMapping joystickMapping)
        {
            if (joystickGuid == Guid.Empty)
                return null;
            var dinputThread = new Thread(() => directInputListener.Listen(joystickGuid, playerNumber, _settingsData.UseSto0ZDrivingHack, joystickMapping));
            dinputThread.Start();
            return dinputThread;
        }

        static void Main(string[] args)
        {
            if (!LoadSettingsData())
                return;
            var last = (int) Enum.GetValues(typeof(GameProfiles)).Cast<GameProfiles>().Last();
            if (args.Length != 1)
            {
                PrintGameModes(last);
                return;
            }

            int gameSelection;
            if (!int.TryParse(args[0], out gameSelection))
            {
                PrintGameModes(last);
                return;
            }

            if (gameSelection > last)
            {
                PrintGameModes(last);
                return;
            }

            JoystickMapping jmap1 = null;
            JoystickMapping jmap2 = null;

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
                Console.WriteLine($"Loading joystick mapping failed with error: {ex.InnerException} {ex.Message}");
                return;
            }

            if (jmap1 == null)
                jmap1 = new JoystickMapping();

            if (jmap2 == null)
                jmap2 = new JoystickMapping();

            InputCode.ButtonMode = (GameProfiles)gameSelection;
            var _serialPortHandler = new SerialPortHandler();
            var directInputListener = new DirectInputListener();
            KeyboardController kc = new KeyboardController();
            var jvsThread = new Thread(() => _serialPortHandler.ListenSerial("COM14"));
            jvsThread.Start();
            var processQueueThread = new Thread(_serialPortHandler.ProcessQueue);
            processQueueThread.Start();
            var directInputThreadP1 = CreateDirectInputThread(_settingsData.PlayerOneGuid, 1, directInputListener, jmap1);
            // Wait before launching second thread.
            Thread.Sleep(1000);
            var directInputThreadP2 = CreateDirectInputThread(_settingsData.PlayerTwoGuid, 2, directInputListener, jmap2);
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
                        directInputThreadP1 = CreateDirectInputThread(_settingsData.PlayerOneGuid, 1, directInputListener, jmap1);
                    }

                    if (directInputThreadP2 != null && !directInputThreadP2.IsAlive)
                    {
                        directInputThreadP2 = CreateDirectInputThread(_settingsData.PlayerTwoGuid, 2, directInputListener, jmap2);
                    }
                    Thread.Sleep(5000);
                }
            });
            gameThread.Start();
            while (gameThread.IsAlive)
                Thread.Sleep(1000);
        }

        private static void PrintGameModes(int gameModeCount)
        {
            Console.WriteLine("Please insert proper game mode.");
            Console.WriteLine("Modes: ");
            for (var i = 0; i <= gameModeCount; i++)
            {
                Console.WriteLine($"Mode Number: {i}, GameName: {((GameProfiles)(i)).ToDescription()}");
            }
            Console.ReadKey();
        }
    }
}
