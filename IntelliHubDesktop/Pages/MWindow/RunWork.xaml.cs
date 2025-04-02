using IntelliHub.Models.Parser;
using IntelliHubDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace IntelliHubDesktop.Pages.MWindow
{
    /// <summary>
    /// RunWork.xaml 的交互逻辑
    /// </summary>
    public partial class RunWork : Window
    {
        public List<TodoItem> CompletedTodos { get; set; } = new();
        public List<TodoItem> RemainingTodos { get; set; } = new();
        
        public WorkStream ws = new WorkStream();

        public RunWork(WorkStream runWs)
        {
            InitializeComponent();

            ws.WorkName = runWs.WorkName;
            ws.Desp = runWs.Desp;
            ws.ID = runWs.ID;
            ws.Todos = runWs.Todos;

            StartButton.Click += StartButton_Click;

            Loaded += (s, e) =>
            {
                RemainingTodos = ws.Todos.ToList();
                CompletedTodo.ItemsSource = CompletedTodos;
                RemainingTodo.ItemsSource = RemainingTodos;
            };

            Title = $"工作流：{ws.WorkName} - {ws.ID} - 共{ws.Todos.Count}条流程";
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;

            foreach (var todo in ws.Todos.ToList())
            {
                Task.Run(() =>
                {
                    DateTime startTime = DateTime.Now;
                    bool result = FunParser.Run(todo.Text, out string output);
                    DateTime endTime = DateTime.Now;
                    TimeSpan duration = endTime - startTime;
                    Dispatcher.Invoke(() =>
                    {
                        LogOutput.Items.Add($"流程: {todo.Text}\n" +
                                           $"开始时间: {startTime.ToString("HH:mm:ss")}\n" +
                                           $"结束时间: {endTime.ToString("HH:mm:ss")}\n" +
                                           $"运行时间: {duration.TotalSeconds:F2} 秒\n" +
                                           $"运行结果: {(result ? "成功" : "失败")}\n" +
                                           $"输出: {output}");
                    });

                    Dispatcher.Invoke(() => UpdateTodoLists(todo, result));
                });
            }
            StartButton.IsEnabled = true;
        }

        private void UpdateTodoLists(TodoItem todo,bool result)
        {
            if (result && !CompletedTodos.Contains(todo) && RemainingTodos.Contains(todo))
            {
                CompletedTodos.Add(todo);
                RemainingTodos.Remove(todo);
            }
        }

        public static string ConvertEncoding(string text, Encoding sourceEncoding, Encoding targetEncoding)
        {
            byte[] sourceBytes = sourceEncoding.GetBytes(text);
            string result = targetEncoding.GetString(sourceBytes);

            return result;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            LogOutput.Items.Clear();

            CompletedTodos = new List<TodoItem>();
            RemainingTodos = new List<TodoItem>(ws.Todos);

            CompletedTodo.ItemsSource = CompletedTodos;
            RemainingTodo.ItemsSource = RemainingTodos;
        }
    }
}
