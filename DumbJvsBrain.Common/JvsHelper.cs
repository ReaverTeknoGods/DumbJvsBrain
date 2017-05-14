using System;
using System.Collections.Generic;

namespace DumbJvsBrain.Common
{
    public static class JvsHelper
    {
        public const byte JVS_BROADCAST = 0xFF;
        public const byte JVS_OP_RESET = 0xF0;
        public const byte JVS_OP_ADDRESS = 0xF1;
        public const byte JVS_SYNC_CODE = 0xE0;
        public const byte JVS_TRUE = 0x01;
        public const byte JVS_ADDR_MASTER = 0x00;
        public const byte JVS_COMMAND_REV = 0x13;
        public const byte JVS_READID_DATA = 0x10;

        public const string JVS_IDENTIFIER_Sega2005Jvs14572 = "\x1\x1SEGA CORPORATION;I/O BD JVS;837-14572;Ver1.00;2005/10\0";
        public const string JVS_IDENTIFIER_Sega1998Jvs13551 = "\x1\x1SEGA ENTERPRISES,LTD.;I/O BD JVS;837-13551 ;Ver1.00;98/10\0";
        public const string JVS_IDENTIFIER_NBGI_MarioKart3 = "\x1\x1NBGI.;NA-JV;Ver6.01;JPN,MK3100-1-NA-APR0-A01\0";
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

        public static int CalculateSto0ZWheelPos(int wheel)
        {
            // DEADZONE STUFF

            // OFFSET VALUE FOR CALCULATIONS
            int lx = wheel - 32767;

            // SETUP
            double deadzone = 0.25f * 32767;
            double magnitude = Math.Sqrt(lx * lx);
            double normalizedLX = lx / magnitude;
            double normalizedMagnitude = 0;

            // CALCULATE
            if (magnitude > deadzone)
            {
                if (magnitude > 32767) magnitude = 32767;

                magnitude -= deadzone;
                normalizedMagnitude = (normalizedLX * (magnitude / (32767 - deadzone))) + 1;
                var oldRange = 2;
                var newRange = 255;
                normalizedMagnitude = (normalizedMagnitude * newRange) / oldRange;
            }
            else
            {
                magnitude = 127.5;
                normalizedMagnitude = 127.5;
            }

            var finalMagnitude = Convert.ToInt32(normalizedMagnitude);
            return finalMagnitude;
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

        /// <summary>
        /// Crafts a valid JVS package.
        /// </summary>
        /// <param name="node">Target node.</param>
        /// <param name="bytes">package bytes.</param>
        /// <returns>Complete JVS package.</returns>
        public static byte[] CraftJvsPackage(byte node, byte[] bytes)
        {
            var packageBytes = new List<byte> { JVS_SYNC_CODE, node, (byte)(bytes.Length + 1) };
            packageBytes.AddRange(bytes);
            packageBytes.Add(CalcChecksum(0x00, bytes, bytes.Length));
            return packageBytes.ToArray();
        }

        /// <summary>
        /// Calculates JVS checksum.
        /// </summary>
        /// <param name="dest">Destination node.</param>
        /// <param name="bytes">The data.</param>
        /// <param name="length">Length</param>
        /// <returns></returns>
        public static byte CalcChecksum(int dest, byte[] bytes, int length)
        {
            var csum = dest + length + 1;

            for (var i = 0; i < length; i++)
                csum = (csum + bytes[i]) % 256;

            return (byte)csum;
        }
    }
}
