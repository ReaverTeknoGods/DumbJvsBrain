using System;
using System.Linq;
using System.Threading;
using SharpDX.DirectInput;

namespace DumbJvsBrain.Common
{
    public class DirectInputListener
    {
        /// <summary>
        /// This is so we can easily kill the thread.
        /// </summary>
        public bool KillMe { get; set; }

        private byte _player1GunMultiplier = 1;
        private byte _player2GunMultiplier = 1;

        /// <summary>
        /// Handles Lets Go Island controls.
        /// </summary>
        /// <param name="playerButtons"></param>
        /// <param name="playerNumber"></param>
        private void HandleLgiControls(PlayerButtons playerButtons, int playerNumber)
        {
            while (true)
            {
                if (KillMe)
                    return;
                if (playerNumber == 1)
                {
                    if (playerButtons.UpPressed())
                    {
                        if (InputCode.AnalogBytes[0] <= 0xE0)
                            InputCode.AnalogBytes[0] += _player1GunMultiplier;
                    }
                    if (playerButtons.DownPressed())
                    {
                        if (InputCode.AnalogBytes[0] >= 10)
                            InputCode.AnalogBytes[0] -= _player1GunMultiplier;
                    }
                    if (playerButtons.RightPressed())
                    {
                        if (InputCode.AnalogBytes[2] >= 10)
                            InputCode.AnalogBytes[2] -= _player1GunMultiplier;
                    }
                    if (playerButtons.LeftPressed())
                    {
                        if (InputCode.AnalogBytes[2] <= 0xE0)
                            InputCode.AnalogBytes[2] += _player1GunMultiplier;
                    }
                }
                if (playerNumber == 2)
                {
                    if (playerButtons.UpPressed())
                    {
                        if (InputCode.AnalogBytes[4] <= 0xE0)
                            InputCode.AnalogBytes[4] += _player2GunMultiplier;
                    }
                    if (playerButtons.DownPressed())
                    {
                        if (InputCode.AnalogBytes[4] >= 10)
                            InputCode.AnalogBytes[4] -= _player2GunMultiplier;
                    }
                    if (playerButtons.RightPressed())
                    {
                        if (InputCode.AnalogBytes[6] >= 10)
                            InputCode.AnalogBytes[6] -= _player2GunMultiplier;
                    }
                    if (playerButtons.LeftPressed())
                    {
                        if (InputCode.AnalogBytes[6] <= 0xE0)
                            InputCode.AnalogBytes[6] += _player2GunMultiplier;
                    }
                }
            Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Listens given joystick.
        /// </summary>
        /// <param name="joystickGuid">Joystick Guid</param>
        /// <param name="playerNumber">Player number.</param>
        /// <param name="useSto0Z">If we use sto0z hack for driving.</param>
        /// <param name="joystickMapping">Joystick mapping.</param>
        public void Listen(Guid joystickGuid, int playerNumber, bool useSto0Z, JoystickMapping joystickMapping)
        {
            KillMe = false;
            if (!DoesJoystickExist(joystickGuid))
                return;

            if (InputCode.ButtonMode == GameProfiles.LetsGoIsland || InputCode.ButtonMode == GameProfiles.SegaDreamRaiders)
            {
                if (playerNumber == 1)
                {
                    if (joystickMapping.GunMultiplier >= 1 && joystickMapping.GunMultiplier <= 10)
                        _player1GunMultiplier = (byte) joystickMapping.GunMultiplier;
                    else
                        _player1GunMultiplier = 1;
                }
                if (playerNumber == 2)
                {
                    if (joystickMapping.GunMultiplier >= 1 && joystickMapping.GunMultiplier <= 10)
                        _player2GunMultiplier = (byte)joystickMapping.GunMultiplier;
                    else
                        _player2GunMultiplier = 1;
                }
                if (InputCode.ButtonMode == GameProfiles.LetsGoIsland)
                {
                    InputCode.AnalogBytes[0] = 127;
                    InputCode.AnalogBytes[2] = 127;
                    InputCode.AnalogBytes[4] = 127;
                    InputCode.AnalogBytes[6] = 127;
                }
            }

            using (var joystick = new Joystick(new DirectInput(), joystickGuid))
            {
                Console.WriteLine(
                    $"Listening Player {playerNumber} GUID: {joystickGuid} ProductName: {Helper.ExtractWithoutZeroes(joystick.Information.ProductName)}");

                // Set BufferSize in order to use buffered data.
                joystick.Properties.BufferSize = 512;

                // Acquire the joystick
                joystick.Acquire();
                if (InputCode.ButtonMode == GameProfiles.LetsGoIsland || InputCode.ButtonMode == GameProfiles.SegaDreamRaiders || InputCode.ButtonMode == GameProfiles.GoldenGun)
                {
                    Thread t;
                    t = playerNumber == 1 ? new Thread(() => HandleLgiControls(InputCode.PlayerOneButtons, 1)) : new Thread(() => HandleLgiControls(InputCode.PlayerTwoButtons, 2));
                    t.Start();
                }

                // Poll events from joystick
                try
                {
                    while (true)
                    {
                        if (KillMe)
                        {
                            joystick.Unacquire();
                            return;
                        }
                        joystick.Poll();
                        var datas = joystick.GetBufferedData();
                        foreach (var state in datas)
                        {
                            switch (InputCode.ButtonMode)
                            {
                                case GameProfiles.SegaRacingClassic:
                                    SrcInput(state, useSto0Z, joystickMapping);
                                    break;
                                case GameProfiles.InitialD6Aa:
                                    InitialD6Input(state, useSto0Z, joystickMapping);
                                    break;
                                case GameProfiles.VirtuaTennis4:
                                case GameProfiles.MeltyBlood:
                                    switch (playerNumber)
                                    {
                                        case 1:
                                            Standard6ButtonController(state, InputCode.PlayerOneButtons, joystickMapping);
                                            break;
                                        case 2:
                                            Standard6ButtonController(state, InputCode.PlayerTwoButtons, joystickMapping);
                                            break;
                                    }
                                    break;
                                    case GameProfiles.LetsGoIsland:
                                    case GameProfiles.SegaDreamRaiders:
                                        switch (playerNumber)
                                        {
                                            case 1:
                                                LetsGoIslandGuns(state, InputCode.PlayerOneButtons, joystickMapping);
                                                break;
                                            case 2:
                                                LetsGoIslandGuns(state, InputCode.PlayerTwoButtons, joystickMapping);
                                                break;
                                        }
                                        break;
                                case GameProfiles.GoldenGun:
                                    switch (playerNumber)
                                    {
                                        case 1:
                                            GoldenGunGuns(state, InputCode.PlayerOneButtons, joystickMapping);
                                            break;
                                        case 2:
                                            GoldenGunGuns(state, InputCode.PlayerTwoButtons, joystickMapping);
                                            break;
                                    }
                                    break;
                                case GameProfiles.VirtuaRLimit:
                                    VrlInput(state, useSto0Z, joystickMapping);
                                    break;
                                    case GameProfiles.AbdeTest:
                                        switch (playerNumber)
                                        {
                                            case 1:
                                                StandardJvsAbdeController(state, InputCode.PlayerOneButtons, joystickMapping);
                                                break;
                                            case 2:
                                                StandardJvsAbdeController(state, InputCode.PlayerTwoButtons, joystickMapping);
                                                break;
                                        }
                                        break;
                                    case GameProfiles.SegaSonicAllStarsRacing:
                                        SegaSonicInput(state, joystickMapping);
                                        break;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception happened in DirectInput controller thread: " + e);
                    joystick.Unacquire();
                }
            }
        }

        private void LetsGoIslandGuns(JoystickUpdate state, PlayerButtons playerButtons, JoystickMapping joystickMapping)
        {
            HandleDefaultSpecialButtons(state, joystickMapping, playerButtons);
            GetDirectionPress(playerButtons, joystickMapping.GunLeft, state, Direction.Left);
            GetDirectionPress(playerButtons, joystickMapping.GunRight, state, Direction.Right);
            GetDirectionPress(playerButtons, joystickMapping.GunUp, state, Direction.Up);
            GetDirectionPress(playerButtons, joystickMapping.GunDown, state, Direction.Down);
            playerButtons.Button1 = GetButtonPress(joystickMapping.GunTrigger, state);
        }

        private void GoldenGunGuns(JoystickUpdate state, PlayerButtons playerButtons, JoystickMapping joystickMapping)
        {
            HandleDefaultSpecialButtons(state, joystickMapping, playerButtons);
            GetDirectionPress(playerButtons, joystickMapping.GunUp, state, Direction.Right);
            GetDirectionPress(playerButtons, joystickMapping.GunDown, state, Direction.Left);
            GetDirectionPress(playerButtons, joystickMapping.GunLeft, state, Direction.Down);
            GetDirectionPress(playerButtons, joystickMapping.GunRight, state, Direction.Up);
            playerButtons.Button1 = GetButtonPress(joystickMapping.GunTrigger, state);
        }

        /// <summary>
        /// Listens input for Standard JVS ABDE button layout. Some games use this. (TX1/2 various game testing)
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="playerButtons">Player whos buttons to populate.</param>
        /// <param name="joystickMapping">Joystick mapping.</param>
        private static void StandardJvsAbdeController(JoystickUpdate state, PlayerButtons playerButtons, JoystickMapping joystickMapping)
        {
            HandleDefaultSpecialButtons(state, joystickMapping, playerButtons);
            GetDirectionPress(playerButtons, joystickMapping.Up, state, Direction.Up);
            GetDirectionPress(playerButtons, joystickMapping.Down, state, Direction.Down);
            GetDirectionPress(playerButtons, joystickMapping.Left, state, Direction.Left);
            GetDirectionPress(playerButtons, joystickMapping.Right, state, Direction.Right);
            playerButtons.Button1 = GetButtonPress(joystickMapping.Button1, state);
            playerButtons.Button2 = GetButtonPress(joystickMapping.Button2, state);
            playerButtons.Button4 = GetButtonPress(joystickMapping.Button3, state);
            playerButtons.Button5 = GetButtonPress(joystickMapping.Button4, state);
        }

        /// <summary>
        /// Listens input for Standard JVS 6 button layout.
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="playerButtons">Player whos buttons to populate.</param>
        /// <param name="joystickMapping">Joystick mapping.</param>
        private static void Standard6ButtonController(JoystickUpdate state, PlayerButtons playerButtons, JoystickMapping joystickMapping)
        {
            HandleDefaultSpecialButtons(state, joystickMapping, playerButtons);
            GetDirectionPress(playerButtons, joystickMapping.Up, state, Direction.Up);
            GetDirectionPress(playerButtons, joystickMapping.Down, state, Direction.Down);
            GetDirectionPress(playerButtons, joystickMapping.Left, state, Direction.Left);
            GetDirectionPress(playerButtons, joystickMapping.Right, state, Direction.Right);
            playerButtons.Button1 = GetButtonPress(joystickMapping.Button1, state);
            playerButtons.Button2 = GetButtonPress(joystickMapping.Button2, state);
            playerButtons.Button3 = GetButtonPress(joystickMapping.Button3, state);
            playerButtons.Button4 = GetButtonPress(joystickMapping.Button4, state);
            playerButtons.Button5 = GetButtonPress(joystickMapping.Button5, state);
            playerButtons.Button6 = GetButtonPress(joystickMapping.Button6, state);
        }

        /// <summary>
        /// Listens input for Virtua-R Limit
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="useSto0Z">Use stoozes special driving controls.</param>
        /// <param name="joystickMapping">Joystick mapping.</param>
        private static void VrlInput(JoystickUpdate state, bool useSto0Z, JoystickMapping joystickMapping)
        {
            PlayerButtons playerButtons = InputCode.PlayerOneButtons;
            HandleDefaultWheelControls(state, joystickMapping, useSto0Z);
            HandleDefaultSpecialButtons(state, joystickMapping, playerButtons);
        }

        /// <summary>
        /// Listens input for Sega Racing Classic.
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="useSto0Z">Use stoozes special driving controls.</param>
        /// <param name="joystickMapping"></param>
        private static void SrcInput(JoystickUpdate state, bool useSto0Z, JoystickMapping joystickMapping)
        {
            var playerButtons = InputCode.PlayerOneButtons;
            var playerButtons2 = InputCode.PlayerTwoButtons;
            HandleDefaultWheelControls(state, joystickMapping, useSto0Z);
            HandleDefaultSpecialButtons(state, joystickMapping, playerButtons);
            playerButtons.Up = GetButtonPress(joystickMapping.SrcViewChange1, state);
            playerButtons.Down = GetButtonPress(joystickMapping.SrcViewChange2, state);
            playerButtons.Left = GetButtonPress(joystickMapping.SrcViewChange3, state);
            playerButtons.Right = GetButtonPress(joystickMapping.SrcViewChange4, state);
            if (GetButtonPress(joystickMapping.SrcGearChange1, state) == true)
            {
                playerButtons2.Up = true;
                playerButtons2.Left = true;
                playerButtons2.Right = false;
                playerButtons2.Down = false;
            }

            if (GetButtonPress(joystickMapping.SrcGearChange2, state) == true)
            {
                playerButtons2.Up = false;
                playerButtons2.Right = false;
                playerButtons2.Down = true;
                playerButtons2.Left = true;
            }

            if (GetButtonPress(joystickMapping.SrcGearChange3, state) == true)
            {
                playerButtons2.Left = false;
                playerButtons2.Up = true;
                playerButtons2.Right = false;
                playerButtons2.Down = false;
            }

            if (GetButtonPress(joystickMapping.SrcGearChange4, state) == true)
            {
                playerButtons2.Left = false;
                playerButtons2.Up = false;
                playerButtons2.Right = false;
                playerButtons2.Down = true;
            }

        }

        /// <summary>
        /// Listens input for Sega Sonic.
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="joystickMapping">Joystick mapping.</param>
        private static void SegaSonicInput(JoystickUpdate state, JoystickMapping joystickMapping)
        {
            var playerButtons = InputCode.PlayerOneButtons;
            HandleDefaultWheelControls(state, joystickMapping);
            HandleDefaultSpecialButtons(state, joystickMapping, playerButtons);
            playerButtons.Button1 = GetButtonPress(joystickMapping.SonicItem, state);
        }

        /// <summary>
        /// Listens input for Initial D6.
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="useSto0Z">Use stoozes special driving controls.</param>
        /// <param name="joystickMapping">Joystick mapping.</param>
        private static void InitialD6Input(JoystickUpdate state, bool useSto0Z, JoystickMapping joystickMapping)
        {
            HandleDefaultWheelControls(state, joystickMapping, useSto0Z);
            HandleDefaultSpecialButtons(state, joystickMapping, InputCode.PlayerOneButtons);
            InputCode.PlayerOneButtons.Button1 = GetButtonPress(joystickMapping.InitialD6ViewChange, state);
            InputCode.PlayerOneButtons.Up = GetButtonPress(joystickMapping.InitialD6MenuUp, state);
            InputCode.PlayerOneButtons.Down = GetButtonPress(joystickMapping.InitialD6MenuDown, state);
            InputCode.PlayerOneButtons.Left = GetButtonPress(joystickMapping.InitialD6MenuLeft, state);
            InputCode.PlayerOneButtons.Right = GetButtonPress(joystickMapping.InitialD6MenuRight, state);
            InputCode.PlayerTwoButtons.Up = GetButtonPress(joystickMapping.InitialD6ShiftUp, state);
            InputCode.PlayerTwoButtons.Down = GetButtonPress(joystickMapping.InitialD6ShiftDown, state);
        }

        /// <summary>
        /// Handles special buttons Start, Test and Service.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="joystickMapping"></param>
        /// <param name="playerButtons"></param>
        private static void HandleDefaultSpecialButtons(JoystickUpdate state, JoystickMapping joystickMapping, PlayerButtons playerButtons)
        {
            playerButtons.Start = GetButtonPress(joystickMapping.Start, state);
            playerButtons.Test = GetButtonPress(joystickMapping.Test, state);
            playerButtons.Service = GetButtonPress(joystickMapping.Service, state);
        }

        /// <summary>
        /// Get directional press from button, POV and analog.
        /// </summary>
        /// <param name="playerButtons"></param>
        /// <param name="button"></param>
        /// <param name="state"></param>
        /// <param name="direction"></param>
        private static void GetDirectionPress(PlayerButtons playerButtons, JoystickButton button, JoystickUpdate state, Direction direction)
        {
            if (button == null)
                return;
            if ((JoystickOffset)button.Button != state.Offset)
                return;
            // POV
            if (button.Button >= 32 && button.Button <= 44)
            {
                switch (state.Value)
                {
                    case -1:
                        InputCode.SetPlayerDirection(playerButtons, Direction.HorizontalCenter);
                        InputCode.SetPlayerDirection(playerButtons, Direction.VerticalCenter);
                        break;
                    case 0:
                        InputCode.SetPlayerDirection(playerButtons, Direction.Up);
                        break;
                    case 4500:
                        InputCode.SetPlayerDirection(playerButtons, Direction.Up);
                        InputCode.SetPlayerDirection(playerButtons, Direction.Right);
                        break;
                    case 9000:
                        InputCode.SetPlayerDirection(playerButtons, Direction.Right);
                        break;
                    case 13500:
                        InputCode.SetPlayerDirection(playerButtons, Direction.Down);
                        InputCode.SetPlayerDirection(playerButtons, Direction.Right);
                        break;
                    case 18000:
                        InputCode.SetPlayerDirection(playerButtons, Direction.Down);
                        break;
                    case 22500:
                        InputCode.SetPlayerDirection(playerButtons, Direction.Left);
                        InputCode.SetPlayerDirection(playerButtons, Direction.Down);
                        break;
                    case 27000:
                        InputCode.SetPlayerDirection(playerButtons, Direction.Left);
                        break;
                    case 31500:
                        InputCode.SetPlayerDirection(playerButtons, Direction.Left);
                        InputCode.SetPlayerDirection(playerButtons, Direction.Up);
                        break;
                }
            }

            // Analog Axis, we expect that the both direction are on same axis!!!!
            if (state.Offset == JoystickOffset.X || state.Offset == JoystickOffset.Y ||
                state.Offset == JoystickOffset.Z || state.Offset == JoystickOffset.RotationX ||
                state.Offset == JoystickOffset.RotationY || state.Offset == JoystickOffset.RotationZ ||
                state.Offset == JoystickOffset.Sliders0 || state.Offset == JoystickOffset.Sliders1 ||
                state.Offset == JoystickOffset.AccelerationX || state.Offset == JoystickOffset.AccelerationY ||
                state.Offset == JoystickOffset.AccelerationZ)
            {
                if (button.IsAxisMinus)
                {
                    if (state.Value >= 32064 + 15000)
                    {
                    }
                    else if (state.Value <= 32064 - 15000)
                    {
                        InputCode.SetPlayerDirection(playerButtons, direction);
                    }
                    else
                    {
                        if (direction == Direction.Left || direction == Direction.Right)
                            InputCode.SetPlayerDirection(playerButtons, Direction.HorizontalCenter);
                        if (direction == Direction.Up || direction == Direction.Down)
                            InputCode.SetPlayerDirection(playerButtons, Direction.VerticalCenter);
                    }
                }
                else
                {
                    if (state.Value >= 32064 + 15000)
                    {
                        InputCode.SetPlayerDirection(playerButtons, direction);
                    }
                    else if (state.Value <= 32064 - 15000)
                    {
                    }
                    else
                    {
                        if(direction == Direction.Left || direction == Direction.Right)
                            InputCode.SetPlayerDirection(playerButtons, Direction.HorizontalCenter);
                        if (direction == Direction.Up || direction == Direction.Down)
                            InputCode.SetPlayerDirection(playerButtons, Direction.VerticalCenter);
                    }
                }
            }

            // Normal button
            if (button.Button >= 48 && button.Button <= 175)
            {
                if (state.Value != 0)
                {
                    InputCode.SetPlayerDirection(playerButtons, direction);
                }
                else
                {
                    if (direction == Direction.Left && !playerButtons.RightPressed())
                        InputCode.SetPlayerDirection(playerButtons, Direction.HorizontalCenter);
                    if (direction == Direction.Right && !playerButtons.LeftPressed())
                        InputCode.SetPlayerDirection(playerButtons, Direction.HorizontalCenter);
                    if (direction == Direction.Up && !playerButtons.DownPressed())
                        InputCode.SetPlayerDirection(playerButtons, Direction.VerticalCenter);
                    if (direction == Direction.Down && !playerButtons.UpPressed())
                        InputCode.SetPlayerDirection(playerButtons, Direction.VerticalCenter);
                }
            }
        }

        /// <summary>
        /// Gets if button is pressed. Null if not the same as requested.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private static bool? GetButtonPress(JoystickButton button, JoystickUpdate state)
        {
            if (button == null)
                return false;
            if ((JoystickOffset) button.Button != state.Offset)
                return null;

            // POV
            if (button.Button >= 32 && button.Button <= 44)
            {
                if (state.Value == button.PovDirection)
                {
                    return true;
                }
                return false;
            }

            // Normal button
            if (button.Button >= 48 && button.Button <= 175)
            {
                return state.Value != 0;
            }
            return null;
        }

        /// <summary>
        /// Handles default JVS wheel, gas and brake functionality.
        /// </summary>
        /// <param name="state">JoystickUpdate</param>
        /// <param name="joystickMapping">JoystickMapping</param>
        /// <param name="useSto0Z">If we use sto0z optimizations.</param>
        private static void HandleDefaultWheelControls(JoystickUpdate state, JoystickMapping joystickMapping, bool useSto0Z = false)
        {
            if (joystickMapping.WheelAxis == null
                || joystickMapping.GasAxis == null
                || joystickMapping.BrakeAxis == null)
                return;
            // Wheel is always full axis
            if ((JoystickOffset)joystickMapping.WheelAxis.Button == state.Offset)
            {
                bool isSonic = InputCode.ButtonMode == GameProfiles.SegaSonicAllStarsRacing;
                InputCode.AnalogBytes[0] = useSto0Z
                    ? JvsHelper.CalculateSto0ZWheelPos(state.Value)
                    : JvsHelper.CalculateWheelPos(state.Value, false, isSonic);
            }

            // If Gas has full axis
            if ((JoystickOffset)joystickMapping.GasAxis.Button == state.Offset && joystickMapping.GasAxis.IsFullAxis)
            {
                InputCode.AnalogBytes[2] = JvsHelper.CalculateGasPos(state.Value, true, joystickMapping.GasAxis.IsReverseAxis);
            }

            // If brake has full axis
            if ((JoystickOffset)joystickMapping.BrakeAxis.Button == state.Offset &&
                joystickMapping.BrakeAxis.IsFullAxis)
            {
                InputCode.AnalogBytes[4] = JvsHelper.CalculateGasPos(state.Value, true, joystickMapping.BrakeAxis.IsReverseAxis);
            }

            // If gas has NO full axis (we default that other way is brake, otherwise just ignore and let user fix his mistake himself.)
            if ((JoystickOffset)joystickMapping.GasAxis.Button == state.Offset && !joystickMapping.GasAxis.IsFullAxis)
            {
                if (joystickMapping.GasAxis.IsAxisMinus)
                {
                    if (state.Value <= 32767)
                    {
                        InputCode.AnalogBytes[2] = JvsHelper.CalculateGasPos(-state.Value + 32767, false, joystickMapping.GasAxis.IsReverseAxis);
                        InputCode.AnalogBytes[4] = 0;
                    }
                    else
                    {
                        InputCode.AnalogBytes[4] = JvsHelper.CalculateGasPos(state.Value - 32767, false, joystickMapping.GasAxis.IsReverseAxis);
                        InputCode.AnalogBytes[2] = 0;
                    }
                }
                else
                {
                    if (state.Value <= 32767)
                    {
                        InputCode.AnalogBytes[4] = JvsHelper.CalculateGasPos(state.Value - 32767, false, joystickMapping.GasAxis.IsReverseAxis);
                        InputCode.AnalogBytes[2] = 0;
                    }
                    else
                    {
                        InputCode.AnalogBytes[2] = JvsHelper.CalculateGasPos(-state.Value + 32767, false, joystickMapping.GasAxis.IsReverseAxis);
                        InputCode.AnalogBytes[4] = 0;
                    }
                }
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
    }
}
