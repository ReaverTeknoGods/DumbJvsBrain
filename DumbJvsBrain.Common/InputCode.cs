using System;

namespace DumbJvsBrain.Common
{
    public enum GunHorizontalDirection
    {
        HorizontalCenter,
        Up,
        Down
    }
    public enum GunVerticalDirection
    {
        VerticalCenter,
        Left,
        Right,
    }
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
        public bool LeftPressed()
        {
            return Left.HasValue && Left.Value;
        }
        public bool RightPressed()
        {
            return Right.HasValue && Right.Value;
        }
        public bool UpPressed()
        {
            return Up.HasValue && Up.Value;
        }
        public bool DownPressed()
        {
            return Down.HasValue && Down.Value;
        }
        private bool? _up;
        private bool? _down;
        private bool? _right;
        private bool? _left;
        private bool? _button1;
        private bool? _button2;
        private bool? _button3;
        private bool? _button4;
        private bool? _button5;
        private bool? _button6;
        private bool? _start;
        private bool? _service;
        private bool? _test;
        public Guid JoystickGuid { get; set; }

        public bool? Up
        {
            get { return _up; }
            set { if(value != null) _up = value; }
        }

        public bool? Down
        {
            get { return _down; }
            set { if (value != null) _down = value; }
        }

        public bool? Left
        {
            get { return _left; }
            set { if (value != null) _left = value; }
        }

        public bool? Right
        {
            get { return _right; }
            set { if (value != null) _right = value; }
        }

        public bool? Button1
        {
            get { return _button1; }
            set { if (value != null) _button1 = value; }
        }

        public bool? Button2
        {
            get { return _button2; }
            set { if (value != null) _button2 = value; }
        }

        public bool? Button3
        {
            get { return _button3; }
            set { if (value != null) _button3 = value; }
        }

        public bool? Button4
        {
            get { return _button4; }
            set { if (value != null) _button4 = value; }
        }

        public bool? Button5
        {
            get { return _button5; }
            set { if (value != null) _button5 = value; }
        }

        public bool? Button6
        {
            get { return _button6; }
            set { if (value != null) _button6 = value; }
        }

        public bool? Start
        {
            get { return _start; }
            set { if (value != null) _start = value; }
        }

        public bool? Service
        {
            get { return _service; }
            set { if (value != null) _service = value; }
        }

        public bool? Test
        {
            get { return _test; }
            set { if (value != null) _test = value; }
        }
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

        public static byte[] AnalogBytes = new byte[16];

        public static PlayerButtons PlayerOneButtons = new PlayerButtons();

        public static PlayerButtons PlayerTwoButtons = new PlayerButtons();
    }
}
