using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace IntelliHubDesktop.Models
{
    public static class ConfigModel
    {
        // 配置文件路径
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IntelliHub",
            "config.json");

        // 配置项存储对象
        private static JObject _configurations;

        // 初始化配置
        public static void Initialize()
        {
            // 确保配置目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));

            // 如果配置文件存在，则加载
            if (File.Exists(ConfigFilePath))
            {
                string json = File.ReadAllText(ConfigFilePath);
                _configurations = JObject.Parse(json);
            }
            else
            {
                // 如果文件不存在，创建一个空的 JObject
                _configurations = new JObject();
                SaveToFile(); // 保存空配置到文件
            }
        }

        // 保存配置项
        public static void Save(string key, string value)
        {
            if (_configurations == null)
            {
                throw new InvalidOperationException("ConfigModel has not been initialized. Call Initialize() first.");
            }

            _configurations[key] = value;
            SaveToFile();
        }

        // 读取配置项
        public static string Read(string key)
        {
            if (_configurations == null)
            {
                throw new InvalidOperationException("ConfigModel has not been initialized. Call Initialize() first.");
            }

            return _configurations[key]?.ToString();
        }

        // 删除配置项
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

        // 获取所有配置项
        public static JObject GetAll()
        {
            if (_configurations == null)
            {
                throw new InvalidOperationException("ConfigModel has not been initialized. Call Initialize() first.");
            }

            return new JObject(_configurations);
        }

        // 将配置保存到文件
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