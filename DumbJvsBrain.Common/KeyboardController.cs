using System;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace DumbJvsBrain.Common
{
    public class KeyboardController
    {
        private IKeyboardMouseEvents m_GlobalHook;
        private bool _hookPlayer1;
        private bool _hookPlayer2;

        public void Subscribe(bool hookPlayer1, bool hookPlayer2)
        {
            _hookPlayer1 = hookPlayer1;
            _hookPlayer2 = hookPlayer2;
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();

            //m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.KeyDown += MGlobalHookOnKeyDown;
            //m_GlobalHook.KeyPress += GlobalHookKeyPress;
            m_GlobalHook.KeyUp += MGlobalHookOnKeyUp;
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
            if (_hookPlayer1)
            {
                if (key == Keys.D1)
                {
                    InputCode.PlayerOneButtons.Start = pressed;
                }
                if (key == Keys.Left)
                {
                    //InputCode.Wheel = pressed ? 0x20 : 0x7F;
                    InputCode.PlayerOneButtons.Left = pressed;
                }
                if (key == Keys.Right)
                {
                    //InputCode.Wheel = pressed ? 0xD0 : 0x7F;
                    InputCode.PlayerOneButtons.Right = pressed;
                }
                if (key == Keys.Up)
                {
                    //InputCode.Gas = pressed ? 0xFF : 0x00;
                    InputCode.PlayerOneButtons.Up = pressed;
                }
                if (key == Keys.Down)
                {
                    //InputCode.Brake = pressed ? 0xFF : 0x00;
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

        ///// <summary>
        ///// Hooks the keyboard.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        //{
        //}

        /// <summary>
        /// Hooks mouse, not needed until we have proper gun game support.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            //Console.WriteLine("MouseDown: \t{0}; \t System Timestamp: \t{1}", e.Button, e.Timestamp);

            // uncommenting the following line will suppress the middle mouse button click
            // if (e.Buttons == MouseButtons.Middle) { e.Handled = true; }
        }

        public void Unsubscribe()
        {
            //m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
            m_GlobalHook.KeyDown -= MGlobalHookOnKeyDown;
            //m_GlobalHook.KeyPress -= GlobalHookKeyPress;
            m_GlobalHook.KeyUp -= MGlobalHookOnKeyUp;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }
    }
}
