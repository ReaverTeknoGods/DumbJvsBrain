using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;

namespace DirtyJvsBrain
{
    public class X360Listener
    {
        public void Listen()
        {
            List<Guid> joysticks = new List<Guid>();
            // Initialize DirectInput
            var directInput = new DirectInput();

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
            {
                joysticks.Add(deviceInstance.InstanceGuid);
            }

            foreach (
                var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
            {
                joysticks.Add(deviceInstance.InstanceGuid);
            }

            if (!joysticks.Any())
            {
                return;
            }

            var joystick = new Joystick(directInput, joysticks.FirstOrDefault());

            Console.WriteLine("Found Joystick/Gamepad with GUID: {0}", joysticks.FirstOrDefault());

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 512;

            // Acquire the joystick
            joystick.Acquire();

            // Poll events from joystick
            try
            {
                while (true)
                {
                    joystick.Poll();
                    var datas = joystick.GetBufferedData();
                    foreach (var state in datas)
                    {
                        if (InputCode.ButtonMode == GameSelection.InitialD6)
                        {
                            if (state.Offset == JoystickOffset.X)
                            {
                                InputCode.Wheel = Helper.CalculateWheelPos(state.Value);
                            }
                            if (state.Offset == JoystickOffset.Z)
                            {
                                if (state.Value <= 32767)
                                {
                                    InputCode.Gas = Helper.CalculateGasPos(-state.Value + 32767);
                                }
                                else
                                {
                                    InputCode.Brake = Helper.CalculateGasPos(state.Value - 32767);
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
                                InputCode.Start1 = state.Value != 0;
                            }
                            if (state.Offset == JoystickOffset.Buttons9)
                            {
                                InputCode.Test = state.Value != 0;
                            }
                            if (state.Offset == JoystickOffset.Buttons6)
                            {
                                InputCode.Service1 = state.Value != 0;
                            }
                        }
                        else if (InputCode.ButtonMode == GameSelection.VirtuaTennis4)
                        {
                            if (state.Offset == JoystickOffset.X)
                            {
                                if (state.Value >= 32064 + 15000)
                                {
                                    InputCode.Player1Right = true;
                                    InputCode.Player1Left = false;
                                }
                                else if (state.Value <= 32064 - 15000)
                                {
                                    InputCode.Player1Left = true;
                                    InputCode.Player1Right = false;
                                }
                                else
                                {
                                    InputCode.Player1Left = false;
                                    InputCode.Player1Right = false;
                                }
                            }
                            if (state.Offset == JoystickOffset.Y)
                            {
                                if (state.Value >= 32730 + 15000)
                                {
                                    InputCode.Player1Down = true;
                                    InputCode.Player1Up = false;
                                }
                                else if (state.Value <= 32730 - 15000)
                                {
                                    InputCode.Player1Up = true;
                                    InputCode.Player1Down = false;
                                }
                                else
                                {
                                    InputCode.Player1Down = false;
                                    InputCode.Player1Up = false;
                                }
                            }
                            if (state.Offset == JoystickOffset.Buttons0)
                            {
                                InputCode.Player1Button1 = state.Value != 0;
                            }
                            if (state.Offset == JoystickOffset.Buttons1)
                            {
                                InputCode.Player1Button2 = state.Value != 0;
                            }
                            if (state.Offset == JoystickOffset.Buttons2)
                            {
                                InputCode.Player1Button3 = state.Value != 0;
                            }
                            if (state.Offset == JoystickOffset.Buttons3)
                            {
                                InputCode.Player1Button4 = state.Value != 0;
                            }
                            if (state.Offset == JoystickOffset.Buttons4)
                            {
                                InputCode.Player1Button5 = state.Value != 0;
                            }
                            if (state.Offset == JoystickOffset.Buttons5)
                            {
                                InputCode.Player1Button6 = state.Value != 0;
                            }
                            if (state.Offset == JoystickOffset.Buttons7)
                            {
                                InputCode.Start1 = state.Value != 0;
                            }
                            if (state.Offset == JoystickOffset.Buttons9)
                            {
                                InputCode.Test = state.Value != 0;
                            }
                            if (state.Offset == JoystickOffset.Buttons6)
                            {
                                InputCode.Service1 = state.Value != 0;
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception happened in xbox360 controller thread: " + e);
            }
        }
    }
}
