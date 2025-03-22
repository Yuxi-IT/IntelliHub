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
        public static void Parse(string[] cmds)
        {
            Debug.WriteLine($"网络操作 {cmds[1].ToLower()}");
            WebClient client = new WebClient();
            switch (cmds[1].ToLower())
            {
                case "download":
                    File.WriteAllBytes((SpaceConvert(cmds[3])), client.DownloadData(SpaceConvert(cmds[2])));
                    break;
                case "get":
                    var content = client.DownloadString(SpaceConvert(cmds[2]));
                    Program.msg.Add(
                        new Message()
                        {
                            Role = "assistant",
                            Content = content
                        });
                    Console.WriteLine(content);
                    break;
                case "post":
                    var pcontent = client.UploadString(SpaceConvert(cmds[2]), SpaceConvert(cmds[3]));
                    Program.msg.Add(
                        new Message()
                        {
                            Role = "assistant",
                            Content = pcontent
                        });
                    Console.WriteLine(pcontent);
                    
                    break;
                default:
                    break;
            }
        }
        public static string SpaceConvert(string ipt)
        {
            return ipt.Replace("%20", " ");
        }
    }
}
