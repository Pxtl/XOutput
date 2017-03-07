using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
            string path = $@"{ dirName }\{FixDevName(devName)}.json";

            var serializer = new XmlSerializer(typeof(DeviceMapping));
            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<InputControl[]>(jsonString, new StringEnumConverter());
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
            string path = $@"{ dirName }\{FixDevName(devName)}.json";
            File.WriteAllText(path, JsonConvert.SerializeObject(mappings, Formatting.Indented, new StringEnumConverter()));
        }
    }

    public class DeviceMapping
    {
        public string DeviceName { get; set; }
        public InputControl[] Controls { get; set; }
    }
}