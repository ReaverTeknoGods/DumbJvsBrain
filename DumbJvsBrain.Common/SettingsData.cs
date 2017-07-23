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
        public string LgiDir { get; set; }
        public string MeltyBloodDir { get; set; }
        public string SegaRacingClassicDir { get; set; }
        public string SegaSonicDir { get; set; }
        public string SegaDreamRaidersDir { get; set; }
        public bool UseSto0ZDrivingHack { get; set; }
        public bool UseMouse { get; set; }
        public string InitialD6Dir { get; set; }
        public bool XInputMode { get; set; }
        public string GoldenGunDir { get; set; }
    }
}
