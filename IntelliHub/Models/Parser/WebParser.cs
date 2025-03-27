using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntelliHub.Models.Parser
{
    public class WebParser
    {
        public static bool Parse(string[] cmds,out string output)
        {
            try
            {
                Debug.WriteLine($"网络操作 {cmds[1].ToLower()}");

                using (WebClient client = new WebClient())
                {
                    switch (cmds[1].ToLower())
                    {
                        case "download":
                            // 下载文件
                            byte[] data = client.DownloadData(SpaceConvert(cmds[2]));
                            File.WriteAllBytes(SpaceConvert(cmds[3]), data);
                            output = "Success";
                            return true;
                            break;

                        case "get":
                            // 发送 GET 请求
                            string content = client.DownloadString(SpaceConvert(cmds[2]));
                            Runtimes.msg.Add(
                                new Message()
                                {
                                    Role = "assistant",
                                    Content = content
                                });
                            Console.WriteLine(content);
                            output = content;
                            return true;
                            break;

                        case "post":
                            // 发送 POST 请求
                            string postData = SpaceConvert(cmds[3]);
                            string pcontent = client.UploadString(SpaceConvert(cmds[2]), postData);
                            Runtimes.msg.Add(
                                new Message()
                                {
                                    Role = "assistant",
                                    Content = pcontent
                                });
                            Console.WriteLine(pcontent);
                            output = pcontent;
                            break;

                        default:
                            output = $"未知命令: {cmds[1]}";
                            return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                output = $"操作失败: {ex.Message}";
                return false;
            }
        }
        public static string SpaceConvert(string ipt)
        {
            return ipt.Replace("%20", " ")
                      .Replace("%22", "\"")
                      .Replace("%0A", "\n");
        }
    }
}
