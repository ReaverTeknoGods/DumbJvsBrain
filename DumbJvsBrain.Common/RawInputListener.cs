using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
namespace DumbJvsBrain.Common
{
    public class RawInputListener
    {
        private int oldX;
        private int oldY;
        private IMouseEvents mouseEvents;
        private IKeyboardMouseEvents _mGlobalHook;
        private int _windowHeight;
        private int _windowWidth;
        private int _windowLocationX;
        private int _windowLocationY;
        private bool _windowFound;
        private bool _killListen;
        private Thread _listenThread;
        private int _mouseX;
        private int _mouseY;
        private bool _reverseAxis;

        private IntPtr GetWindowInformation()
        {
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains("teaGfx DirectX Release") || pList.MainWindowTitle.Contains("gamewin") || pList.MainWindowTitle.Contains("Ring Gun(NNDXG20-WIN)"))
                {
                    return pList.MainWindowHandle;
                }
                //if(!string.IsNullOrWhiteSpace(pList.MainWindowTitle))
                //    Console.WriteLine(pList.MainWindowTitle);
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        static extern bool ClipCursor(ref RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        static extern long ReleaseCapture();

        private void ListenThread()
        {
            while (!_killListen)
            {
                if (!_windowFound)
                    continue;
                var width = _windowWidth;
                var height = _windowHeight;
                var minX = _windowLocationX;
                var minY = _windowLocationY;
                var xArgs = CleanMouse(_mouseX - minX);
                var yArgs = CleanMouse(_mouseY - minY);
                if (yArgs < 0)
                    yArgs = 0;
                if (xArgs < 0)
                    xArgs = 0;
                var x = (ushort)(xArgs / (width / 255));
                var y = (ushort)(yArgs / (height / 255));
                if (_reverseAxis)
                {
                    InputCode.AnalogBytes[0] = (byte)~Cleanup(x);
                    InputCode.AnalogBytes[2] = (byte)~Cleanup(y);
                }
                else
                {
                    InputCode.AnalogBytes[2] = Cleanup(x);
                    InputCode.AnalogBytes[0] = Cleanup(y);
                }
                Thread.Sleep(10);
            }
        }

        public void ListenToDevice(bool reversedAxis)
        {
            _reverseAxis = reversedAxis;
            _mGlobalHook = Hook.GlobalEvents();
            _mGlobalHook.KeyDown += MGlobalHookOnKeyDown;
            _mGlobalHook.KeyUp += MGlobalHookOnKeyUp;
            mouseEvents = Hook.GlobalEvents();
            mouseEvents.MouseMove += MouseEventsOnMouseMove;
            mouseEvents.MouseDown += MouseEventOnMouseDown;
            mouseEvents.MouseUp += MouseEventsOnMouseUp;
            _killListen = false;
            _listenThread = new Thread(ListenThread);
            _listenThread.Start();
        }

        private void MGlobalHookOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
                SetPlayerButton(keyEventArgs.KeyCode, true);
        }

        private void MGlobalHookOnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
                SetPlayerButton(keyEventArgs.KeyCode, false);
        }

        void SetPlayerButton(Keys key, bool pressed)
        {
            if (key == Keys.D8)
            {
                InputCode.PlayerOneButtons.Test = pressed;
            }
            if (key == Keys.D9)
            {
                InputCode.PlayerOneButtons.Service = pressed;
            }
            if (key == Keys.D0)
            {
                InputCode.PlayerTwoButtons.Service = pressed;
            }
        }

        private void MouseEventOnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            if ((mouseEventArgs.Button & MouseButtons.Left) != 0)
            {
                InputCode.PlayerOneButtons.Button1 = true;
            }
            if ((mouseEventArgs.Button & MouseButtons.Right) != 0)
            {
                InputCode.PlayerOneButtons.Button2 = true;
                InputCode.PlayerOneButtons.Start = true;
            }
        }

        private void MouseEventsOnMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            if ((mouseEventArgs.Button & MouseButtons.Left) != 0)
            {
                InputCode.PlayerOneButtons.Button1 = false;
            }
            if ((mouseEventArgs.Button & MouseButtons.Right) != 0)
            {
                InputCode.PlayerOneButtons.Button2 = false;
                InputCode.PlayerOneButtons.Start = false;
            }
        }

        private void MouseEventsOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!_windowFound)
            {
                var ptr = GetWindowInformation();
                if (ptr != IntPtr.Zero)
                {
                    RECT rct = new RECT();
                    GetWindowRect(ptr, ref rct);
                    _windowHeight = rct.Bottom - rct.Top;
                    _windowWidth = rct.Right - rct.Left;
                    _windowLocationX = rct.Top;
                    _windowLocationY = rct.Left;
                    ClipCursor(ref rct);
                    _windowFound = true;
                }
                return;
            }
            _mouseX = mouseEventArgs.X;
            _mouseY = mouseEventArgs.Y;
        }

        private int CleanMouse(int mouseLocation)
        {
            if (mouseLocation < 0)
                return 0;
            return mouseLocation;
        }

        private byte Cleanup(ushort value)
        {
            if (value > 0xFF)
            {
                value = 0x00;
            }
            if (value < 0)
            {
                value = 0xFF;
            }
            value = (ushort) ~value;
            return (byte)value;
        }


        public void StopListening()
        {
            mouseEvents.MouseMove -= MouseEventsOnMouseMove;
            ReleaseCapture();
            _killListen = true;
        }
        
    }
}
