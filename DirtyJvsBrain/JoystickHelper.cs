using System.Xml;
using System.Xml.Serialization;

namespace DirtyJvsBrain
{
    public static class JoystickHelper
    {
        /// <summary>
        /// Serializes JoystickData class to a joystick.xml file.
        /// </summary>
        /// <param name="joystick"></param>
        public static void Serialize(JoystickData joystick)
        {
            var serializer = new XmlSerializer(joystick.GetType());
            using (var writer = XmlWriter.Create("joystick.xml"))
            {
                serializer.Serialize(writer, joystick);
            }
        }

        /// <summary>
        /// Deserializes joystick.xml to the class.
        /// </summary>
        /// <returns>Read JoystickData class.</returns>
        public static JoystickData DeSerialize()
        {
            var serializer = new XmlSerializer(typeof(JoystickData));
            using (var reader = XmlReader.Create("joystick.xml"))
            {
                var joystick = (JoystickData)serializer.Deserialize(reader);
                return joystick;
            }
        }
    }
}
