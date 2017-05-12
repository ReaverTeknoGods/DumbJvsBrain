using System;
using System.Linq;
using System.Threading;

namespace DirtyJvsBrain
{
    internal class Program
    {
        /// <summary>
        /// Duh the main.
        /// </summary>
        /// <param name="args">Command line parameters.</param>
        static void Main(string[] args)
        {
            try
            {
                var directInputListener = new DirectInputListener();
                Console.WriteLine("TeknoGods Presents Dirty JVS Brain 0.2");

                if (args.Length != 2)
                {
                    PrintHelp();
                    return;
                }

                if (args[0] == "cmd")
                {
                    CheckCommandParameter(args[1], directInputListener);
                    return;
                }

                if (!CheckGameModeParameter(args[0]))
                    return;

                if (!CheckComPort(args[1]))
                    return;

                Console.WriteLine("Launching DirectInput controller listeners");
                Console.WriteLine($"Listening at {args[1].ToUpper()}");
                Console.WriteLine("Use CTRL+C to quit!");

                if (!ReadJoystickGuids()) return;

                var directInputThreadP1 = CreateDirectInputThread(InputCode.JoystickData.PlayerOneGuid, 1, directInputListener);

                // Wait before launching second thread.
                Thread.Sleep(1000);
                var directInputThreadP2 = CreateDirectInputThread(InputCode.JoystickData.PlayerTwoGuid, 2, directInputListener);
                LaunchJvsEmulator(args[1]);

                while (true)
                {
                    // We only resurrect this since I had no crashes ever in the other threads. Feel free to improve!
                    if (directInputThreadP1 != null && !directInputThreadP1.IsAlive)
                    {
                        directInputThreadP1 = CreateDirectInputThread(InputCode.JoystickData.PlayerOneGuid, 1, directInputListener);
                    }

                    if (directInputThreadP2 != null && !directInputThreadP2.IsAlive)
                    {
                        directInputThreadP2 = CreateDirectInputThread(InputCode.JoystickData.PlayerTwoGuid, 2, directInputListener);
                    }

                    Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Checks that com port exists.
        /// </summary>
        /// <param name="portName">The com port name.</param>
        /// <returns></returns>
        private static bool CheckComPort(string portName)
        {
            var ports = System.IO.Ports.SerialPort.GetPortNames();
            if (!ports.Contains(portName.ToUpper()))
            {
                Console.WriteLine($"Cannot find given serial port: {portName.ToUpper()}");
                Console.WriteLine("Available ports: ");
                foreach (var port in ports)
                {
                    Console.WriteLine(port);
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks the game mode.
        /// </summary>
        /// <param name="parameter">Given game mode.</param>
        /// <returns></returns>
        private static bool CheckGameModeParameter(string parameter)
        {
            int gameSelection = 0;
            var last = (int)Enum.GetValues(typeof(GameSelection)).Cast<GameSelection>().Last();
            if (!int.TryParse(parameter, out gameSelection))
            {
                PrintGameModes(last);
                return false;
            }

            if (gameSelection > last)
            {
                PrintGameModes(last);
                return false;
            }
            InputCode.ButtonMode = (GameSelection)gameSelection;
            return true;
        }

        /// <summary>
        /// Checks cmd commandline parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="directInputListener">DirectInput listener.</param>
        private static void CheckCommandParameter(string parameter, DirectInputListener directInputListener)
        {
            switch (parameter.ToLower())
            {
                case "getjoysticks":
                    directInputListener.ListJoysticks();
                    break;
                case "generatejoystickdata":
                    WriteJoystickGuidFile();
                    break;
                default:
                    Console.WriteLine("Unknown cmd.");
                    break;
            }
        }

        /// <summary>
        /// Prints help.
        /// </summary>
        private static void PrintHelp()
        {
            var last = (int)Enum.GetValues(typeof(GameSelection)).Cast<GameSelection>().Last();
            Console.WriteLine("Usage for emulation: DirtyJvsBrain.exe <mode> <serialPort>");
            Console.WriteLine("Example: DirtyJvsBrain.exe 1 COM10");
            Console.WriteLine("Usage for listing joystick GUID: DirtyJvsBrain.exe cmd getJoysticks");
            Console.WriteLine("Usage for listing joystick GUID: DirtyJvsBrain.exe cmd generateJoystickData");
            PrintGameModes(last);
            return;
        }

        /// <summary>
        /// Launches the jvs emulator.
        /// </summary>
        /// <param name="serialPort">Serial port to listen.</param>
        private static void LaunchJvsEmulator(string serialPort)
        {
            var serialPortHandler = new SerialPortHandler();
            var jvsThread = new Thread(() => serialPortHandler.ListenSerial(serialPort));
            jvsThread.Start();
            var processQueueThread = new Thread(serialPortHandler.ProcessQueue);
            processQueueThread.Start();
        }

        /// <summary>
        /// Reads joystick guids from xml file.
        /// </summary>
        /// <returns>If success, true.</returns>
        private static bool ReadJoystickGuids()
        {
            try
            {
                ReadJoystickGuidFile();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading Joystick Data, please generate new one with: cmd generateJoystickdata");
                Console.WriteLine(e);
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
            var dinputThread = new Thread(() => directInputListener.Listen(joystickGuid, playerNumber));
            dinputThread.Start();
            return dinputThread;
        }

        /// <summary>
        /// Reads joystick.xml
        /// </summary>
        private static void ReadJoystickGuidFile()
        {
            InputCode.JoystickData = JoystickHelper.DeSerialize();
        }
        /// <summary>
        /// Writes joystick.xml
        /// </summary>
        private static void WriteJoystickGuidFile()
        {
            Console.WriteLine("Generating new joystick.xml");
            JoystickHelper.Serialize(new JoystickData());
            Console.WriteLine("Done");
        }

        /// <summary>
        /// Prints available game mods.
        /// </summary>
        /// <param name="gameModeCount">Maximum game mode count.</param>
        private static void PrintGameModes(int gameModeCount)
        {
            Console.WriteLine("Please insert proper game mode.");
            Console.WriteLine("Modes: ");
            for (var i = 0; i <= gameModeCount; i++)
            {
                Console.WriteLine($"Mode Number: {i}, GameName: {((GameSelection)(i)).ToDescription()}");
            }
            Console.ReadKey();
        }
    }
}
