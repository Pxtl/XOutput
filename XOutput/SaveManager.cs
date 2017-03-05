using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace XOutput
{
    static class SaveManager
    {
        static private string dirName = @"configs";

        public static string FixDevName(string devName)
        {
            return new string(devName
               .Where(x => !Path.GetInvalidFileNameChars().Contains(x))
               .ToArray());
        }

        public static InputControl[] Load(string devName)
        {
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
                return null;
            }
            string path = $@"{ dirName }\{FixDevName(devName)}.xml";

            var serializer = new XmlSerializer(typeof(DeviceMapping));
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    var deviceMapping = (DeviceMapping)serializer.Deserialize(stream);
                    return deviceMapping.Controls;
                }
            }
            else
            {
                return new InputControl[21];
            }
        }

        public static void Save(string devName, InputControl[] mappings)
        {
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            string path = $@"{ dirName }\{FixDevName(devName)}.xml";
            var serializer = new XmlSerializer(typeof(DeviceMapping));
            using (var stream = File.Create(path))
            {
                serializer.Serialize(stream, new DeviceMapping { Controls = mappings, DeviceName = devName });
            }
        }
    }

    public class DeviceMapping
    {
        public string DeviceName { get; set; }
        public InputControl[] Controls { get; set; }
    }
}