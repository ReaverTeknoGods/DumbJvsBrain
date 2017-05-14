using System;

namespace DumbJvsBrain.Common
{
    public enum Direction
    {
        HorizontalCenter,
        VerticalCenter,
        Left,
        Right,
        Up,
        Down
    }

    public class PlayerButtons
    {
        public Guid JoystickGuid { get; set; }
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Button1 { get; set; }
        public bool Button2 { get; set; }
        public bool Button3 { get; set; }
        public bool Button4 { get; set; }
        public bool Button5 { get; set; }
        public bool Button6 { get; set; }
        public bool Start { get; set; }
        public bool Service { get; set; }
        public bool Test { get; set; }
    }
    public static class InputCode
    {
        public static void SetPlayerDirection(PlayerButtons playerButtons, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    playerButtons.Up = true;
                    playerButtons.Down = false;
                    break;
                case Direction.Down:
                    playerButtons.Up = false;
                    playerButtons.Down = true;
                    break;
                case Direction.VerticalCenter:
                    playerButtons.Up = false;
                    playerButtons.Down = false;
                    break;
                case Direction.HorizontalCenter:
                    playerButtons.Left = false;
                    playerButtons.Right = false;
                    break;
                case Direction.Left:
                    playerButtons.Left = true;
                    playerButtons.Right = false;
                    break;
                case Direction.Right:
                    playerButtons.Left = false;
                    playerButtons.Right = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static GameProfiles ButtonMode { get; set; }

        public static bool ShiftUp { get; set; }

        public static bool ShiftDown { get; set; }

        public static bool ViewChange { get; set; }

        public static int Wheel { get; set; }

        public static int Gas { get; set; }

        public static int Brake { get; set; }

        public static PlayerButtons PlayerOneButtons = new PlayerButtons();

        public static PlayerButtons PlayerTwoButtons = new PlayerButtons();
    }
}
