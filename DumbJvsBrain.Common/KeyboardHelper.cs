using System.Xml;
using System.Xml.Serialization;

namespace DumbJvsBrain.Common
{
    public static class KeyboardHelper
    {
        /// <summary>
        /// Serializes KeyboardMap class to a KeyboardMap.xml file.
        /// </summary>
        /// <param name="keyboard"></param>
        public static void Serialize(KeyboardMap keyboard)
        {
            var serializer = new XmlSerializer(keyboard.GetType());
            using (var writer = XmlWriter.Create("KeyboardMap.xml"))
            {
                serializer.Serialize(writer, keyboard);
            }
        }

        /// <summary>
        /// Deserializes KeyboardMap.xml to the class.
        /// </summary>
        /// <returns>Read KeyboardMap class.</returns>
        public static KeyboardMap DeSerialize()
        {
            var serializer = new XmlSerializer(typeof(KeyboardMap));
            using (var reader = XmlReader.Create("KeyboardMap.xml"))
            {
                var joystick = (KeyboardMap)serializer.Deserialize(reader);
                return joystick;
            }
        }
    }
}
