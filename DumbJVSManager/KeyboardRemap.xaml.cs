using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Gma.System.MouseKeyHook;
using System.Windows.Input;
using DumbJvsBrain.Common;
using MessageBox = System.Windows.MessageBox;

namespace DumbJVSManager
{
    /// <summary>
    /// Interaction logic for KeyboardRemap.xaml
    /// </summary>
    public partial class KeyboardRemap : Window
    {
        private IKeyboardMouseEvents _mGlobalHook;
        public KeyboardRemap()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Saves keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSaveKeys(object sender, RoutedEventArgs e)
        {
            try
            {
                var kbMap = new KeyboardMap
                {
                    P1B1 = GetPressedKey(TxtP1B1.Tag),
                    P1B2 = GetPressedKey(TxtP1B2.Tag),
                    P1B3 = GetPressedKey(TxtP1B3.Tag),
                    P1B4 = GetPressedKey(TxtP1B4.Tag),
                    P1B5 = GetPressedKey(TxtP1B5.Tag),
                    P1B6 = GetPressedKey(TxtP1B6.Tag),
                    P1Up = GetPressedKey(TxtP1Up.Tag),
                    P1Down = GetPressedKey(TxtP1Down.Tag),
                    P1Left = GetPressedKey(TxtP1Left.Tag),
                    P1Right = GetPressedKey(TxtP1Right.Tag),
                    P1Service = GetPressedKey(TxtP1Service.Tag),
                    P1Start = GetPressedKey(TxtP1Start.Tag),
                    P2B1 = GetPressedKey(TxtP2B1.Tag),
                    P2B2 = GetPressedKey(TxtP2B2.Tag),
                    P2B3 = GetPressedKey(TxtP2B3.Tag),
                    P2B4 = GetPressedKey(TxtP2B4.Tag),
                    P2B5 = GetPressedKey(TxtP2B5.Tag),
                    P2B6 = GetPressedKey(TxtP2B6.Tag),
                    P2Up = GetPressedKey(TxtP2Up.Tag),
                    P2Down = GetPressedKey(TxtP2Down.Tag),
                    P2Left = GetPressedKey(TxtP2Left.Tag),
                    P2Right = GetPressedKey(TxtP2Right.Tag),
                    P2Service = GetPressedKey(TxtP2Service.Tag),
                    P2Start = GetPressedKey(TxtP2Start.Tag),
                    TestSw = GetPressedKey(TxtTestSw.Tag)
                };
                KeyboardHelper.Serialize(kbMap);
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error while saving: {exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        /// <summary>
        /// Gets pressed key from tag of the text box.
        /// </summary>
        /// <param name="value">Pressed key.</param>
        /// <returns>Pressed key.</returns>

        private int GetPressedKey(object value)
        {
            if (value == null)
                return 0;
            if (value.GetType() == typeof(System.Windows.Forms.Keys))
            {
                var parsedValue = (int)(System.Windows.Forms.Keys)value;
                return parsedValue;
            }
            return 0;
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void KeyboardRemap_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _mGlobalHook = Hook.GlobalEvents();
                _mGlobalHook.KeyDown += GlobalHookKeyDown;
                if (!File.Exists("KeyboardMap.xml"))
                    return;
                var kbMap = KeyboardHelper.DeSerialize();
                SetTextBoxText(kbMap.P1B1, TxtP1B1);
                SetTextBoxText(kbMap.P1B2, TxtP1B2);
                SetTextBoxText(kbMap.P1B3, TxtP1B3);
                SetTextBoxText(kbMap.P1B4, TxtP1B4);
                SetTextBoxText(kbMap.P1B5, TxtP1B5);
                SetTextBoxText(kbMap.P1B6, TxtP1B6);
                SetTextBoxText(kbMap.P2B1, TxtP2B1);
                SetTextBoxText(kbMap.P2B2, TxtP2B2);
                SetTextBoxText(kbMap.P2B3, TxtP2B3);
                SetTextBoxText(kbMap.P2B4, TxtP2B4);
                SetTextBoxText(kbMap.P2B5, TxtP2B5);
                SetTextBoxText(kbMap.P2B6, TxtP2B6);
                SetTextBoxText(kbMap.P1Up, TxtP1Up);
                SetTextBoxText(kbMap.P1Down, TxtP1Down);
                SetTextBoxText(kbMap.P1Left, TxtP1Left);
                SetTextBoxText(kbMap.P1Right, TxtP1Right);
                SetTextBoxText(kbMap.P1Start, TxtP1Start);
                SetTextBoxText(kbMap.P2Up, TxtP2Up);
                SetTextBoxText(kbMap.P2Down, TxtP2Down);
                SetTextBoxText(kbMap.P2Left, TxtP2Left);
                SetTextBoxText(kbMap.P2Right, TxtP2Right);
                SetTextBoxText(kbMap.P2Start, TxtP2Start);
                SetTextBoxText(kbMap.P1Service, TxtP1Service);
                SetTextBoxText(kbMap.P2Service, TxtP2Service);
                SetTextBoxText(kbMap.TestSw, TxtTestSw);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error while loading: {exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Sets text box text and tag.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="txtBox"></param>
        private void SetTextBoxText(int key, TextBox txtBox)
        {
            txtBox.Text = ((System.Windows.Forms.Keys)key).ToString();
            txtBox.Tag = (System.Windows.Forms.Keys)key;
        }

        /// <summary>
        /// Global Hook Key Down event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalHookKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            var txtControl = GetActiveTextBox();
            if (txtControl == null)
                return;

            txtControl.Text = e.KeyCode.ToString();
            txtControl.Tag = e.KeyCode;
        }

        /// <summary>
        /// Closing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyboardRemap_OnClosing(object sender, CancelEventArgs e)
        {
            _mGlobalHook.KeyDown -= GlobalHookKeyDown;
        }

        /// <summary>
        /// Gets active text box.
        /// </summary>
        /// <returns></returns>
        private TextBox GetActiveTextBox()
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(this);
            if (focusedControl == null)
                return null;
            if(focusedControl.GetType() == typeof(TextBox))
                return (TextBox) focusedControl;
            return null;
        }
    }
}
