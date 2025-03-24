using IntelliHubMobile.Models;
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntelliHubMobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // 处理提交按钮点击事件
        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            var userInput = TextInputEntry.Text; // 获取用户输入的文字
            if (string.IsNullOrWhiteSpace(userInput))
            {
                await DisplayAlert("Error", "Please enter some text.", "OK");
                return;
            }

            //try
            //{
            // 发送文字到服务器
            TextInputEntry.Text = "";
            var serverResponse = SendTextToServer(userInput);
                await DisplayAlert("Server Response", serverResponse, "OK");
            //}
            //catch (Exception ex)
            //{
            //    await DisplayAlert("Error", ex.Message, "OK");
            //}
        }

        // 发送文字到服务器的方法
        public string SendTextToServer(string text)
        {
            try
            {
                var url = $"{Runtimes.ServerUrl}{Uri.EscapeDataString(text)}";
                Debug.WriteLine(url);

                using var webClient = new WebClient();
                string response = webClient.DownloadString(url); // 同步请求
                return response;
            }
            catch (WebException ex)
            {
                // 处理网络异常
                Debug.WriteLine("WebException: " + ex.Message);
                if (ex.Response != null)
                {
                    using var stream = ex.Response.GetResponseStream();
                    using var reader = new System.IO.StreamReader(stream);
                    string errorResponse = reader.ReadToEnd();
                    Debug.WriteLine("Error Response: " + errorResponse);
                }
                throw; // 重新抛出异常
            }
            catch (Exception ex)
            {
                // 处理其他异常
                Debug.WriteLine("Exception: " + ex.Message);
                throw; // 重新抛出异常
            }
        }
        //http://novapp.w1.luyouxia.net
    }
}