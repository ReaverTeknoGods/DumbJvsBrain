using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DumbJvsBrain.Common;
using SharpDX.XInput;

namespace DumbJVSManager
{
    /// <summary>
    /// Interaction logic for DirectInputRemap.xaml
    /// </summary>
    public partial class XInputRemap : Window
    {
        public XInputRemapViewModel ViewModel { get; set; }
        private bool stopListen;
        private bool showAllControls = true;
        private int _playerId;

        public XInputRemap(bool showExtras, int playerId)
        {
            ViewModel = new XInputRemapViewModel();
            DataContext = ViewModel;
            ViewModel.ExtrasVisible = showExtras;
            _playerId = playerId;
            InitializeComponent();
        }

        /// <summary>
        /// Listens given joystick.
        /// </summary>
        /// <param name="joystickIndex">Joystick number</param>
        public void Listen(UserIndex joystickIndex)
        {
            var controller = new Controller(joystickIndex);
            if (!controller.IsConnected)
                return;
            var previousState = controller.GetState();
            try
            {
                while (!stopListen)
                {
                    var state = controller.GetState();
                    if (previousState.PacketNumber != state.PacketNumber)
                    {
                        SetTextBoxText(state, previousState);
                    }
                    Thread.Sleep(10);
                    previousState = state;
                }
            }
            catch (Exception e)
            {
                    
            }
        }

        private void BtnSaveKeys(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_playerId == 1)
                {
                    // Save player 1 xml
                    var map = new XInputMapping()
                    {
                        Button1 = GetTagButtonInfo(TxtButton1),
                        Button2 = GetTagButtonInfo(TxtButton2),
                        Button3 = GetTagButtonInfo(TxtButton3),
                        Button4 = GetTagButtonInfo(TxtButton4),
                        Button5 = GetTagButtonInfo(TxtButton5),
                        Button6 = GetTagButtonInfo(TxtButton6),
                        Start = GetTagButtonInfo(TxtStart),
                        Test = GetTagButtonInfo(TxtTestSw),
                        Service = GetTagButtonInfo(TxtService),
                        Up = GetTagButtonInfo(TxtUp),
                        Down = GetTagButtonInfo(TxtDown),
                        Left = GetTagButtonInfo(TxtLeft),
                        Right = GetTagButtonInfo(TxtRight),
                        GunUp = GetTagButtonInfo(TxtLgiUp),
                        GunDown = GetTagButtonInfo(TxtLgiDown),
                        GunLeft = GetTagButtonInfo(TxtLgiLeft),
                        GunRight = GetTagButtonInfo(TxtLgiRight),
                        GunTrigger = GetTagButtonInfo(TxtGunTrigger),
                        SonicItem = GetTagButtonInfo(TxtSonicItemButton),
                        GasAxis = GetTagButtonInfo(TxtGas),
                        BrakeAxis = GetTagButtonInfo(TxtBrake),
                        WheelAxis = GetTagButtonInfo(TxtWheel),
                        SrcGearChange1 = GetTagButtonInfo(TxtSrcGearChange1),
                        SrcGearChange2 = GetTagButtonInfo(TxtSrcGearChange2),
                        SrcGearChange3 = GetTagButtonInfo(TxtSrcGearChange3),
                        SrcGearChange4 = GetTagButtonInfo(TxtSrcGearChange4),
                        SrcViewChange1 = GetTagButtonInfo(TxtSrcViewChange1),
                        SrcViewChange2 = GetTagButtonInfo(TxtSrcViewChange2),
                        SrcViewChange3 = GetTagButtonInfo(TxtSrcViewChange3),
                        SrcViewChange4 = GetTagButtonInfo(TxtSrcViewChange4),
                        InitialD6MenuDown = GetTagButtonInfo(TxtInitialD6MenuDown),
                        InitialD6MenuLeft = GetTagButtonInfo(TxtInitialD6MenuLeft),
                        InitialD6MenuRight = GetTagButtonInfo(TxtInitialD6MenuRight),
                        InitialD6MenuUp = GetTagButtonInfo(TxtInitialD6MenuUp),
                        InitialD6ShiftDown = GetTagButtonInfo(TxtInitialD6ShiftDown),
                        InitialD6ShiftUp = GetTagButtonInfo(TxtInitialD6ShiftUp),
                        InitialD6ViewChange = GetTagButtonInfo(TxtInitialD6ViewChange),
                    };
                    map.GunMultiplier = IUpDownMovementMultiplier.Value ?? 1;
                    JoystickHelper.SerializeXInput(map, 1);
                }

                if (_playerId == 2)
                {
                    // Save player 2 xml
                    var map = new XInputMapping()
                    {
                        Button1 = GetTagButtonInfo(TxtButton1),
                        Button2 = GetTagButtonInfo(TxtButton2),
                        Button3 = GetTagButtonInfo(TxtButton3),
                        Button4 = GetTagButtonInfo(TxtButton4),
                        Button5 = GetTagButtonInfo(TxtButton5),
                        Button6 = GetTagButtonInfo(TxtButton6),
                        Start = GetTagButtonInfo(TxtStart),
                        Test = GetTagButtonInfo(TxtTestSw),
                        Service = GetTagButtonInfo(TxtService),
                        Up = GetTagButtonInfo(TxtUp),
                        Down = GetTagButtonInfo(TxtDown),
                        Left = GetTagButtonInfo(TxtLeft),
                        Right = GetTagButtonInfo(TxtRight),
                        GunUp = GetTagButtonInfo(TxtLgiUp),
                        GunDown = GetTagButtonInfo(TxtLgiDown),
                        GunLeft = GetTagButtonInfo(TxtLgiLeft),
                        GunRight = GetTagButtonInfo(TxtLgiRight),
                        GunTrigger = GetTagButtonInfo(TxtGunTrigger)
                    };
                    map.GunMultiplier = IUpDownMovementMultiplier.Value ?? 1;
                    JoystickHelper.SerializeXInput(map, 2);
                }
                MessageBox.Show("Save Complete", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Saving failed with error: {ex.InnerException} {ex.Message}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            Close();
        }

        private XInputButton GetTagButtonInfo(TextBox txt)
        {
            return (XInputButton) txt.Tag;
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            Close();
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
            if (focusedControl.GetType() == typeof(TextBox))
                return (TextBox)focusedControl;
            return null;
        }

        private void GetAnalogXInput(short value, bool isLeftThumb, TextBox txt, bool isY)
        {
            var deadZone = isLeftThumb ? Gamepad.LeftThumbDeadZone : Gamepad.RightThumbDeadZone;
            XInputButton button = new XInputButton {IsButton = false};
            if (value > 0 + deadZone)
            {
                txt.Text = isLeftThumb ? "LeftThumb" : "RightThumb";
                if (isY)
                {
                    if (isLeftThumb)
                        button.IsLeftThumbY = true;
                    else
                        button.IsRightThumbY = true;
                    txt.Text += "Y+";
                }
                else
                {
                    if (isLeftThumb)
                        button.IsLeftThumbX = true;
                    else
                        button.IsRightThumbX = true;
                    button.IsAxisMinus = false;
                    txt.Text += "X+";
                }
                txt.Tag = button;
            }
            else if (value < 0 - deadZone)
            {
                txt.Text = isLeftThumb ? "LeftThumb" : "RightThumb";
                button.IsAxisMinus = true;
                if (isY)
                {
                    if (isLeftThumb)
                        button.IsLeftThumbY = true;
                    else
                        button.IsRightThumbY = true;
                    txt.Text += "Y-";
                    txt.Tag = button;
                }
                else
                {
                    if (isLeftThumb)
                        button.IsLeftThumbX = true;
                    else
                        button.IsRightThumbX = true;
                    txt.Text += "X-";
                    txt.Tag = button;
                }
            }
        }

        private void HandleButton(GamepadButtonFlags buttonFlag, TextBox txt)
        {
            var button = new XInputButton
            {
                IsButton = true,
                ButtonCode = (short) buttonFlag
            };
            txt.Text = buttonFlag.ToString();
            txt.Tag = button;
        }

        /// <summary>
        /// Sets text box text and tag.
        /// </summary>
        /// <param name="newState">New state.</param>
        /// <param name="oldState">Previous state.</param>
        private void SetTextBoxText(State newState, State oldState)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    var txt = GetActiveTextBox();
                    if (txt == null) return;

                    if (newState.Gamepad.Buttons != oldState.Gamepad.Buttons)
                    {
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.A)
                            HandleButton(GamepadButtonFlags.A, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.B)
                            HandleButton(GamepadButtonFlags.B, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.X)
                            HandleButton(GamepadButtonFlags.X, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.Y)
                            HandleButton(GamepadButtonFlags.Y, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.Start)
                            HandleButton(GamepadButtonFlags.Start, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.Back)
                            HandleButton(GamepadButtonFlags.Back, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.LeftShoulder)
                            HandleButton(GamepadButtonFlags.LeftShoulder, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.RightShoulder)
                            HandleButton(GamepadButtonFlags.RightShoulder, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.LeftThumb)
                            HandleButton(GamepadButtonFlags.LeftThumb, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.RightThumb)
                            HandleButton(GamepadButtonFlags.RightThumb, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.DPadDown)
                            HandleButton(GamepadButtonFlags.DPadDown, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.DPadUp)
                            HandleButton(GamepadButtonFlags.DPadUp, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.DPadLeft)
                            HandleButton(GamepadButtonFlags.DPadLeft, txt);
                        if (newState.Gamepad.Buttons == GamepadButtonFlags.DPadRight)
                            HandleButton(GamepadButtonFlags.DPadRight, txt);
                        return;
                    }

                    if (newState.Gamepad.LeftThumbX != oldState.Gamepad.LeftThumbX)
                    {
                        GetAnalogXInput(newState.Gamepad.LeftThumbX, true, txt, false);
                        return;
                    }

                    if (newState.Gamepad.RightThumbX != oldState.Gamepad.RightThumbX)
                    {
                        GetAnalogXInput(newState.Gamepad.RightThumbX, false, txt, false);
                        return;
                    }

                    if (newState.Gamepad.LeftThumbY != oldState.Gamepad.LeftThumbY)
                    {
                        GetAnalogXInput(newState.Gamepad.LeftThumbY, true, txt, true);
                        return;
                    }

                    if (newState.Gamepad.RightThumbY != oldState.Gamepad.RightThumbY)
                    {
                        GetAnalogXInput(newState.Gamepad.RightThumbY, false, txt, true);
                        return;
                    }

                    if (newState.Gamepad.LeftTrigger != oldState.Gamepad.LeftTrigger)
                    {
                        var button = new XInputButton { IsLeftTrigger = true};
                        txt.Text = "LeftTrigger";
                        txt.Tag = button;
                        return;
                    }

                    if (newState.Gamepad.RightTrigger != oldState.Gamepad.RightTrigger)
                    {
                        var button = new XInputButton { IsRightTrigger = true };
                        txt.Text = "RightTrigger";
                        txt.Tag = button;
                    }
                }));
        }

        private void XInputRemap_OnClosing(object sender, CancelEventArgs e)
        {
            stopListen = true;
        }

        private void XInputRemap_OnLoaded(object sender, RoutedEventArgs e)
        {
            Thread xinputThread;
            if (_playerId == 1)
            {
                xinputThread = new Thread(() => Listen(UserIndex.One));
                LoadPlayerKeys(1);
            }
            else
            {
                xinputThread = new Thread(() => Listen(UserIndex.Two));
                LoadPlayerKeys(2);
            }
            xinputThread.Start();
        }

        private void LoadPlayerKeys(int playerNumber)
        {
            try
            {
                if (playerNumber == 1)
                {
                    if (File.Exists("XInputMapping1.xml"))
                    {
                        var map = JoystickHelper.DeSerializeXInput(1);
                        GetJoystickInformation(TxtButton1, map.Button1);
                        GetJoystickInformation(TxtButton2, map.Button2);
                        GetJoystickInformation(TxtButton3, map.Button3);
                        GetJoystickInformation(TxtButton4, map.Button4);
                        GetJoystickInformation(TxtButton5, map.Button5);
                        GetJoystickInformation(TxtButton6, map.Button6);
                        GetJoystickInformation(TxtStart, map.Start);
                        GetJoystickInformation(TxtService, map.Service);
                        GetJoystickInformation(TxtTestSw, map.Test);
                        GetJoystickInformation(TxtUp, map.Up);
                        GetJoystickInformation(TxtDown, map.Down);
                        GetJoystickInformation(TxtLeft, map.Left);
                        GetJoystickInformation(TxtRight, map.Right);
                        GetJoystickInformation(TxtSrcGearChange1, map.SrcGearChange1);
                        GetJoystickInformation(TxtSrcGearChange2, map.SrcGearChange2);
                        GetJoystickInformation(TxtSrcGearChange3, map.SrcGearChange3);
                        GetJoystickInformation(TxtSrcGearChange4, map.SrcGearChange4);
                        GetJoystickInformation(TxtSrcViewChange1, map.SrcViewChange1);
                        GetJoystickInformation(TxtSrcViewChange2, map.SrcViewChange2);
                        GetJoystickInformation(TxtSrcViewChange3, map.SrcViewChange3);
                        GetJoystickInformation(TxtSrcViewChange4, map.SrcViewChange4);
                        GetJoystickInformation(TxtSonicItemButton, map.SonicItem);
                        GetJoystickInformation(TxtGunTrigger, map.GunTrigger);
                        GetJoystickInformation(TxtLgiLeft, map.GunLeft);
                        GetJoystickInformation(TxtLgiRight, map.GunRight);
                        GetJoystickInformation(TxtLgiUp, map.GunUp);
                        GetJoystickInformation(TxtLgiDown, map.GunDown);
                        GetJoystickInformation(TxtWheel, map.WheelAxis);
                        GetJoystickInformation(TxtGas, map.GasAxis, false, true);
                        GetJoystickInformation(TxtBrake, map.BrakeAxis, true);
                        GetJoystickInformation(TxtInitialD6ShiftUp, map.InitialD6ShiftUp);
                        GetJoystickInformation(TxtInitialD6ShiftDown, map.InitialD6ShiftDown);
                        GetJoystickInformation(TxtInitialD6ViewChange, map.InitialD6ViewChange);
                        GetJoystickInformation(TxtInitialD6MenuDown, map.InitialD6MenuDown);
                        GetJoystickInformation(TxtInitialD6MenuUp, map.InitialD6MenuUp);
                        GetJoystickInformation(TxtInitialD6MenuLeft, map.InitialD6MenuLeft);
                        GetJoystickInformation(TxtInitialD6MenuRight, map.InitialD6MenuRight);
                        if (map.GunMultiplier >= 1 && map.GunMultiplier <= 10)
                            IUpDownMovementMultiplier.Value = map.GunMultiplier;
                        else
                            IUpDownMovementMultiplier.Value = 1;
                    }
                }
                else if (playerNumber == 2)
                {
                    if (File.Exists("XInputMapping2.xml"))
                    {
                        var map = JoystickHelper.DeSerializeXInput(2);
                        GetJoystickInformation(TxtButton1, map.Button1);
                        GetJoystickInformation(TxtButton2, map.Button2);
                        GetJoystickInformation(TxtButton3, map.Button3);
                        GetJoystickInformation(TxtButton4, map.Button4);
                        GetJoystickInformation(TxtButton5, map.Button5);
                        GetJoystickInformation(TxtButton6, map.Button6);
                        GetJoystickInformation(TxtStart, map.Start);
                        GetJoystickInformation(TxtService, map.Service);
                        GetJoystickInformation(TxtTestSw, map.Test);
                        GetJoystickInformation(TxtUp, map.Up);
                        GetJoystickInformation(TxtDown, map.Down);
                        GetJoystickInformation(TxtLeft, map.Left);
                        GetJoystickInformation(TxtRight, map.Right);
                        GetJoystickInformation(TxtGunTrigger, map.GunTrigger);
                        GetJoystickInformation(TxtLgiLeft, map.GunLeft);
                        GetJoystickInformation(TxtLgiRight, map.GunRight);
                        GetJoystickInformation(TxtLgiUp, map.GunUp);
                        GetJoystickInformation(TxtLgiDown, map.GunDown);
                        if (map.GunMultiplier >= 1 && map.GunMultiplier <= 10)
                            IUpDownMovementMultiplier.Value = map.GunMultiplier;
                        else
                            IUpDownMovementMultiplier.Value = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loading failed with error: {ex.InnerException} {ex.Message}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void GetJoystickInformation(TextBox txt, XInputButton xinputButton, bool isBrake = false, bool isGas = false)
        {
            if (xinputButton == null)
                return;
            if (xinputButton.IsButton)
            {
                txt.Text = ((GamepadButtonFlags) xinputButton.ButtonCode).ToString();
            }
            if (xinputButton.IsLeftThumbY)
            {
                txt.Text = "LeftThumbY";
                txt.Text += xinputButton.IsAxisMinus ? "-" : "+";
            }
            if (xinputButton.IsLeftThumbX)
            {
                txt.Text = "LeftThumbX";
                txt.Text += xinputButton.IsAxisMinus ? "-" : "+";
            }
            if (xinputButton.IsRightThumbX)
            {
                txt.Text = "RightThumbX";
                txt.Text += xinputButton.IsAxisMinus ? "-" : "+";
            }
            if (xinputButton.IsRightThumbY)
            {
                txt.Text = "RightThumbY";
                txt.Text += xinputButton.IsAxisMinus ? "-" : "+";
            }
            if (xinputButton.IsLeftTrigger)
            {
                txt.Text = "LeftTrigger";
            }
            if (xinputButton.IsRightTrigger)
            {
                txt.Text = "RightTrigger";
            }
            txt.Tag = xinputButton;
        }
    }
}
