using IntelliHubDesktop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;

namespace IntelliHubDesktop.Pages
{
    /// <summary>
    /// AISettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class AISettingPage : Page
    {
        public AISettingPage()
        {
            InitializeComponent();
            Runtimes.settings = AISettingModel.LoadSettings();
            AISettingListView.ItemsSource = Runtimes.settings;
            InitializeSelectedAI();
        }

        private void InitializeSelectedAI()
        {
            if (AISettingListView.Items.Count > 0 && Runtimes.CurrAI == null)
            {
                // 默认选中第一项
                AISettingListView.SelectedIndex = 0;
                Runtimes.CurrAI = AISettingListView.SelectedItem as AISetting;
            }
            else if (Runtimes.CurrAI != null)
            {
                // 查找与 Runtimes.CurrAI 名称和设定匹配的项
                var matchedItem = Runtimes.settings.FirstOrDefault(s =>
                    s.AIName == Runtimes.CurrAI.AIName &&
                    s.Setting == Runtimes.CurrAI.Setting);

                if (matchedItem != null)
                {
                    // 选中匹配的项
                    AISettingListView.SelectedItem = matchedItem;
                }
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 获取当前选中的智能体
                var selectedItem = AISettingListView.SelectedItem as AISetting;

                // 如果选中了某个智能体，则更新其内容
                if (selectedItem != null)
                {
                    selectedItem.AIName = NameBox.Text;
                    selectedItem.Setting = SettingBox.Text;
                }

                // 数据验证：确保没有重复的 AIName
                var duplicateNames = Runtimes.settings
                    .GroupBy(s => s.AIName)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateNames.Any())
                {
                    MessageBox.Show($"以下智能体名称重复：{string.Join(", ", duplicateNames)}", "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 数据验证：确保所有 AIName 不为空
                var emptyNameItems = Runtimes.settings.Where(s => string.IsNullOrWhiteSpace(s.AIName)).ToList();
                if (emptyNameItems.Any())
                {
                    MessageBox.Show("智能体名称不能为空！", "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 保存数据
                AISettingModel.SaveSettings(Runtimes.settings);
                MessageBox.Show("保存成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // 捕获并显示异常信息
                MessageBox.Show($"保存时发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Runtimes.settings.Add(new AISetting { AIName = "新智能体", Setting = "默认设定" });
            var duplicateNames = Runtimes.settings
                .GroupBy(s => s.AIName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNames.Any())
            {
                MessageBox.Show($"以下智能体名称重复：{string.Join(", ", duplicateNames)}", "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 数据验证：确保所有 AIName 不为空
            var emptyNameItems = Runtimes.settings.Where(s => string.IsNullOrWhiteSpace(s.AIName)).ToList();
            if (emptyNameItems.Any())
            {
                MessageBox.Show("智能体名称不能为空！", "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            AISettingModel.SaveSettings(Runtimes.settings);
            Runtimes.settings = AISettingModel.LoadSettings();
            AISettingListView.ItemsSource = Runtimes.settings;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = AISettingListView.SelectedItem as AISetting;
            if (selectedItem != null)
            {
                Runtimes.settings.Remove(selectedItem);
                File.WriteAllText(AISettingModel.ConfigFilePath, JsonConvert.SerializeObject(Runtimes.settings, Formatting.Indented));
            }
            Runtimes.settings = AISettingModel.LoadSettings();
            AISettingListView.ItemsSource = Runtimes.settings;
        }

        private void AISettingListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = AISettingListView.SelectedItem as AISetting;
            if (selectedItem != null)
            {
                Runtimes.CurrAI = selectedItem;
                NameBox.Text = selectedItem.AIName;
                SettingBox.Text = selectedItem.Setting;
            }
        }
    }
}
