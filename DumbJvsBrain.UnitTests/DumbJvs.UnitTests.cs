using System.Linq;
using System.Text;
using DumbJvsBrain.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static DumbJvsBrain.Common.JvsPackageEmulator;
using static DumbJvsBrain.Common.JvsHelper;

namespace DumbJvsBrain.UnitTests
{
    [TestClass]
    public class DumbsJvsUnitTests
    {
        [TestMethod]
        public void JVS_RESET_ShouldReturnNothing()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(JVS_BROADCAST, new byte[] { JVS_OP_RESET, 0xD9 });

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, 0);
        }

        [TestMethod]
        public void JVS_ADDRESS_ShouldReturnPackage()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(JVS_BROADCAST, new byte[] { JVS_OP_ADDRESS, 0x01 }); // 0x01 = Bus address
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] {});

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_GET_IDENTIFIER_ShouldReturnIdentifier()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(1, new byte[] { JVS_READID_DATA });
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, Encoding.ASCII.GetBytes(JVS_IDENTIFIER_Sega2005Jvs14572));

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_GET_ANALOG_ShouldReturnThreeChannels()
        {
            // Arrange
            InputCode.Wheel = 0xBA;
            InputCode.Gas = 0xBE;
            InputCode.Brake = 0xBE;
            var requestBytes = CraftJvsPackage(1, new byte[] { JVS_READ_ANALOG, 0x03 }); // 22 = REQUEST ANALOG, 3 = 3 Channels
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { 0x00, (byte)InputCode.Wheel, 0x00, (byte)InputCode.Gas, 0x00, (byte)InputCode.Brake});

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_READ_DIGINAL_ShouldReturnPlayerOneAndPlayerTwoButtonsAndExt()
        {
            // Arrange
            InputCode.PlayerOneButtons.Button1 = true;
            InputCode.PlayerOneButtons.Button4 = true;
            InputCode.PlayerTwoButtons.Button1 = true;
            InputCode.PlayerTwoButtons.Button4 = true;
            InputCode.PlayerOneButtons.Test = true;
            var requestBytes = CraftJvsPackage(1, new byte[] { JVS_READ_DIGITAL, 0x02, 0x02 }); // 22 = REQUEST DIGITAL, 2 = Player Count, 2 Bytes Per Player
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { 0x80, 0x02, 0x40, 0x02, 0x40 }); // Special Switches, P1, P1Ext, P2, P2Ext

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_READ_DIGINAL_READ_ANALOG_ShouldReturnPlayerOneAndPlayerTwoButtonsAndExtAndThreeAnalogChannels()
        {
            // Arrange
            InputCode.PlayerOneButtons.Button1 = true;
            InputCode.PlayerOneButtons.Button4 = true;
            InputCode.PlayerTwoButtons.Button1 = true;
            InputCode.PlayerTwoButtons.Button4 = true;
            InputCode.PlayerOneButtons.Test = true;
            InputCode.Wheel = 0xBA;
            InputCode.Gas = 0xBE;
            InputCode.Brake = 0xBE;
            var requestBytes = CraftJvsPackage(1, new byte[] { JVS_READ_DIGITAL, 0x02, 0x02, JVS_READ_ANALOG, 0x03 }); // 22 = REQUEST DIGITAL, 2 = Player Count, 2 Bytes Per Player, 22 = REQUEST ANALOG, 3 = 3 Channels
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { 0x80, 0x02, 0x40, 0x02, 0x40, 0x00, (byte)InputCode.Wheel, 0x00, (byte)InputCode.Gas, 0x00, (byte)InputCode.Brake }); // Special Switches, P1, P1Ext, P2, P2Ext

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_READ_DIGINAL_ShouldReturnPlayerOneAndPlayerTwoButtons()
        {
            // Arrange
            InputCode.PlayerOneButtons.Button1 = true;
            InputCode.PlayerTwoButtons.Button1 = true;
            InputCode.PlayerOneButtons.Test = true;
            var requestBytes = CraftJvsPackage(1, new byte[] { JVS_READ_DIGITAL, 0x02, 0x01 }); // 22 = REQUEST DIGITAL, 2 = Player Count, 1 Bytes Per Player
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { 0x80, 0x02, 0x02 }); // Special Switches, P1, P2

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_READ_DIGINAL_ShouldReturnPlayerOne()
        {
            // Arrange
            InputCode.PlayerOneButtons.Button1 = true;
            InputCode.PlayerOneButtons.Test = true;
            var requestBytes = CraftJvsPackage(1, new byte[] { JVS_READ_DIGITAL, 0x01, 0x01 }); // 22 = REQUEST DIGITAL, 1 = Player Count, 1 Bytes Per Player
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { 0x80, 0x02 }); // Special Switches, P1

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_READ_DIGINAL_ShouldReturnPlayerOneExt()
        {
            // Arrange
            InputCode.PlayerOneButtons.Button1 = true;
            InputCode.PlayerOneButtons.Button4 = true;
            InputCode.PlayerOneButtons.Test = true;
            var requestBytes = CraftJvsPackage(1, new byte[] { JVS_READ_DIGITAL, 0x01, 0x02 }); // 22 = REQUEST DIGITAL, 1 = Player Count, 2 Bytes Per Player
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { 0x80, 0x02, 0x40 }); // Special Switches, P1, P1Ext

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_READ_COIN_ShouldReturnOneCoinSlotWithOkStatus()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(1, new byte[] { JVS_READ_COIN, 0x01 }); // 22 = Request coin slots, 1 slot
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { JVS_COIN_NORMAL_OPERATION, 0x00 }); // Coin Normal Operation, 0 coins inserted.

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_READ_COIN_ShouldReturnTwoCoinSlotsWithOkStatus()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(1, new byte[] { JVS_READ_COIN, 0x02 }); // 22 = Request coin slots, 2 slots
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { JVS_COIN_NORMAL_OPERATION, 0x00, JVS_COIN_NORMAL_OPERATION, 0x00 }); // Coin Normal Operation, 0 coins inserted. x 2

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_GET_COMMANDREV_ShouldReturnVersionOnePointThree()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(1, new byte[] { 0x11 });
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { 0x13 }); // Revision 1.3

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }


        [TestMethod]
        public void JVS_GET_JVSVERSION_ShouldReturnVersionTwoPointZero()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(1, new byte[] { 0x12 });
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { 0x20 }); // Version 2.0

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_GET_COMMUNICATIONVERSION_ShouldReturnVersionTwoPointZero()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(1, new byte[] { 0x13 });
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { 0x10 }); // Version 1.0

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_GET_COMMUNICATIONVERSION_JVSVERSION_COMMANDREV_ShouldReturnRightValues()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(1, new byte[] { 0x13, 0x12, 0x11 });
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { 0x10, 0x01, 0x20, 0x01, 0x13 });

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_GENERALPURPOSEOUTPUT_ShouldReturnJVSOK_REPORTOK()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(1, new byte[] { 0x32, 0x02, 0x00, 0x00 });
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { });

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_GENERALPURPOSEOUTPUT2_ShouldReturnJVSOK_REPORTOK()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(1, new byte[] { 0x32, 0x03, 0x00, 0x00, 0x00 });
            var espectedBytes = CraftJvsPackageWithStatusAndReport(0, new byte[] { });

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }
    }
}
