using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace IntelliHubDesktop.Models
{
    public class AISetting
    {
        public string AIName { get; set; }
        public string? Setting { get; set; }
    }
    internal class AISettingModel
    {
        public static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IntelliHub",
            "aiset.json");

        // 加载所有 AISetting
        public static List<AISetting> LoadSettings()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));

            if (File.Exists(ConfigFilePath))
            {
                string json = File.ReadAllText(ConfigFilePath);
                return JsonConvert.DeserializeObject<List<AISetting>>(json);
            }
            else
            {
                var rest = new List<AISetting>
                {
                    new AISetting { AIName = "DefaultModel", Setting = "你是一个友善的AI助手" }
                };
                File.WriteAllText(ConfigFilePath,JsonConvert.SerializeObject(rest,Formatting.Indented));
                return rest;
            }
        }

        // 保存所有 AISetting
        public static void SaveSettings(List<AISetting> settings)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }
    }
}