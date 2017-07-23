using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using static DumbJvsBrain.Common.JvsHelper;

namespace DumbJvsBrain.Common
{
    public class JvsReply
    {
        public byte[] Bytes { get; set; }
        public int LengthReduction { get; set; }

        public bool Error { get; set; }
    }

    public static class JvsPackageEmulator
    {
        private static byte[] _lastPackage;
        private static bool CompareTwoArraysGipsyWay(byte[] array1, byte[] array2, int count)
        {
            for(var i = 0; i < count; i++)
                if (array1[i] != array2[i])
                    return false;
            return true;
        }
        /// <summary>
        /// Gets special bits for Digital.
        /// </summary>
        /// <returns>Bits for digital.</returns>
        private static byte GetSpecialBits()
        {
            byte result = 00;
            if (InputCode.PlayerOneButtons.Test.HasValue && InputCode.PlayerOneButtons.Test.Value)
                result |= 0x80;
            return result;
        }

        /// <summary>
        /// Gets Player 1 switch data.
        /// </summary>
        /// <returns>Bits for player 1 switch data.</returns>
        private static byte GetPlayer1Controls()
        {
            byte result = 0;
            if (InputCode.PlayerOneButtons.Start.HasValue && InputCode.PlayerOneButtons.Start.Value)
                result |= 0x80;
            if (InputCode.PlayerOneButtons.Service.HasValue && InputCode.PlayerOneButtons.Service.Value)
                result |= 0x40;
            if (InputCode.PlayerOneButtons.UpPressed())
                result |= 0x20;
            if (InputCode.PlayerOneButtons.DownPressed())
                result |= 0x10;
            if (InputCode.PlayerOneButtons.LeftPressed())
                result |= 0x08;
            if (InputCode.PlayerOneButtons.RightPressed())
                result |= 0x04;
            if (InputCode.PlayerOneButtons.Button1.HasValue && InputCode.PlayerOneButtons.Button1.Value)
                result |= 0x02;
            if (InputCode.PlayerOneButtons.Button2.HasValue && InputCode.PlayerOneButtons.Button2.Value)
                result |= 0x01;
            return result;
        }

        /// <summary>
        /// Gets Player 1 extended switch data.
        /// </summary>
        /// <returns>Bits for player 1 extended switch data.</returns>
        private static byte GetPlayer1ControlsExt()
        {
            byte result = 0;
            if (InputCode.PlayerOneButtons.Button3.HasValue && InputCode.PlayerOneButtons.Button3.Value)
                result |= 0x80;
            if (InputCode.PlayerOneButtons.Button4.HasValue && InputCode.PlayerOneButtons.Button4.Value)
                result |= 0x40;
            if (InputCode.PlayerOneButtons.Button5.HasValue && InputCode.PlayerOneButtons.Button5.Value)
                result |= 0x20;
            if (InputCode.PlayerOneButtons.Button6.HasValue && InputCode.PlayerOneButtons.Button6.Value)
                result |= 0x10;
            return result;
        }

        /// <summary>
        /// Gets Player 2 switch data.
        /// </summary>
        /// <returns>Bits for player 2 switch data.</returns>
        private static byte GetPlayer2Controls()
        {
            byte result = 0;
            if (InputCode.PlayerTwoButtons.Start.HasValue && InputCode.PlayerTwoButtons.Start.Value)
                result |= 0x80;
            if (InputCode.PlayerTwoButtons.Service.HasValue && InputCode.PlayerTwoButtons.Service.Value)
                result |= 0x40;
            if (InputCode.PlayerTwoButtons.UpPressed())
                result |= 0x20;
            if (InputCode.PlayerTwoButtons.DownPressed())
                result |= 0x10;
            if (InputCode.PlayerTwoButtons.LeftPressed())
                result |= 0x08;
            if (InputCode.PlayerTwoButtons.RightPressed())
                result |= 0x04;
            if (InputCode.PlayerTwoButtons.Button1.HasValue && InputCode.PlayerTwoButtons.Button1.Value)
                result |= 0x02;
            if (InputCode.PlayerTwoButtons.Button2.HasValue && InputCode.PlayerTwoButtons.Button2.Value)
                result |= 0x01;
            return result;
        }

        /// <summary>
        /// Gets Player 2 extended switch data.
        /// </summary>
        /// <returns>Bits for player 2 extended switch data.</returns>
        private static byte GetPlayer2ControlsExt()
        {
            byte result = 0;
            if (InputCode.PlayerTwoButtons.Button3.HasValue && InputCode.PlayerTwoButtons.Button3.Value)
                result |= 0x80;
            if (InputCode.PlayerTwoButtons.Button4.HasValue && InputCode.PlayerTwoButtons.Button4.Value)
                result |= 0x40;
            if (InputCode.PlayerTwoButtons.Button5.HasValue && InputCode.PlayerTwoButtons.Button5.Value)
                result |= 0x20;
            if (InputCode.PlayerTwoButtons.Button6.HasValue && InputCode.PlayerTwoButtons.Button6.Value)
                result |= 0x10;
            return result;
        }

        public static JvsReply ParsePackage(byte[] bytesLeft, bool multiPackage)
        {
            JvsReply reply = new JvsReply();
            // We take first byte of the package
            switch (bytesLeft[0])
            {
                case JVS_OP_ADDRESS:
                    return JvsGetAddress(bytesLeft, reply);
                case 0x10:
                    return JvsGetIdentifier(reply);
                case 0x11:
                    return JvsGetCommandRev(reply, multiPackage);
                case 0x12:
                    return JvsGetJvsVersion(reply, multiPackage);
                case 0x13:
                    return JvsGetCommunicationVersion(reply, multiPackage);
                case 0x14:
                    return JvsGetSlaveFeatures(reply, multiPackage);
                case 0x15:
                    return JvsConveyMainBoardId(bytesLeft, reply);
                case 0x20:
                    return JvsGetDigitalReply(bytesLeft, reply, multiPackage);
                case 0x21:
                    return JvsGetCoinReply(bytesLeft, reply, multiPackage);
                case 0x22:
                    return JvsGetAnalogReply(bytesLeft, reply, multiPackage);
                case 0x2F:
                    return JvsReTransmitData(reply);
                case 0x30:
                case 0x31:
                    return JvsGetCoinReduce(reply, multiPackage);
                case 0x32:
                    return JvsGeneralPurposeOutput(bytesLeft, reply, multiPackage);
                case 0x37:
                    return JvsGeneralPurposeOutput2(bytesLeft, reply, multiPackage);
            }
            Console.WriteLine($"Unknown package, contact Reaver! Package: {ByteArrayToString(bytesLeft)}");
            reply.Error = true;
            return reply;
        }

        private static JvsReply JvsReTransmitData(JvsReply reply)
        {
            reply.LengthReduction = 1;
            reply.Bytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00};
            return reply;
        }

        private static JvsReply JvsGetCoinReduce(JvsReply reply, bool multiPackage)
        {
            reply.LengthReduction = 4;
            reply.Bytes = !multiPackage ? new byte[] { } : new byte[] { 0x03 };
            return reply;
        }

        private static JvsReply JvsGeneralPurposeOutput(byte[] bytesLeft, JvsReply reply, bool multiPackage)
        {
            var byteCount = bytesLeft[1];
            reply.LengthReduction = byteCount + 2; // Command Code + Size + Outputs

            // Special invalid package from Virtua-R Limit
            //if(bytesLeft.Length > 4)
            //    if (bytesLeft[byteCount + 2] == 0x00)
            //        reply.LengthReduction++;

            reply.Bytes = !multiPackage ? new byte[] { } : new byte[] { 0x01 };
            return reply;
        }

        private static JvsReply JvsGeneralPurposeOutput2(byte[] bytesLeft, JvsReply reply, bool multiPackage)
        {
            reply.LengthReduction = 3; // Command Code + Size + Outputs
            reply.Bytes = !multiPackage ? new byte[] { } : new byte[] { 0x01 };
            return reply;
        }

        private static JvsReply JvsGetAddress(byte[] bytesLeft, JvsReply reply)
        {
            if (bytesLeft[1] != 0x01)
            {
                MessageBox.Show($"Unsupported JVS_OP_ADDRESS package, contact Reaver! Package: {ByteArrayToString(bytesLeft)}");
                throw new NotSupportedException();
            }
            reply.Bytes = new byte[] {};
            reply.LengthReduction = 2;
            return reply;
        }

        private static JvsReply JvsGetIdentifier(JvsReply reply)
        {
            reply.LengthReduction = 1;
            reply.Bytes = Encoding.ASCII.GetBytes(JVS_IDENTIFIER_Sega2005Jvs14572);
            return reply;
        }

        private static JvsReply JvsGetCommunicationVersion(JvsReply reply, bool multiPackage)
        {
            reply.LengthReduction = 1;
            reply.Bytes = multiPackage ? new byte[] { 0x01, 0x10 } : new byte[] { 0x10 };
            return reply;
        }

        private static JvsReply JvsGetJvsVersion(JvsReply reply, bool multiPackage)
        {
            reply.LengthReduction = 1;
            reply.Bytes = multiPackage ? new byte[] { 0x01, 0x20 } : new byte[] { 0x20 };
            return reply;
        }

        private static JvsReply JvsGetCommandRev(JvsReply reply, bool multiPackage)
        {
            reply.LengthReduction = 1;
            reply.Bytes = multiPackage ? new byte[] { 0x01, 0x13 } : new byte[] { 0x13 };
            return reply;
        }

        private static JvsReply JvsGetSlaveFeatures(JvsReply reply, bool multiPackage)
        {
            reply.LengthReduction = 1;
            reply.Bytes = multiPackage
                ? new byte[]
                {
                    0x01, 0x01, 0x02, 0x0E, 0x00, 0x02, 0x02, 0x00, 0x00, 0x03, 0x08, 0x0A, 0x00, 0x12, 0x14, 0x00,
                    0x00, 0x00
                }
                : new byte[]
                {
                    0x01, 0x02, 0x0E, 0x00, 0x02, 0x02, 0x00, 0x00, 0x03, 0x08, 0x0A, 0x00, 0x12, 0x14, 0x00, 0x00, 0x00
                };
            return reply;
        }

        private static JvsReply JvsConveyMainBoardId(byte[] bytesLeft, JvsReply reply)
        {
            for (var i = 0; i < bytesLeft.Length; i++)
            {
                if (i == 0x00)
                    break;
                reply.LengthReduction++;
            }
            reply.LengthReduction++;
            reply.Bytes = new byte[]
            {
                0x01, 0x01, 0x05
            };
            return reply;
        }

        private static JvsReply JvsGetAnalogReply(byte[] bytesLeft, JvsReply reply, bool multiPackage)
        {
            var byteLst = new List<byte>();
            int channelCount = 0;
            channelCount = bytesLeft.Length == 1 ? 8 : bytesLeft[1]; // Stupid hack for Virtua-R Limit
            reply.LengthReduction = 2;

            if (multiPackage)
                byteLst.Add(0x01);
            for (int i = 0; i < channelCount; i++)
            {
                byteLst.Add(InputCode.AnalogBytes[(i * 2)]);
                byteLst.Add(InputCode.AnalogBytes[(i * 2)+1]);
            }
            reply.Bytes = byteLst.ToArray();
            return reply;
        }

        private static JvsReply JvsGetCoinReply(byte[] bytesLeft, JvsReply reply, bool multiPackage)
        {
            var slotCount = bytesLeft[1];
            reply.LengthReduction = 2;
            if (slotCount == 0)
            {
                reply.Bytes = !multiPackage ? new byte[] { } : new byte[] { 0x01 };
            }
            else if (slotCount == 1)
            {
                reply.Bytes = !multiPackage ? new byte[] { 0x00, 0x00 } : new byte[] { 0x01, 0x00, 0x00 };
            }
            else if (slotCount == 2)
            {
                reply.Bytes = !multiPackage ? new byte[] { 0x00, 0x00, 0x00, 0x00 } : new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00 };
            }
            else
            {
                MessageBox.Show($"Unknown READ_COIN_INPUTS package, contact Reaver!  Package: {ByteArrayToString(bytesLeft)}");
                throw new NotSupportedException();
            }
            return reply;
        }

        private static JvsReply JvsGetDigitalReply(byte[] bytesLeft, JvsReply reply, bool multiPackage)
        {
            var byteLst = new List<byte>();
            var players = bytesLeft[1];
            var bytesToRead = bytesLeft[2];
            if (multiPackage)
                byteLst.Add(0x01);
            byteLst.Add(GetSpecialBits());
            if (players > 2)
            {
                MessageBox.Show($"Why would you have more than 2 players?  Package: {ByteArrayToString(bytesLeft)}", "Contact Reaver asap!",
                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                throw new NotSupportedException();
            }
            if (players != 0)
            {
                byteLst.Add(GetPlayer1Controls());
                bytesToRead--;
                if (bytesToRead != 0)
                {
                    byteLst.Add(GetPlayer1ControlsExt());
                    bytesToRead--;
                }
                while (bytesToRead != 0)
                {
                    byteLst.Add(0x00);
                    bytesToRead--;
                }
                if (players == 2)
                {
                    bytesToRead = bytesLeft[2];
                    byteLst.Add(GetPlayer2Controls());
                    bytesToRead--;
                    if (bytesToRead != 0)
                    {
                        byteLst.Add(GetPlayer2ControlsExt());
                        bytesToRead--;
                    }
                    while (bytesToRead != 0)
                    {
                        byteLst.Add(0x00);
                        bytesToRead--;
                    }
                }
            }
            reply.LengthReduction = 3;
            reply.Bytes = byteLst.ToArray();
            return reply;
        }

        private static byte[] AdnvacedJvs(byte[] data)
        {
            // Disect package (take out unwanted data)
            var byteLst = new List<byte>();
            var multiPackage = false;
            var replyBytes = new List<byte>();
            var packageSize = data[2] - 1; // Reduce CRC as we don't need that
            for (int i = 0; i < data.Length; i++)
            {
                if (i == 0 || i == 1 || i == 2 || i == data.Length-1)
                    continue;
                byteLst.Add(data[i]);
            }
            for (var i = 0; i < packageSize;)
            {
                var reply = ParsePackage(byteLst.ToArray(), multiPackage);
                if (reply.Error)
                    return null;
                for (int x = 0; x < reply.LengthReduction; x++)
                {
                    if(byteLst.Count != 0)
                        byteLst.RemoveAt(0);
                }
                i += reply.LengthReduction;
                replyBytes.AddRange(reply.Bytes);
                multiPackage = true;
            }
            _lastPackage = replyBytes.ToArray();
            return replyBytes.ToArray();
        }

        /// <summary>
        /// THIS CODE IS BEYOND RETARTED AND FOR HACKY TESTS ONLY!!!
        /// For proper JVS handling: must code proper detection of packages, multiple requests in one package and proper responses!
        /// Now we just know what SEGA asks and return back like bunch of monkey boys.
        /// Feel free to improve.
        /// </summary>
        /// <param name="data">Input data from the com port.</param>
        /// <returns>"proper" response.</returns>
        public static byte[] GetReply(byte[] data)
        {
            // We don't care about these kind of packages, need to improve in case comes with lot of delay etc.
            if (data.Length <= 3)
                return new byte[0];
            //Console.WriteLine("Package: " + ByteArrayToString(data));
            switch (data[3])
            {
                // E0FF03F0D9CB
                case JVS_OP_RESET:
                    {
                        return new byte[0];
                    }
                default:
                {
                    var reply = CraftJvsPackageWithStatusAndReport(0, AdnvacedJvs(data));
                    //Console.WriteLine("Reply: " + ByteArrayToString(reply));
                    return reply;
                }
            }
        }
    }
}
