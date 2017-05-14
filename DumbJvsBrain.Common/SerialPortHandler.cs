using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace DumbJvsBrain.Common
{
    public class SerialPortHandler
    {
        private readonly Queue<byte> _recievedData = new Queue<byte>();
        private SerialPort _port;
        public bool KillMe { get; set; }
        //private readonly List<byte> _lastPackage = new List<byte>(); // This is for TESTING
        /// <summary>
        /// Process the queue, very dirty and hacky. Please improve.
        /// This is the stablest I got it, lot of research and development of different methods.
        /// </summary>
        public void ProcessQueue()
        {
            while (true)
            {
                if (KillMe)
                    return;
                var queue = new List<byte>();
                if (_recievedData.Count != 0)
                {
                    var f = _recievedData.Dequeue();
                    if (f == 0xE0)
                    {
                        var count = 0;
                        byte size = 0;
                        while (true)
                        {
                            if (KillMe)
                                return;
                            if (count == 0)
                            {
                                queue.Add(f);
                                count++;
                            }
                            else if (_recievedData.Count > 2 && count == 1)
                            {
                                queue.Add(_recievedData.Dequeue());
                                size = _recievedData.Dequeue();
                                queue.Add(size);
                                count++;
                            }
                            else if (count == 2 && _recievedData.Count >= size)
                            {
                                for (int i = 0; i < size; i++)
                                {
                                    queue.Add(_recievedData.Dequeue());
                                }
                                //_lastPackage.Clear();
                                //_lastPackage.AddRange(queue);
                                var reply = JvsPackageEmulator.GetReply(queue.ToArray());
                                if (reply.Length != 0)
                                    _port.Write(reply, 0, reply.Length);
                                break;
                            }
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Listen the serial port.
        /// </summary>
        /// <param name="port">Port name.</param>
        public void ListenSerial(string port)
        {
            KillMe = false;
            _port = new SerialPort(port)
            {
                BaudRate = 115200,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadTimeout = 0,
                WriteBufferSize = 516,
                ReadBufferSize = 516,
                Handshake = Handshake.None
            };

            _port.DataReceived += delegate (object sender, SerialDataReceivedEventArgs args)
            {
                var sp = (SerialPort)sender;
                var data = new byte[sp.BytesToRead];
                var r = sp.Read(data, 0, data.Length);
                for (var i = 0; i < r; i++)
                    _recievedData.Enqueue(data[i]);
            };

            _port.Open();
            while (_port.IsOpen)
            {
                if (KillMe)
                {
                    _port.Close();
                    break;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
