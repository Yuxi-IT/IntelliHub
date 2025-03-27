using IntelliHub.Models;
using IntelliHub.Models.Parser;
using Newtonsoft.Json;
using RestSharp;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    public static void Main(string[] args)
    {
        ConfigModel.Initialize();
        Console.OutputEncoding = Encoding.UTF8;
        while (true)
        {
            var ipt = Console.ReadLine();
            if (!Regex.IsMatch(ipt, @"(?<!\\)\\(?![\\\""'nrtbf])"))
                ipt = Regex.Replace(ipt, @"(?<!\\)\\(?![\\\""'nrtbf])", @"\\");
            if (!string.IsNullOrEmpty(ipt))
            {
                Runtimes.msg.Add(new Message()
                {
                    Role = "user",
                    Content = ipt
                });

                var cpm = ChatApiRequest.Send(msgs: Runtimes.msg,apikey: Runtimes.ApiKey, Model: "deepseek-v3");
                if(cpm != null)
                {
                    foreach (var choice in cpm.Choices)
                    {
                        Runtimes.msg.Add(new Message()
                        {
                            Role = "assistant",
                            Content = choice.Message.Content
                        });
                        Console.WriteLine(choice.Message.Content);
                        FunParser.Run(choice.Message.Content, out string output);
                        Runtimes.msg.Add(new Message()
                        {
                            Role = "assistant",
                            Content = output
                        });
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(output);
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.Error.WriteLine("请求失败");
                }
                
            }
        }

    }
}