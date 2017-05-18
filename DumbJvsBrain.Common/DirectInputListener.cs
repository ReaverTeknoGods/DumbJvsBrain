using System;
using System.Linq;
using SharpDX.DirectInput;

namespace DumbJvsBrain.Common
{
    public class DirectInputListener
    {
        /// <summary>
        /// This is so we can easily kill the thread.
        /// </summary>
        public bool KillMe { get; set; }

        /// <summary>
        /// Listens given joystick.
        /// </summary>
        /// <param name="joystickGuid">Joystick Guid</param>
        /// <param name="playerNumber">Player number.</param>
        /// <param name="useSto0Z">If we use sto0z hack for driving.</param>
        public void Listen(Guid joystickGuid, int playerNumber, bool useSto0Z)
        {
            KillMe = false;
            if (!DoesJoystickExist(joystickGuid))
                return;

            using (var joystick = new Joystick(new DirectInput(), joystickGuid))
            {
                Console.WriteLine(
                    $"Listening Player {playerNumber} GUID: {joystickGuid} ProductName: {Helper.ExtractWithoutZeroes(joystick.Information.ProductName)}");

                // Set BufferSize in order to use buffered data.
                joystick.Properties.BufferSize = 512;

                // Acquire the joystick
                joystick.Acquire();

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
                                    SrcInput(state, useSto0Z);
                                    break;
                                case GameProfiles.VirtuaTennis4:
                                case GameProfiles.MeltyBlood:
                                    switch (playerNumber)
                                    {
                                        case 1:
                                            Standard6ButtonController(state, InputCode.PlayerOneButtons);
                                            break;
                                        case 2:
                                            Standard6ButtonController(state, InputCode.PlayerTwoButtons);
                                            break;
                                    }
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

        /// <summary>
        /// Listens input for Standard JVS 6 button layout.
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        /// <param name="playerButtons">Player whos buttons to populate.</param>
        private static void Standard6ButtonController(JoystickUpdate state, PlayerButtons playerButtons)
        {
            if (state.Offset == JoystickOffset.X)
            {
                if (state.Value >= 32064 + 15000)
                {
                    InputCode.SetPlayerDirection(playerButtons, Direction.Right);
                }
                else if (state.Value <= 32064 - 15000)
                {
                    InputCode.SetPlayerDirection(playerButtons, Direction.Left);
                }
                else
                {
                    InputCode.SetPlayerDirection(playerButtons, Direction.HorizontalCenter);
                }
            }
            if (state.Offset == JoystickOffset.Y)
            {
                if (state.Value >= 32730 + 15000)
                {
                    InputCode.SetPlayerDirection(playerButtons, Direction.Down);
                }
                else if (state.Value <= 32730 - 15000)
                {
                    InputCode.SetPlayerDirection(playerButtons, Direction.Up);
                }
                else
                {
                    InputCode.SetPlayerDirection(playerButtons, Direction.VerticalCenter);
                }
            }
            if (state.Offset == JoystickOffset.Buttons0)
            {
                playerButtons.Button1 = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons1)
            {
                playerButtons.Button2 = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons2)
            {
                playerButtons.Button3 = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons3)
            {
                playerButtons.Button4 = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons4)
            {
                playerButtons.Button5 = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons5)
            {
                playerButtons.Button6 = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons7)
            {
                playerButtons.Start = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons9)
            {
                playerButtons.Test = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons6)
            {
                playerButtons.Service = state.Value != 0;
            }
        }

        /// <summary>
        /// Listens input for Sega Racing Classic.
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        private static void SrcInput(JoystickUpdate state, bool useSto0z)
        {
            PlayerButtons playerButtons = InputCode.PlayerOneButtons;
            if (state.Offset == JoystickOffset.X)
            {
                InputCode.Wheel = useSto0z ? JvsHelper.CalculateWheelPos(state.Value) : JvsHelper.CalculateSto0ZWheelPos(state.Value);
            }
            if (state.Offset == JoystickOffset.Z)
            {
                if (state.Value <= 32767)
                {
                    InputCode.Gas = JvsHelper.CalculateGasPos(-state.Value + 32767);
                }
                else
                {
                    InputCode.Brake = JvsHelper.CalculateGasPos(state.Value - 32767);
                }
            }
            if (state.Offset == JoystickOffset.Buttons0)
            {
                playerButtons.Up = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons1)
            {
                playerButtons.Down = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons2)
            {
                playerButtons.Left = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons3)
            {
                playerButtons.Right = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons5)
            {
                // Shift Down
                InputCode.ShiftDown = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons6)
            {
                // Shift Up
                InputCode.ShiftUp = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons7)
            {
                playerButtons.Start = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons9)
            {
                playerButtons.Test = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons6)
            {
                playerButtons.Service = state.Value != 0;
            }
        }

        /// <summary>
        /// Listens input for Initial D6.
        /// </summary>
        /// <param name="state">JoystickUpdate state.</param>
        private static void InitialD6Input(JoystickUpdate state)
        {
            PlayerButtons playerButtons = InputCode.PlayerOneButtons;
            if (state.Offset == JoystickOffset.X)
            {
                InputCode.Wheel = JvsHelper.CalculateWheelPos(state.Value);
            }
            if (state.Offset == JoystickOffset.Z)
            {
                if (state.Value <= 32767)
                {
                    InputCode.Gas = JvsHelper.CalculateGasPos(-state.Value + 32767);
                }
                else
                {
                    InputCode.Brake = JvsHelper.CalculateGasPos(state.Value - 32767);
                }
            }
            if (state.Offset == JoystickOffset.Buttons5)
            {
                // Shift Down
                InputCode.ShiftDown = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons6)
            {
                // Shift Up
                InputCode.ShiftUp = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons7)
            {
                playerButtons.Start = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons9)
            {
                playerButtons.Test = state.Value != 0;
            }
            if (state.Offset == JoystickOffset.Buttons6)
            {
                playerButtons.Service = state.Value != 0;
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
                    x =>
                        (x.Type == DeviceType.Gamepad || x.Type == DeviceType.Joystick || x.Type == DeviceType.Driving || x.Type == DeviceType.Flight || x.Type == DeviceType.FirstPerson) &&
                        x.InstanceGuid == joystickGuid);
        }
    }
}
