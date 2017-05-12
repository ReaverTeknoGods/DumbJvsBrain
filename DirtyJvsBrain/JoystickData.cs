using System;

namespace DirtyJvsBrain
{
    public class JoystickData
    {
        public JoystickData()
        {
            PlayerOneGuid = Guid.Empty;
            PlayerTwoGuid = Guid.Empty;
        }
        public Guid PlayerOneGuid { get; set; }
        public Guid PlayerTwoGuid { get; set; }
    }
}
