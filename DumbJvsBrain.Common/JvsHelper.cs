using System;
using System.Collections.Generic;
using System.Text;

namespace DumbJvsBrain.Common
{
    public static class JvsHelper
    {
        public const byte JVS_BROADCAST = 0xFF;
        public const byte JVS_OP_RESET = 0xF0;
        public const byte JVS_OP_ADDRESS = 0xF1;
        public const byte JVS_SYNC_CODE = 0xE0;
        public const byte JVS_TRUE = 0x01;
        public const byte JVS_REPORT_OK = 0x01;
        public const byte JVS_REPORT_ERROR1 = 0x02;
        public const byte JVS_REPORT_ERROR2 = 0x03;
        public const byte JVS_REPORT_DEVICE_BUSY = 0x04;
        public const byte JVS_STATUS_OK = 0x01;
        public const byte JVS_STATUS_UNKNOWN = 0x02;
        public const byte JVS_STATUS_CHECKSUM_FAIL = 0x03;
        public const byte JVS_STATUS_OVERFLOW = 0x04;
        public const byte JVS_ADDR_MASTER = 0x00;
        public const byte JVS_COMMAND_REV = 0x13;
        public const byte JVS_READID_DATA = 0x10;
        public const byte JVS_READ_DIGITAL = 0x20;
        public const byte JVS_READ_COIN = 0x21;
        public const byte JVS_READ_ANALOG = 0x22;
        public const byte JVS_READ_ROTATORY = 0x23;
        public const byte JVS_COIN_NORMAL_OPERATION = 0x00;
        public const byte JVS_COIN_COIN_JAM = 0x01;
        public const byte JVS_COIN_SYSTEM_DISCONNECTED = 0x02;
        public const byte JVS_COIN_SYSTEM_BUSY = 0x03;

        public const string JVS_IDENTIFIER_Sega2005Jvs14572 = "SEGA CORPORATION;I/O BD JVS;837-14572;Ver1.00;2005/10\0";
        public const string JVS_IDENTIFIER_Sega1998Jvs13551 = "SEGA ENTERPRISES,LTD.;I/O BD JVS;837-13551 ;Ver1.00;98/10\0";
        public const string JVS_IDENTIFIER_NBGI_MarioKart3 = "NBGI.;NA-JV;Ver6.01;JPN,MK3100-1-NA-APR0-A01\0";
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
        /// Crafts a valid JVS package with status and report.
        /// </summary>
        /// <param name="node">Target node.</param>
        /// <param name="bytes">package bytes.</param>
        /// <returns>Complete JVS package.</returns>
        public static byte[] CraftJvsPackageWithStatusAndReport(byte node, byte[] bytes)
        {
            var packageBytes = new List<byte>
            {
                JVS_SYNC_CODE,
                node,
                (byte) (bytes.Length + 3), // +3 because of Status bytes and CRC.
                JVS_STATUS_OK,
                JVS_REPORT_OK
            };
            packageBytes.AddRange(bytes);
            packageBytes.Add(CalcChecksumAndAddStatusAndReport(0x00, bytes, bytes.Length));
            return packageBytes.ToArray();
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

        public static byte CalcChecksumAndAddStatusAndReport(int dest, byte[] bytes, int length)
        {
            var packageForCalc = new List<byte>();
            packageForCalc.Add(JVS_STATUS_OK);
            packageForCalc.Add(JVS_REPORT_OK);
            packageForCalc.AddRange(bytes);
            return CalcChecksum(dest, packageForCalc.ToArray(), packageForCalc.Count);
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

        /// <summary>
        /// Converts byte array to string
        /// </summary>
        /// <param name="ba">Byte array.</param>
        /// <returns>Parsed string.</returns>
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
