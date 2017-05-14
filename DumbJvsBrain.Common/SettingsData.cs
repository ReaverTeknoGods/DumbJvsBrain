using System;

namespace DumbJvsBrain.Common
{
    public class SettingsData
    {
        public SettingsData()
        {
            PlayerOneGuid = Guid.Empty;
            PlayerTwoGuid = Guid.Empty;
        }
        public Guid PlayerOneGuid { get; set; }
        public Guid PlayerTwoGuid { get; set; }
        public bool UseKeyboard { get; set; }
        public string VirtuaTennis4Dir { get; set; }
        public string MeltyBloodDir { get; set; }
        public string SegaRacingClassicDir { get; set; }
        public bool UseSto0zDrivingHack { get; set; }
    }
}
