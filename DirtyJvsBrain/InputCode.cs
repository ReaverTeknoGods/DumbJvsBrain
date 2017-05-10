using System.ComponentModel;

namespace DirtyJvsBrain
{
    public enum GameSelection
    {
        [Description("Initial D6AA")]
        InitialD6 = 0,
        [Description("Virtua Tennis 4")]
        VirtuaTennis4 = 1,
        //[Description("Sega Sonic All-Stars Racing")]
        //SegaSonic = 2,
    }
    public static class InputCode
    {
        public static GameSelection ButtonMode { get; set; }
        public static bool Start1 { get; set; }

        public static bool Start2 { get; set; }

        public static bool Test { get; set; }

        public static bool Service1 { get; set; }

        public static bool ShiftUp { get; set; }

        public static bool ShiftDown { get; set; }

        public static bool ViewChange { get; set; }

        public static int Wheel { get; set; }

        public static int Gas { get; set; }

        public static int Brake { get; set; }

        public static bool Player1Button1 { get; set; }
        public static bool Player1Button2 { get; set; }
        public static bool Player1Button3 { get; set; }
        public static bool Player1Button4 { get; set; }
        public static bool Player1Button5 { get; set; }
        public static bool Player1Button6 { get; set; }
        public static bool Player1Up { get; set; }
        public static bool Player1Down { get; set; }
        public static bool Player1Left { get; set; }
        public static bool Player1Right { get; set; }

        public static bool Player2Button1 { get; set; }
        public static bool Player2Button2 { get; set; }
        public static bool Player2Button3 { get; set; }
        public static bool Player2Button4 { get; set; }
        public static bool Player2Button5 { get; set; }
        public static bool Player2Button6 { get; set; }
        public static bool Player2Up { get; set; }
        public static bool Player2Down { get; set; }
        public static bool Player2Left { get; set; }
        public static bool Player2Right { get; set; }
        public static bool Service2 { get; set; }
    }
}
