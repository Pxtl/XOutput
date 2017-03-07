using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Linq;

namespace XOutput
{
    public static class SaveManager
    {
        static private string _dirName = @"configs";

        public static InputControl[] LoadDeviceControls(string devName)
        {
            return Load<InputControl[]>(devName) ?? new InputControl[21];
        }

        public static void SaveDeviceControls(string devName, InputControl[] mappings)
        {
            Save(devName, mappings);
        }

        public static void SaveConfig(Config config)
        {
            Save("Config", config);
        }

        public static Config LoadConfig()
        {
            return Load<Config>("Config") ?? new Config();
        }

        #region helper methods
        private static T Load<T>(string name)
        {
            if (!Directory.Exists(_dirName))
            {
                Directory.CreateDirectory(_dirName);
                return default(T);
            }
            string path = $@"{ _dirName }\{CleanFileName(name)}.json";

            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(jsonString, new StringEnumConverter());
            }
            else
            {
                return default(T);
            }
        }

        private static void Save<T>(string name, T obj)
        {
            if (!Directory.Exists(_dirName))
            {
                Directory.CreateDirectory(_dirName);
            }
            string path = $@"{ _dirName }\{CleanFileName(name)}.json";
            File.WriteAllText(path, JsonConvert.SerializeObject(obj, Formatting.Indented, new StringEnumConverter()));
        }

        public static string CleanFileName(string devName)
        {
            return new string(devName
               .Where(x => !Path.GetInvalidFileNameChars().Contains(x))
               .ToArray());
        }
        #endregion
    }
}