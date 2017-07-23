using System;
using System.Threading;
using SharpDX.XInput;

namespace DumbJvsBrain.Common
{
    public class XInputListener
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
        /// <param name="playerNumber">Player number.</param>
        /// <param name="useSto0Z">If we use sto0z hack for driving.</param>
        /// <param name="xinputMapping">Joystick mapping.</param>
        public void Listen(int playerNumber, bool useSto0Z, XInputMapping xinputMapping)
        {
            KillMe = false;
            UserIndex index;
            if (playerNumber == 1)
                index = UserIndex.One;
            else if (playerNumber == 2)
                index = UserIndex.Two;
            else
                return;

            var controller = new Controller(index);
            if (!controller.IsConnected)
                return;

            if (InputCode.ButtonMode == GameProfiles.LetsGoIsland ||
                InputCode.ButtonMode == GameProfiles.SegaDreamRaiders)
            {
                if (playerNumber == 1)
                {
                    if (xinputMapping.GunMultiplier >= 1 && xinputMapping.GunMultiplier <= 10)
                        _player1GunMultiplier = (byte)xinputMapping.GunMultiplier;
                    else
                        _player1GunMultiplier = 1;
                }
                if (playerNumber == 2)
                {
                    if (xinputMapping.GunMultiplier >= 1 && xinputMapping.GunMultiplier <= 10)
                        _player2GunMultiplier = (byte)xinputMapping.GunMultiplier;
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


            Console.WriteLine(
                $"Listening Player {playerNumber} Xbox Index: {index}");

            if (InputCode.ButtonMode == GameProfiles.LetsGoIsland ||
                InputCode.ButtonMode == GameProfiles.SegaDreamRaiders)
            {
                Thread t;
                t = playerNumber == 1
                    ? new Thread(() => HandleLgiControls(InputCode.PlayerOneButtons, 1))
                    : new Thread(() => HandleLgiControls(InputCode.PlayerTwoButtons, 2));
                t.Start();
            }

            // Poll events from joystick
            try
            {
                var previousState = controller.GetState();
                while (!KillMe)
                {
                    var state = controller.GetState();
                    if (previousState.PacketNumber != state.PacketNumber)
                    {
                        switch (InputCode.ButtonMode)
                        {
                            case GameProfiles.SegaRacingClassic:
                                SrcInput(state, useSto0Z, xinputMapping);
                                break;
                            case GameProfiles.InitialD6Aa:
                                InitialD6Input(state, useSto0Z, xinputMapping);
                                break;
                            case GameProfiles.VirtuaTennis4:
                            case GameProfiles.MeltyBlood:
                                switch (playerNumber)
                                {
                                    case 1:
                                        Standard6ButtonController(state, InputCode.PlayerOneButtons,
                                            xinputMapping);
                                        break;
                                    case 2:
                                        Standard6ButtonController(state, InputCode.PlayerTwoButtons,
                                            xinputMapping);
                                        break;
                                }
                                break;
                            case GameProfiles.LetsGoIsland:
                            case GameProfiles.SegaDreamRaiders:
                                switch (playerNumber)
                                {
                                    case 1:
                                        LetsGoIslandGuns(state, InputCode.PlayerOneButtons,
                                            xinputMapping);
                                        break;
                                    case 2:
                                        LetsGoIslandGuns(state, InputCode.PlayerTwoButtons,
                                            xinputMapping);
                                        break;
                                }
                                break;
                            case GameProfiles.VirtuaRLimit:
                                VrlInput(state, useSto0Z, xinputMapping);
                                break;
                            case GameProfiles.AbdeTest:
                                switch (playerNumber)
                                {
                                    case 1:
                                        StandardJvsAbdeController(state, InputCode.PlayerOneButtons,
                                            xinputMapping);
                                        break;
                                    case 2:
                                        StandardJvsAbdeController(state, InputCode.PlayerTwoButtons,
                                            xinputMapping);
                                        break;
                                }
                                break;
                            case GameProfiles.SegaSonicAllStarsRacing:
                                SegaSonicInput(state, xinputMapping);
                                break;
                        }
                    }
                    Thread.Sleep(10);
                    previousState = state;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception happened in DirectInput controller thread: " + e);
            }
        }

        private void LetsGoIslandGuns(State state, PlayerButtons playerButtons, XInputMapping xinputMapping)
        {
            HandleDefaultSpecialButtons(state, xinputMapping, playerButtons);
            GetDirectionPress(playerButtons, xinputMapping.GunLeft, state, Direction.Left);
            GetDirectionPress(playerButtons, xinputMapping.GunRight, state, Direction.Right);
            GetDirectionPress(playerButtons, xinputMapping.GunUp, state, Direction.Up);
            GetDirectionPress(playerButtons, xinputMapping.GunDown, state, Direction.Down);
            playerButtons.Button1 = GetButtonPress(xinputMapping.GunTrigger, state);
        }

        /// <summary>
        /// Listens input for Standard JVS ABDE button layout. Some games use this. (TX1/2 various game testing)
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="playerButtons">Player whos buttons to populate.</param>
        /// <param name="xinputMapping">Joystick mapping.</param>
        private static void StandardJvsAbdeController(State state, PlayerButtons playerButtons, XInputMapping xinputMapping)
        {
            HandleDefaultSpecialButtons(state, xinputMapping, playerButtons);
            GetDirectionPress(playerButtons, xinputMapping.Up, state, Direction.Up);
            GetDirectionPress(playerButtons, xinputMapping.Down, state, Direction.Down);
            GetDirectionPress(playerButtons, xinputMapping.Left, state, Direction.Left);
            GetDirectionPress(playerButtons, xinputMapping.Right, state, Direction.Right);
            playerButtons.Button1 = GetButtonPress(xinputMapping.Button1, state);
            playerButtons.Button2 = GetButtonPress(xinputMapping.Button2, state);
            playerButtons.Button4 = GetButtonPress(xinputMapping.Button3, state);
            playerButtons.Button5 = GetButtonPress(xinputMapping.Button4, state);
        }

        /// <summary>
        /// Listens input for Standard JVS 6 button layout.
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="playerButtons">Player whos buttons to populate.</param>
        /// <param name="xinputMapping">Joystick mapping.</param>
        private static void Standard6ButtonController(State state, PlayerButtons playerButtons, XInputMapping xinputMapping)
        {
            HandleDefaultSpecialButtons(state, xinputMapping, playerButtons);
            GetDirectionPress(playerButtons, xinputMapping.Up, state, Direction.Up);
            GetDirectionPress(playerButtons, xinputMapping.Down, state, Direction.Down);
            GetDirectionPress(playerButtons, xinputMapping.Left, state, Direction.Left);
            GetDirectionPress(playerButtons, xinputMapping.Right, state, Direction.Right);
            playerButtons.Button1 = GetButtonPress(xinputMapping.Button1, state);
            playerButtons.Button2 = GetButtonPress(xinputMapping.Button2, state);
            playerButtons.Button3 = GetButtonPress(xinputMapping.Button3, state);
            playerButtons.Button4 = GetButtonPress(xinputMapping.Button4, state);
            playerButtons.Button5 = GetButtonPress(xinputMapping.Button5, state);
            playerButtons.Button6 = GetButtonPress(xinputMapping.Button6, state);
        }

        /// <summary>
        /// Listens input for Virtua-R Limit
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="useSto0Z">Use stoozes special driving controls.</param>
        /// <param name="xinputMapping">Joystick mapping.</param>
        private static void VrlInput(State state, bool useSto0Z, XInputMapping xinputMapping)
        {
            PlayerButtons playerButtons = InputCode.PlayerOneButtons;
            HandleDefaultWheelControls(state, xinputMapping, useSto0Z);
            HandleDefaultSpecialButtons(state, xinputMapping, playerButtons);
        }

        /// <summary>
        /// Listens input for Sega Racing Classic.
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="useSto0Z">Use stoozes special driving controls.</param>
        /// <param name="xinputMapping"></param>
        private static void SrcInput(State state, bool useSto0Z, XInputMapping xinputMapping)
        {
            var playerButtons = InputCode.PlayerOneButtons;
            var playerButtons2 = InputCode.PlayerTwoButtons;
            HandleDefaultWheelControls(state, xinputMapping, useSto0Z);
            HandleDefaultSpecialButtons(state, xinputMapping, playerButtons);
            playerButtons.Up = GetButtonPress(xinputMapping.SrcViewChange1, state);
            playerButtons.Down = GetButtonPress(xinputMapping.SrcViewChange2, state);
            playerButtons.Left = GetButtonPress(xinputMapping.SrcViewChange3, state);
            playerButtons.Right = GetButtonPress(xinputMapping.SrcViewChange4, state);
            if (GetButtonPress(xinputMapping.SrcGearChange1, state) == true)
            {
                playerButtons2.Up = true;
                playerButtons2.Left = true;
                playerButtons2.Right = false;
                playerButtons2.Down = false;
            }

            if (GetButtonPress(xinputMapping.SrcGearChange2, state) == true)
            {
                playerButtons2.Up = false;
                playerButtons2.Right = false;
                playerButtons2.Down = true;
                playerButtons2.Left = true;
            }

            if (GetButtonPress(xinputMapping.SrcGearChange3, state) == true)
            {
                playerButtons2.Left = false;
                playerButtons2.Up = true;
                playerButtons2.Right = false;
                playerButtons2.Down = false;
            }

            if (GetButtonPress(xinputMapping.SrcGearChange4, state) == true)
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
        /// <param name="xinputMapping">Joystick mapping.</param>
        private static void SegaSonicInput(State state, XInputMapping xinputMapping)
        {
            var playerButtons = InputCode.PlayerOneButtons;
            HandleDefaultWheelControls(state, xinputMapping);
            HandleDefaultSpecialButtons(state, xinputMapping, playerButtons);
            playerButtons.Button1 = GetButtonPress(xinputMapping.SonicItem, state);
        }

        /// <summary>
        /// Listens input for Initial D6.
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="useSto0Z">Use stoozes special driving controls.</param>
        /// <param name="xinputMapping">Joystick mapping.</param>
        private static void InitialD6Input(State state, bool useSto0Z, XInputMapping xinputMapping)
        {
            HandleDefaultWheelControls(state, xinputMapping, useSto0Z);
            HandleDefaultSpecialButtons(state, xinputMapping, InputCode.PlayerOneButtons);
            InputCode.PlayerOneButtons.Button1 = GetButtonPress(xinputMapping.InitialD6ViewChange, state);
            InputCode.PlayerOneButtons.Up = GetButtonPress(xinputMapping.InitialD6MenuUp, state);
            InputCode.PlayerOneButtons.Down = GetButtonPress(xinputMapping.InitialD6MenuDown, state);
            InputCode.PlayerOneButtons.Left = GetButtonPress(xinputMapping.InitialD6MenuLeft, state);
            InputCode.PlayerOneButtons.Right = GetButtonPress(xinputMapping.InitialD6MenuRight, state);
            InputCode.PlayerTwoButtons.Up = GetButtonPress(xinputMapping.InitialD6ShiftUp, state);
            InputCode.PlayerTwoButtons.Down = GetButtonPress(xinputMapping.InitialD6ShiftDown, state);
        }

        /// <summary>
        /// Handles special buttons Start, Test and Service.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="xinputMapping"></param>
        /// <param name="playerButtons"></param>
        private static void HandleDefaultSpecialButtons(State state, XInputMapping xinputMapping, PlayerButtons playerButtons)
        {
            playerButtons.Start = GetButtonPress(xinputMapping.Start, state);
            playerButtons.Test = GetButtonPress(xinputMapping.Test, state);
            playerButtons.Service = GetButtonPress(xinputMapping.Service, state);
        }

        /// <summary>
        /// Get directional press from button, POV and analog.
        /// </summary>
        /// <param name="playerButtons"></param>
        /// <param name="button"></param>
        /// <param name="state"></param>
        /// <param name="direction"></param>
        private static void GetDirectionPress(PlayerButtons playerButtons, XInputButton button, State state, Direction direction)
        {
            if (button == null)
                return;

            // Analog Axis, we expect that the both direction are on same axis!!!!
            if (button.IsLeftThumbX || button.IsLeftThumbY || button.IsRightThumbX || button.IsRightThumbY)
            {
                var calcVal = 0;
                if (button.IsLeftThumbY) calcVal = state.Gamepad.LeftThumbY;
                if (button.IsLeftThumbX) calcVal = state.Gamepad.LeftThumbX;
                if (button.IsRightThumbX) calcVal = state.Gamepad.RightThumbX;
                if (button.IsRightThumbY) calcVal = state.Gamepad.RightThumbY;
                if (button.IsAxisMinus)
                {
                    if (calcVal >= 0 + 15000)
                    {
                    }
                    else if (calcVal <= 0 - 15000)
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
                    if (calcVal >= 0 + 15000)
                    {
                        InputCode.SetPlayerDirection(playerButtons, direction);
                    }
                    else if (calcVal <= 0 - 15000)
                    {
                    }
                    else
                    {
                        if (direction == Direction.Left || direction == Direction.Right)
                            InputCode.SetPlayerDirection(playerButtons, Direction.HorizontalCenter);
                        if (direction == Direction.Up || direction == Direction.Down)
                            InputCode.SetPlayerDirection(playerButtons, Direction.VerticalCenter);
                    }
                }
            }

            // Normal button
            if (button.IsButton)
            {
                if ((button.ButtonCode & (short)state.Gamepad.Buttons) != 0)
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
        private static bool? GetButtonPress(XInputButton button, State state)
        {
            if (button == null)
                return false;

            if (button.IsLeftTrigger)
                return state.Gamepad.LeftTrigger != 0;

            if (button.IsRightTrigger)
                return state.Gamepad.RightTrigger != 0;

            var buttonButtonCode = (short)state.Gamepad.Buttons;
            return (buttonButtonCode & button.ButtonCode) != 0;
        }

        private static byte CalculateAxisOrTriggerGasBrake(XInputButton button, State state)
        {
            if (button.IsButton)
            {
                var btnPress = GetButtonPress(button, state);
                if (btnPress == true)
                    return 0xFF;
                return 0x00;
            }

            if (button.IsLeftThumbX)
            {
                return JvsHelper.CalculateGasPos(state.Gamepad.LeftThumbX, true, false);
            }

            if (button.IsLeftThumbY)
            {
                return JvsHelper.CalculateGasPos(state.Gamepad.LeftThumbY, true, false);
            }

            if (button.IsRightThumbX)
            {
                return JvsHelper.CalculateGasPos(state.Gamepad.RightThumbX, true, false);
            }

            if (button.IsRightThumbY)
            {
                return JvsHelper.CalculateGasPos(state.Gamepad.RightThumbY, true, false);
            }

            if (button.IsLeftTrigger)
            {
                return state.Gamepad.LeftTrigger;
            }

            if (button.IsRightTrigger)
            {
                return state.Gamepad.RightTrigger;
            }
            return 0;
        }

        private static byte CalculateWheelPos(XInputButton button, State state, bool useSto0Z)
        {
            bool isSonic = InputCode.ButtonMode == GameProfiles.SegaSonicAllStarsRacing;
            if (button.IsLeftThumbX)
            {
                return useSto0Z ? JvsHelper.CalculateSto0ZWheelPos(state.Gamepad.LeftThumbX, true) : JvsHelper.CalculateWheelPos(state.Gamepad.LeftThumbX, true, isSonic);
            }

            if (button.IsLeftThumbY)
            {
                return useSto0Z ? JvsHelper.CalculateSto0ZWheelPos(state.Gamepad.LeftThumbY, true) : JvsHelper.CalculateWheelPos(state.Gamepad.LeftThumbY, true, isSonic);
            }

            if (button.IsRightThumbX)
            {
                return useSto0Z ? JvsHelper.CalculateSto0ZWheelPos(state.Gamepad.RightThumbX, true) : JvsHelper.CalculateWheelPos(state.Gamepad.RightThumbX, true, isSonic);
            }

            if (button.IsRightThumbY)
            {
                return useSto0Z ? JvsHelper.CalculateSto0ZWheelPos(state.Gamepad.RightThumbY, true) : JvsHelper.CalculateWheelPos(state.Gamepad.RightThumbY, true, isSonic);
            }
            return 0x7F;
        }

        /// <summary>
        /// Handles default JVS wheel, gas and brake functionality.
        /// </summary>
        /// <param name="state">JoystickUpdate</param>
        /// <param name="xinputMapping"></param>
        /// <param name="useSto0Z">If we use sto0z optimizations.</param>
        private static void HandleDefaultWheelControls(State state, XInputMapping xinputMapping, bool useSto0Z = false)
        {
            if (xinputMapping.WheelAxis == null
                || xinputMapping.GasAxis == null
                || xinputMapping.BrakeAxis == null)
                return;

            // Gas and Brake can be buttons
            InputCode.AnalogBytes[2] = CalculateAxisOrTriggerGasBrake(xinputMapping.GasAxis, state);
            InputCode.AnalogBytes[4] = CalculateAxisOrTriggerGasBrake(xinputMapping.BrakeAxis, state);
            // Wheel is always full axis
            InputCode.AnalogBytes[0] = CalculateWheelPos(xinputMapping.WheelAxis, state, useSto0Z);
        }
    }
}
