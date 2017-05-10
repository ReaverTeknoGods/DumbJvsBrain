using System;
using System.ComponentModel;

namespace DirtyJvsBrain
{
    public static class Helper
    {
        public static int CalculateGasPos(int gas)
        {
            int value = gas / (32625 / 255);
            if (value > 0xFF)
                value = 0xFF;
            return value;
        }
        public static int CalculateWheelPos(int wheel)
        {
            int value = wheel / (65535 / 255);
            return value;
        }
    }
    public static class AttributesHelperExtension
    {
        public static string ToDescription(this Enum value)
        {
            var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }
    }
}
