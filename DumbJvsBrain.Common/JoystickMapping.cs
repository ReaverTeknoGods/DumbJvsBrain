using System;
using System.Xml.Serialization;

namespace DumbJvsBrain.Common
{
    [Serializable]
    public class JoystickButton
    {
        public int Button { get; set; }
        public bool IsAxis { get; set; }
        public bool IsAxisMinus { get; set; }
        public bool IsFullAxis { get; set; }
        public int PovDirection { get; set; }
        public bool IsReverseAxis { get; set; }
    }

    public class XInputButton
    {
        public bool IsLeftThumbX { get; set; }
        public bool IsRightThumbX { get; set; }
        public bool IsLeftThumbY { get; set; }
        public bool IsRightThumbY { get; set; }
        public bool IsAxisMinus { get; set; }
        public bool IsLeftTrigger { get; set; }
        public bool IsRightTrigger { get; set; }
        public short ButtonCode { get; set; }
        public bool IsButton { get; set; }
    }

    [Serializable]
    [XmlRoot("XInputMapping")]
    public class XInputMapping
    {
        public XInputButton Start { get; set; }
        public XInputButton Up { get; set; }
        public XInputButton Down { get; set; }
        public XInputButton Left { get; set; }
        public XInputButton Right { get; set; }
        public XInputButton Button1 { get; set; }
        public XInputButton Button2 { get; set; }
        public XInputButton Button3 { get; set; }
        public XInputButton Button4 { get; set; }
        public XInputButton Button5 { get; set; }
        public XInputButton Button6 { get; set; }
        public XInputButton Service { get; set; }
        public XInputButton Test { get; set; }
        public XInputButton GasAxis { get; set; }
        public XInputButton BrakeAxis { get; set; }
        public XInputButton WheelAxis { get; set; }
        public XInputButton SonicItem { get; set; }
        public XInputButton SrcViewChange1 { get; set; }
        public XInputButton SrcViewChange2 { get; set; }
        public XInputButton SrcViewChange3 { get; set; }
        public XInputButton SrcViewChange4 { get; set; }
        public XInputButton SrcGearChange1 { get; set; }
        public XInputButton SrcGearChange2 { get; set; }
        public XInputButton SrcGearChange3 { get; set; }
        public XInputButton SrcGearChange4 { get; set; }
        public XInputButton GunUp { get; set; }
        public XInputButton GunDown { get; set; }
        public XInputButton GunLeft { get; set; }
        public XInputButton GunRight { get; set; }
        public XInputButton GunTrigger { get; set; }
        public int GunMultiplier { get; set; }
        public XInputButton InitialD6ShiftDown { get; set; }
        public XInputButton InitialD6ShiftUp { get; set; }
        public XInputButton InitialD6ViewChange { get; set; }
        public XInputButton InitialD6MenuUp { get; set; }
        public XInputButton InitialD6MenuDown { get; set; }
        public XInputButton InitialD6MenuLeft { get; set; }
        public XInputButton InitialD6MenuRight { get; set; }
    }

    [Serializable]
    [XmlRoot("JoystickMapping")]
    public class JoystickMapping
    {
        public JoystickButton Start { get; set; }
        public JoystickButton Up { get; set; }
        public JoystickButton Down { get; set; }
        public JoystickButton Left { get; set; }
        public JoystickButton Right { get; set; }
        public JoystickButton Button1 { get; set; }
        public JoystickButton Button2 { get; set; }
        public JoystickButton Button3 { get; set; }
        public JoystickButton Button4 { get; set; }
        public JoystickButton Button5 { get; set; }
        public JoystickButton Button6 { get; set; }
        public JoystickButton Service { get; set; }
        public JoystickButton Test { get; set; }
        public JoystickButton GasAxis { get; set; }
        public JoystickButton BrakeAxis { get; set; }
        public JoystickButton WheelAxis { get; set; }
        public JoystickButton SonicItem { get; set; }
        public JoystickButton SrcViewChange1 { get; set; }
        public JoystickButton SrcViewChange2 { get; set; }
        public JoystickButton SrcViewChange3 { get; set; }
        public JoystickButton SrcViewChange4 { get; set; }
        public JoystickButton SrcGearChange1 { get; set; }
        public JoystickButton SrcGearChange2 { get; set; }
        public JoystickButton SrcGearChange3 { get; set; }
        public JoystickButton SrcGearChange4 { get; set; }
        public JoystickButton GunUp { get; set; }
        public JoystickButton GunDown { get; set; }
        public JoystickButton GunLeft { get; set; }
        public JoystickButton GunRight { get; set; }
        public JoystickButton GunTrigger { get; set; }
        public int GunMultiplier { get; set; }
        public JoystickButton InitialD6ShiftDown { get; set; }
        public JoystickButton InitialD6ShiftUp { get; set; }
        public JoystickButton InitialD6ViewChange { get; set; }
        public JoystickButton InitialD6MenuUp { get; set; }
        public JoystickButton InitialD6MenuDown { get; set; }
        public JoystickButton InitialD6MenuLeft { get; set; }
        public JoystickButton InitialD6MenuRight { get; set; }
    }
}
