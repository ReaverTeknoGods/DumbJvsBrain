using System.IO;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace DumbJvsBrain.Common
{
    public class KeyboardController
    {
        private IKeyboardMouseEvents _mGlobalHook;
        private bool _hookPlayer1;
        private bool _hookPlayer2;
        private bool _useCustomKeys;
        private KeyboardMap _kbMap;

        public void Subscribe(bool hookPlayer1, bool hookPlayer2)
        {
            _hookPlayer1 = hookPlayer1;
            _hookPlayer2 = hookPlayer2;
            _mGlobalHook = Hook.GlobalEvents();
            _mGlobalHook.KeyDown += MGlobalHookOnKeyDown;
            _mGlobalHook.KeyUp += MGlobalHookOnKeyUp;

            if (!File.Exists("KeyboardMap.xml"))
            {
                _useCustomKeys = false;
                return;
            }
            _kbMap = KeyboardHelper.DeSerialize();
            _useCustomKeys = true;
        }

        private void MGlobalHookOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (!_useCustomKeys)
                SetPlayerButton(keyEventArgs.KeyCode, true);
            else
                SetPlayerCustomButton((int)keyEventArgs.KeyCode, true);
        }

        private void MGlobalHookOnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if(!_useCustomKeys)
                SetPlayerButton(keyEventArgs.KeyCode, false);
            else
                SetPlayerCustomButton((int)keyEventArgs.KeyCode, false);
        }

        private void SetPlayerCustomButton(int key, bool pressed)
        {
            if (_hookPlayer1)
            {
                if (key == _kbMap.P1Start)
                {
                    InputCode.PlayerOneButtons.Start = pressed;
                }
                if (key == _kbMap.P1Left)
                {
                    //InputCode.AnalogBytes[0] = (byte) (pressed ? 0x20 : 0x7F);
                    InputCode.PlayerOneButtons.Left = pressed;
                }
                if (key == _kbMap.P1Right)
                {
                    //InputCode.AnalogBytes[0] = (byte) (pressed ? 0xD0 : 0x7F);
                    InputCode.PlayerOneButtons.Right = pressed;
                }
                if (key == _kbMap.P1Up)
                {
                    //InputCode.AnalogBytes[2] = (byte) (pressed ? 0xFF : 0x00);
                    InputCode.PlayerOneButtons.Up = pressed;
                }
                if (key == _kbMap.P1Down)
                {
                    //InputCode.AnalogBytes[4] = (byte) (pressed ? 0xFF : 0x00);
                    InputCode.PlayerOneButtons.Down = pressed;
                }
                if (key == _kbMap.P1B1)
                {
                    InputCode.PlayerOneButtons.Button1 = pressed;
                }
                if (key == _kbMap.P1B2)
                {
                    InputCode.PlayerOneButtons.Button2 = pressed;
                }
                if (key == _kbMap.P1B3)
                {
                    InputCode.PlayerOneButtons.Button3 = pressed;
                }
                if (key == _kbMap.P1B4)
                {
                    InputCode.PlayerOneButtons.Button4 = pressed;
                }
                if (key == _kbMap.P1B5)
                {
                    InputCode.PlayerOneButtons.Button5 = pressed;
                }
                if (key == _kbMap.P1B6)
                {
                    InputCode.PlayerOneButtons.Button6 = pressed;
                }

            }
            if (_hookPlayer2)
            {
                if (key == _kbMap.P2Start)
                {
                    InputCode.PlayerTwoButtons.Start = pressed;
                }
                if (key == _kbMap.P2Left)
                {
                    InputCode.PlayerTwoButtons.Left = pressed;
                }
                if (key == _kbMap.P2Right)
                {
                    InputCode.PlayerTwoButtons.Right = pressed;
                }
                if (key == _kbMap.P2Up)
                {
                    InputCode.PlayerTwoButtons.Up = pressed;
                }
                if (key == _kbMap.P2Down)
                {
                    InputCode.PlayerTwoButtons.Down = pressed;
                }
                if (key == _kbMap.P2B1)
                {
                    InputCode.PlayerTwoButtons.Button1 = pressed;
                }
                if (key == _kbMap.P2B2)
                {
                    InputCode.PlayerTwoButtons.Button2 = pressed;
                }
                if (key == _kbMap.P2B3)
                {
                    InputCode.PlayerTwoButtons.Button3 = pressed;
                }
                if (key == _kbMap.P2B4)
                {
                    InputCode.PlayerTwoButtons.Button4 = pressed;
                }
                if (key == _kbMap.P2B5)
                {
                    InputCode.PlayerTwoButtons.Button5 = pressed;
                }
                if (key == _kbMap.P2B6)
                {
                    InputCode.PlayerTwoButtons.Button6 = pressed;
                }
            }
            if (key == _kbMap.TestSw)
            {
                InputCode.PlayerOneButtons.Test = pressed;
            }
            if (key == _kbMap.P1Service)
            {
                InputCode.PlayerOneButtons.Service = pressed;
            }
            if (key == _kbMap.P2Service)
            {
                InputCode.PlayerTwoButtons.Service = pressed;
            }
        }

        void SetPlayerButton(Keys key, bool pressed)
        {
            if (_hookPlayer1)
            {
                if (key == Keys.D1)
                {
                    InputCode.PlayerOneButtons.Start = pressed;
                }
                if (key == Keys.Left)
                {
                    //InputCode.AnalogBytes[0] = (byte)(pressed ? 0x20 : 0x7F);
                    InputCode.PlayerOneButtons.Left = pressed;
                }
                if (key == Keys.Right)
                {
                    //InputCode.AnalogBytes[0] = (byte)(pressed ? 0xD0 : 0x7F);
                    InputCode.PlayerOneButtons.Right = pressed;
                }
                if (key == Keys.Up)
                {
                    //InputCode.AnalogBytes[2] = (byte)(pressed ? 0xFF : 0x00);
                    InputCode.PlayerOneButtons.Up = pressed;
                }
                if (key == Keys.Down)
                {
                    //InputCode.AnalogBytes[4] = (byte)(pressed ? 0xFF : 0x00);
                    InputCode.PlayerOneButtons.Down = pressed;
                }
                if (key == Keys.Insert)
                {
                    InputCode.PlayerOneButtons.Button1 = pressed;
                }
                if (key == Keys.Home)
                {
                    InputCode.PlayerOneButtons.Button2 = pressed;
                }
                if (key == Keys.PageUp)
                {
                    InputCode.PlayerOneButtons.Button3 = pressed;
                }
                if (key == Keys.Delete)
                {
                    InputCode.PlayerOneButtons.Button4 = pressed;
                }
                if (key == Keys.End)
                {
                    InputCode.PlayerOneButtons.Button5 = pressed;
                }
                if (key == Keys.PageDown)
                {
                    InputCode.PlayerOneButtons.Button6 = pressed;
                }

            }
            if (_hookPlayer2)
            {
                if (key == Keys.D2)
                {
                    InputCode.PlayerTwoButtons.Start = pressed;
                }
                if (key == Keys.A)
                {
                    InputCode.PlayerTwoButtons.Left = pressed;
                }
                if (key == Keys.D)
                {
                    InputCode.PlayerTwoButtons.Right = pressed;
                }
                if (key == Keys.W)
                {
                    InputCode.PlayerTwoButtons.Up = pressed;
                }
                if (key == Keys.S)
                {
                    InputCode.PlayerTwoButtons.Down = pressed;
                }
                if (key == Keys.T)
                {
                    InputCode.PlayerTwoButtons.Button1 = pressed;
                }
                if (key == Keys.Y)
                {
                    InputCode.PlayerTwoButtons.Button2 = pressed;
                }
                if (key == Keys.U)
                {
                    InputCode.PlayerTwoButtons.Button3 = pressed;
                }
                if (key == Keys.G)
                {
                    InputCode.PlayerTwoButtons.Button4 = pressed;
                }
                if (key == Keys.H)
                {
                    InputCode.PlayerTwoButtons.Button5 = pressed;
                }
                if (key == Keys.J)
                {
                    InputCode.PlayerTwoButtons.Button6 = pressed;
                }
            }
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

        public void Unsubscribe()
        {
            _mGlobalHook.KeyDown -= MGlobalHookOnKeyDown;
            _mGlobalHook.KeyUp -= MGlobalHookOnKeyUp;

            //It is recommened to dispose it
            _mGlobalHook.Dispose();
        }
    }
}
