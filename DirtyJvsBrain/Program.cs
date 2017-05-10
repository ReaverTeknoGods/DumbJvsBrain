using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace DirtyJvsBrain
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TeknoGods Presents Dirty JVS Brain 0.1");
            var last = (int)Enum.GetValues(typeof(GameSelection)).Cast<GameSelection>().Last();
            var ports = System.IO.Ports.SerialPort.GetPortNames();

            if (args.Length != 2)
            {
                Console.WriteLine("Usage: DirtyJvsBrain.exe <mode> <serialPort>");
                Console.WriteLine("Example: DirtyJvsBrain.exe 1 COM10");
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

            if (!ports.Contains(args[1].ToUpper()))
            {
                Console.WriteLine($"Cannot find given serial port: {args[1].ToUpper()}");
                Console.WriteLine("Available ports: ");
                foreach (var port in ports)
                {
                    Console.WriteLine(port);
                }
                return;
            }

            InputCode.ButtonMode = (GameSelection) gameSelection;
            var x360Listener = new X360Listener();
            var serialPortHandler = new SerialPortHandler();

            // NOTE: CODE IS VERY UGLY AND DIRTY. Don't cry tonight baby.
            Console.WriteLine("Launching XBOX360/ONE controller listener");
            Console.WriteLine($"Listening at {args[1].ToUpper()}");
            Console.WriteLine("Use CTRL+C to quit!");

            var x360Thread = new Thread(x360Listener.Listen);
            x360Thread.Start();
            var jvsThread = new Thread(() => serialPortHandler.ListenSerial(args[1]));
            jvsThread.Start();
            var processQueueThread = new Thread(serialPortHandler.ProcessQueue);
            processQueueThread.Start();

            while (true)
            {
                // We only resurrect this since I had no crashes ever in the other threads. Feel free to improve!
                if (!x360Thread.IsAlive)
                {
                    Console.WriteLine("Xbox360 thread was terminated, relaunching!");
                    x360Thread = new Thread(x360Listener.Listen);
                }
                Thread.Sleep(5000);
            }
        }

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
