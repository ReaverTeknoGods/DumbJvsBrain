using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using DumbJvsBrain.Common;
using SharpDX.DirectInput;

namespace DumbJVSManager
{
    /// <summary>
    /// Interaction logic for DirectInputRemap.xaml
    /// </summary>
    public partial class DirectInputRemap : Window
    {
        public DirectInputRemapViewModel ViewModel { get; set; }
        private SettingsData _settingsData;
        private bool stopListen;
        private bool showAllControls = true;
        private int _playerId;

        public DirectInputRemap(SettingsData settingsData, bool showExtras, int playerId)
        {
            _settingsData = settingsData;
            ViewModel = new DirectInputRemapViewModel();
            DataContext = ViewModel;
            ViewModel.ExtrasVisible = showExtras;
            _playerId = playerId;
            InitializeComponent();
        }

        /// <summary>
        /// Listens given joystick.
        /// </summary>
        /// <param name="joystickGuid">Joystick Guid</param>
        /// <param name="playerNumber">Player number.</param>
        public void Listen(Guid joystickGuid, int playerNumber)
        {
            if (!DoesJoystickExist(joystickGuid))
                return;
            using (var joystick = new Joystick(new DirectInput(), joystickGuid))
            {
                joystick.Properties.BufferSize = 512;
                joystick.Acquire();
                // Acquire the joystick
                try
                {
                    while (!stopListen)
                    {
                        joystick.Poll();
                        var datas = joystick.GetBufferedData();
                        foreach (var state in datas)
                        {
                            SetTextBoxText(state);
                        }
                    }
                }
                catch (Exception e)
                {
                    
                }
                joystick.Unacquire();
            }
        }

        private void BtnSaveKeys(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_playerId == 1)
                {
                    // Save player 1 xml
                    var map = new JoystickMapping
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
                    if (map.BrakeAxis != null)
                    {
                        if (ChkBrakeFullAxis.IsChecked.HasValue)
                            map.BrakeAxis.IsFullAxis = ChkBrakeFullAxis.IsChecked.Value;
                        if (ChkBrakeReverseAxis.IsChecked.HasValue)
                            map.BrakeAxis.IsReverseAxis = ChkBrakeReverseAxis.IsChecked.Value;
                    }

                    if (map.GasAxis != null)
                    {
                        if (ChkGasFullAxis.IsChecked.HasValue)
                            map.GasAxis.IsFullAxis = ChkGasFullAxis.IsChecked.Value;
                        if(ChkGasReverseAxis.IsChecked.HasValue)
                            map.GasAxis.IsReverseAxis = ChkGasReverseAxis.IsChecked.Value;
                    }
                    map.GunMultiplier = IUpDownMovementMultiplier.Value ?? 1;
                    JoystickHelper.Serialize(map, 1);
                }

                if (_playerId == 2)
                {
                    // Save player 2 xml
                    var map = new JoystickMapping
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
                    JoystickHelper.Serialize(map, 2);
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

        private JoystickButton GetTagButtonInfo(TextBox txt)
        {
            return (JoystickButton) txt.Tag;
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

        /// <summary>
        /// Sets text box text and tag.
        /// </summary>
        /// <param name="key"></param>
        private void SetTextBoxText(JoystickUpdate key)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    var txt = GetActiveTextBox();
                    if (txt == null) return;
                    if (key.Offset == JoystickOffset.PointOfViewControllers0 ||
                        key.Offset == JoystickOffset.PointOfViewControllers1 ||
                        key.Offset == JoystickOffset.PointOfViewControllers2 ||
                        key.Offset == JoystickOffset.PointOfViewControllers3)
                    {
                        if (key.Value != -1)
                        {
                            if (key.Value == 0)
                                txt.Text = key.Offset + " UP";
                            if(key.Value == 9000)
                                txt.Text = key.Offset + " RIGHT";
                            if (key.Value == 18000)
                                txt.Text = key.Offset + " DOWN";
                            if (key.Value == 27000)
                                txt.Text = key.Offset + " LEFT";
                            JoystickButton button = new JoystickButton
                            {
                                Button = (int) key.Offset,
                                IsAxis = false,
                                PovDirection = key.Value
                            };
                            txt.Tag = button;
                        }
                    }
                    if (key.Offset == JoystickOffset.X || key.Offset == JoystickOffset.Y ||
                        key.Offset == JoystickOffset.Z || key.Offset == JoystickOffset.RotationX ||
                        key.Offset == JoystickOffset.RotationY || key.Offset == JoystickOffset.RotationZ ||
                        key.Offset == JoystickOffset.Sliders0 || key.Offset == JoystickOffset.Sliders1 ||
                        key.Offset == JoystickOffset.AccelerationX || key.Offset == JoystickOffset.AccelerationY ||
                        key.Offset == JoystickOffset.AccelerationZ)
                    {
                        if (key.Value > 32767 + 15000)
                        {
                            txt.Text = key.Offset + "+";
                            JoystickButton button = new JoystickButton
                            {
                                Button = (int)key.Offset,
                                IsAxis = true,
                                IsAxisMinus = false,
                            };
                            txt.Tag = button;
                        }
                        else if (key.Value < 32767 - 15000)
                        {
                            txt.Text = key.Offset + "-";
                            JoystickButton button = new JoystickButton
                            {
                                Button = (int)key.Offset,
                                IsAxis = true,
                                IsAxisMinus = true,
                            };
                            txt.Tag = button;
                        }
                    }
                    if (key.Offset >= (JoystickOffset) 48 && key.Offset <= (JoystickOffset) 175)
                    {
                        if (key.Value == 128)
                        {
                            txt.Text = key.Offset.ToString();
                            JoystickButton button = new JoystickButton
                            {
                                Button = (int) key.Offset,
                                IsAxis = false
                            };
                            txt.Tag = button;
                        }
                    }
                }));
        }

        private void DirectInputRemap_OnClosing(object sender, CancelEventArgs e)
        {
            stopListen = true;
        }

        private void DirectInputRemap_OnLoaded(object sender, RoutedEventArgs e)
        {
            Thread dinputThread;
            if (_playerId == 1)
            {
                dinputThread = new Thread(() => Listen(_settingsData.PlayerOneGuid, 1));
                LoadPlayerKeys(1);
            }
            else
            {
                dinputThread = new Thread(() => Listen(_settingsData.PlayerTwoGuid, 2));
                LoadPlayerKeys(2);
            }
            dinputThread.Start();
        }

        private void LoadPlayerKeys(int playerNumber)
        {
            try
            {
                if (playerNumber == 1)
                {
                    if (File.Exists("JoystickMapping1.xml"))
                    {
                        var map = JoystickHelper.DeSerialize(1);
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
                    if (File.Exists("JoystickMapping2.xml"))
                    {
                        var map = JoystickHelper.DeSerialize(2);
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

        private void GetJoystickInformation(TextBox txt, JoystickButton joystickButton, bool isBrake = false, bool isGas = false)
        {
            if (joystickButton == null)
                return;
            var buttonData = (JoystickOffset)joystickButton.Button;
            if (buttonData == JoystickOffset.X || buttonData == JoystickOffset.Y ||
                buttonData == JoystickOffset.Z || buttonData == JoystickOffset.RotationX ||
                buttonData == JoystickOffset.RotationY || buttonData == JoystickOffset.RotationZ ||
                buttonData == JoystickOffset.Sliders0 || buttonData == JoystickOffset.Sliders1 ||
                buttonData == JoystickOffset.AccelerationX || buttonData == JoystickOffset.AccelerationY ||
                buttonData == JoystickOffset.AccelerationZ)
            {
                txt.Text = joystickButton.IsAxisMinus ? buttonData + "-" : buttonData + "+";
                if (isBrake)
                {
                    ChkBrakeFullAxis.IsChecked = joystickButton.IsFullAxis;
                    ChkBrakeReverseAxis.IsChecked = joystickButton.IsReverseAxis;
                }
                if (isGas)
                {
                    ChkGasFullAxis.IsChecked = joystickButton.IsFullAxis;
                    ChkGasReverseAxis.IsChecked = joystickButton.IsReverseAxis;
                }
                txt.Tag = joystickButton;
            }
            if (buttonData >= (JoystickOffset) 48 && buttonData <= (JoystickOffset) 175)
            {
                txt.Text = buttonData.ToString();
                txt.Tag = joystickButton;
            }
            if (buttonData == JoystickOffset.PointOfViewControllers0 ||
                buttonData == JoystickOffset.PointOfViewControllers1 ||
                buttonData == JoystickOffset.PointOfViewControllers2 ||
                buttonData == JoystickOffset.PointOfViewControllers3)
            {
                    if (joystickButton.PovDirection == 0)
                        txt.Text = buttonData + " UP";
                    if (joystickButton.PovDirection == 9000)
                        txt.Text = buttonData + " RIGHT";
                    if (joystickButton.PovDirection == 18000)
                        txt.Text = buttonData + " DOWN";
                    if (joystickButton.PovDirection == 27000)
                        txt.Text = buttonData + " LEFT";
                    JoystickButton button = new JoystickButton
                    {
                        Button = (int)buttonData,
                        IsAxis = false,
                        PovDirection = joystickButton.PovDirection
                    };
                    txt.Tag = button;
            }

        }

        /// <summary>
        /// Checks if joystick or gamepad GUID is found.
        /// </summary>
        /// <param name="joystickGuid">Joystick GUID;:</param>
        /// <returns></returns>
        private bool DoesJoystickExist(Guid joystickGuid)
        {
            return new DirectInput().GetDevices()
                .Any(
                    x => x.InstanceGuid == joystickGuid);
        }

        private void btnFullAxis_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                $"If you see - and + when you hold/release Gas/Brake it means it's full Axis.{Environment.NewLine}If you have full axis and do not check the box, the gas/brake will not function properly.{Environment.NewLine}All premium wheels have full axis such as Logitech G27.{Environment.NewLine}Lesser wheels like Logitech Momo Red do not.",
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnReverseAxis_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                $"If you have a problem in the game where you don't even hold gas/brake and it's doing something. This is for you!",
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
