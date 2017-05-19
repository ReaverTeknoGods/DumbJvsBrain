using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using SharpDX.DirectInput;

namespace DumbJvsBrain.Common
{
    public class JoystickHelper
    {
        /// <summary>
        /// Gets joysticks and gamepads connected to the PC.
        /// </summary>
        /// <returns>List of joystick profiles.</returns>
        public static List<JoystickProfile> GetAvailableJoysticks()
        {
            var list = new List<JoystickProfile>();
            using (var directInput = new DirectInput())
            {
                list.AddRange(directInput.GetDevices().Where(x => x.Type != DeviceType.Mouse && x.Type != DeviceType.Keyboard).ToList().Select(deviceInstance => new JoystickProfile
                {
                    InstanceGuid = deviceInstance.InstanceGuid, ProductName = deviceInstance.ProductName
                }));
            }
            return list;
        }
        /// <summary>
        /// Serializes SettingsData class to a SettingsData.xml file.
        /// </summary>
        /// <param name="joystick"></param>
        public static void Serialize(SettingsData joystick)
        {
            var serializer = new XmlSerializer(joystick.GetType());
            using (var writer = XmlWriter.Create("SettingsData.xml"))
            {
                serializer.Serialize(writer, joystick);
            }
        }

        /// <summary>
        /// Deserializes SettingsData.xml to the class.
        /// </summary>
        /// <returns>Read SettingsData class.</returns>
        public static SettingsData DeSerialize()
        {
            var serializer = new XmlSerializer(typeof(SettingsData));
            using (var reader = XmlReader.Create("SettingsData.xml"))
            {
                var joystick = (SettingsData)serializer.Deserialize(reader);
                return joystick;
            }
        }
    }
}
