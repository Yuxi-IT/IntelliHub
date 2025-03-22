using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IntelliHubDesktop.Models
{
    public class TodoItem
    {
        public DateTime Date { get; } = DateTime.Now;
        public string Text { get; set; }
    }
    public class WorkStream
    {
        public string ID { get; } = Guid.NewGuid().ToString();
        public string WorkName { get; set; }
        public string? Desp { get; set; }
        public ObservableCollection<TodoItem> Todos { get; set; } = new();
    }

    internal class WorkStreamModel
    {
        public static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IntelliHub",
            "wstream.json");

        public static List<WorkStream> LoadSettings()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));

            if (File.Exists(ConfigFilePath))
            {
                string json = File.ReadAllText(ConfigFilePath);
                return JsonConvert.DeserializeObject<List<WorkStream>>(json);
            }
            else
            {
                File.WriteAllText(ConfigFilePath, "[]");
                return new List<WorkStream>();
            }
        }

        public static void SaveSettings(List<WorkStream> settings)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }
    }
}