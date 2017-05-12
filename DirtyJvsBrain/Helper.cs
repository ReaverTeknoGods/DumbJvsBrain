using System;
using System.ComponentModel;

namespace DirtyJvsBrain
{
    public static class Helper
    {
        /// <summary>
        /// Calculates gas position.
        /// </summary>
        /// <param name="gas">Joystick axis value.</param>
        /// <returns>JVS friendly value.</returns>
        public static int CalculateGasPos(int gas)
        {
            var value = gas / (32625 / 255);
            if (value > 0xFF)
                value = 0xFF;
            return value;
        }

        /// <summary>
        /// Calculates wheel position.
        /// </summary>
        /// <param name="wheel">Joystick axis value.</param>
        /// <returns>JVS friendly value.</returns>
        public static int CalculateWheelPos(int wheel)
        {
            int value = wheel / (65535 / 255);
            return value;
        }
    }

    /// <summary>
    /// Used for enums to get the description..
    /// </summary>
    public static class AttributesHelperExtension
    {
        public static string ToDescription(this Enum value)
        {
            var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }
    }
}
