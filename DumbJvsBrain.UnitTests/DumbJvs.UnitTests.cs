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
            var requestBytes = CraftJvsPackage(JVS_BROADCAST, new byte[] { JVS_OP_ADDRESS, 0x01 });
            var espectedBytes = CraftJvsPackage(0, new byte[] { 0x01, 0x01 });

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }

        [TestMethod]
        public void JVS_GETIDENTIFIER_ShouldReturnIdentifier()
        {
            // Arrange
            var requestBytes = CraftJvsPackage(1, new byte[] { JVS_READID_DATA });
            var espectedBytes = CraftJvsPackage(0, Encoding.ASCII.GetBytes(JVS_IDENTIFIER_Sega2005Jvs14572));

            // Act
            var reply = GetReply(requestBytes);

            // Assert
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.Length, espectedBytes.Length);
            Assert.IsTrue(reply.SequenceEqual(espectedBytes));
        }
    }
}
