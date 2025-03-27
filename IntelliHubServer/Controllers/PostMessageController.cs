using IntelliHub.Models;
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

        [HttpGet(Name = "send")]
        public PostMessage Send(string text)
        {
            text = Regex.Replace(text, @"(?<!\\)\\(?![\\\""'nrtbf])", @"\\");
            if (!string.IsNullOrEmpty(text))
            {
                Runtimes.msg.Add(new Message()
                {
                    Role = "user",
                    Content = text
                });

                var cpm = ChatApiRequest.Send(msgs: Runtimes.msg, apikey: Runtimes.ApiKey, Model: "deepseek-v3");
                if (cpm != null)
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
