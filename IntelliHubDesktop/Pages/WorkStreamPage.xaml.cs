using IntelliHubDesktop.Models;
using IntelliHubDesktop.Pages.MWindow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;

namespace IntelliHubDesktop.Pages
{
    public partial class WorkStreamPage : Page
    {
        public WorkStreamPage()
        {
            InitializeComponent();
            Runtimes.wStreams = WorkStreamModel.LoadSettings() ?? new List<WorkStream>();
            WStreamListView.ItemsSource = Runtimes.wStreams;
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = WStreamListView.SelectedItem as WorkStream;

                if (selectedItem != null)
                {
                    selectedItem.WorkName = NameBox.Text;
                    todoList.ItemsSource = selectedItem.Todos;
                }

                var duplicateNames = Runtimes.wStreams
                    .GroupBy(w => w.WorkName)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                WorkStreamModel.SaveSettings(Runtimes.wStreams);
                MessageBox.Show("保存成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存时发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var newWorkStream = new WorkStream { WorkName = "新工作流", Desp = "默认描述" };
            Runtimes.wStreams.Add(newWorkStream);
            var duplicateNames = Runtimes.wStreams
                .GroupBy(w => w.WorkName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            WorkStreamModel.SaveSettings(Runtimes.wStreams);
            Runtimes.wStreams = WorkStreamModel.LoadSettings() ?? new List<WorkStream>();
            WStreamListView.ItemsSource = Runtimes.wStreams;

            WStreamListView.SelectedItem = newWorkStream;
            todoList.ItemsSource = newWorkStream.Todos;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = WStreamListView.SelectedItem as WorkStream;
            if (selectedItem != null)
            {
                Runtimes.wStreams.Remove(selectedItem);
                WorkStreamModel.SaveSettings(Runtimes.wStreams);
            }

            Runtimes.wStreams = WorkStreamModel.LoadSettings() ?? new List<WorkStream>();
            WStreamListView.ItemsSource = Runtimes.wStreams;

            if (WStreamListView.SelectedItem != null)
            {
                var newSelectedItem = WStreamListView.SelectedItem as WorkStream;
                todoList.ItemsSource = newSelectedItem?.Todos;
            }
            else
            {
                todoList.ItemsSource = null;
            }
        }

        private void WStreamListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = WStreamListView.SelectedItem as WorkStream;
            if (selectedItem != null)
            {
                NameBox.Text = selectedItem.WorkName;
                todoList.ItemsSource = selectedItem.Todos;
            }
        }
        private void AddTodoButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedWorkStream = WStreamListView.SelectedItem as WorkStream;
            if (selectedWorkStream != null)
            {
                selectedWorkStream.Todos.Add(new TodoItem { Text = "新流程" });
                todoList.ItemsSource = selectedWorkStream.Todos;
            }
            else
            {
                MessageBox.Show("请先选择一个工作流！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RunWork_Click(object sender, RoutedEventArgs e)
        {
            var selectedWorkStream = WStreamListView.SelectedItem as WorkStream;
            if (selectedWorkStream != null)
            {
                WorkStream newWorkStream = new();
                newWorkStream.Todos = selectedWorkStream.Todos;
                newWorkStream.ID = selectedWorkStream.ID;
                newWorkStream.WorkName = selectedWorkStream.WorkName;
                newWorkStream.Desp = selectedWorkStream.Desp;

                RunWork runWork = new(newWorkStream);
                runWork.Show();
            }
        }
    }
}