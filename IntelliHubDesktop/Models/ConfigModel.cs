using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace IntelliHubDesktop.Models
{
    public static class ConfigModel
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IntelliHub",
            "config.json");

        private static JObject _configurations;

        public static void Initialize()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));

            if (File.Exists(ConfigFilePath))
            {
                string json = File.ReadAllText(ConfigFilePath);
                _configurations = JObject.Parse(json);
            }
            else
            {
                _configurations = new JObject();
                SaveToFile();
            }
        }

        public static void Save(string key, string value)
        {
            if (_configurations == null)
            {
                throw new InvalidOperationException("ConfigModel has not been initialized. Call Initialize() first.");
            }

            _configurations[key] = value;
            SaveToFile();
        }

        public static string Read(string key)
        {
            if (_configurations == null)
            {
                throw new InvalidOperationException("ConfigModel has not been initialized. Call Initialize() first.");
            }

            return _configurations[key]?.ToString();
        }

        public static void Delete(string key)
        {
            if (_configurations == null)
            {
                throw new InvalidOperationException("ConfigModel has not been initialized. Call Initialize() first.");
            }

            if (_configurations.Remove(key))
            {
                SaveToFile();
            }
        }

        public static JObject GetAll()
        {
            if (_configurations == null)
            {
                throw new InvalidOperationException("ConfigModel has not been initialized. Call Initialize() first.");
            }

            return new JObject(_configurations);
        }

        private static void SaveToFile()
        {
            if (_configurations == null)
            {
                throw new InvalidOperationException("ConfigModel has not been initialized. Call Initialize() first.");
            }

            string json = _configurations.ToString(Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }
        
    }
}