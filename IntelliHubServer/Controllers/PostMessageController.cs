﻿using IntelliHub.Models;
using IntelliHub.Models.Parser;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace IntelliHubServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostMessageController : ControllerBase
    {
        private readonly ILogger<PostMessageController> _logger;

        public PostMessageController(ILogger<PostMessageController> logger)
        {
            _logger = logger;
        }
        public static List<Message> msg = new()
        {
            new Message()
            {
                Role = "system",
                Content = @"
        新建c#项目直接使用dotnet new [console]即可
        环境 - 已安装
        -Python -Dotnet6.0/8.0/9.0 -PHP -JDK17 -NodeJS -ADB -Git -CMAKE
        File - 文件操作类
        -Write - path content(content有空格必须用%20代替)
        -Replace - path oldString newString //替换文件中所有指定的字符串
        -Move - path newpath
        -Copy - path newpath
        -Read - path
        -Del - path
        Web - 网络访问类
        -Download url savepath
        -Post url content
        -Get url
        Shell - 命令执行类
        -PS command  //用PowerShell
        -Cmd command //用cmd
        -CmdHidden command
        -PSHidden command
        WebDriver - 浏览器操作类
        -ExecuteJS jscode(代码的中的空格一定要转为%20)
        -OpenUrl url
        -Bing url //必应搜索
        API接口（需要用Web Get）：
        http://192.168.0.104/control?var=led_intensity&val=[灯光值]
        如果删除文件d:\\1.txt，只需输出File Del d:\\1.txt
        除了指令不要有任何多余的内容。不明白意图则回复你不理解
"
            }
        };

        [HttpGet(Name = "send")]
        public PostMessage Send(string text)
        {
            text = Regex.Replace(text, @"(?<!\\)\\(?![\\\""'nrtbf])", @"\\");
            if (!string.IsNullOrEmpty(text))
            {
                msg.Add(new Message()
                {
                    Role = "user",
                    Content = text
                });

                var cpm = ChatApiRequest.Send(msgs: msg, apikey: Runtimes.ApiKey, Model: "deepseek-v3");
                if (cpm != null)
                {
                
                    foreach (var choice in cpm.Choices)
                    {
                        msg.Add(new Message()
                        {
                            Role = "assistant",
                            Content = choice.Message.Content
                        });
                        Console.WriteLine(choice.Message.Content);
                        FunParser.Run(choice.Message.Content, out string output);
                        
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(output);
                        Console.ResetColor();
                        return new PostMessage()
                        {
                            code = 200,
                            result = output
                        };
                    }
                }
                else
                {
                    Console.Error.WriteLine("请求失败");
                    return new PostMessage()
                    {
                        code = 500,
                        result = "请求失败"
                    };
                }

            }
            else
            {
                return new PostMessage()
                {
                    code = 400,
                    result = "输入为空"
                };
            }
            return new PostMessage()
            {
                code = 500,
                result = "位置错误"
            };
        }
    }
}
