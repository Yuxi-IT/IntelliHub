using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IntelliHub.Models.Parser
{
    public class FileParser
    {
        public static void Parse(string[] cmds)
        {
            Debug.WriteLine($"操作文件 {cmds[1].ToLower()}");
            switch (cmds[1].ToLower())
            {
                case "write":
                    Write(SpaceConvert(cmds[2]), SpaceConvert(cmds[3]));
                    break;
                case "move":
                    Move(SpaceConvert(cmds[2]), SpaceConvert(cmds[3]));
                    break;
                case "copy":
                    Copy(SpaceConvert(cmds[2]), SpaceConvert(cmds[3]));
                    break;
                case "del":
                    Delect(SpaceConvert(cmds[2]));
                    break;
                case "replace":
                    Replace(SpaceConvert(cmds[2]), SpaceConvert(cmds[3]), SpaceConvert(cmds[4]));
                    break;
                case "read":
                    var content = Read(SpaceConvert(cmds[2]));
                    Program.msg.Add(
                        new Message() 
                        { 
                            Role = "assistant", 
                            Content = content 
                        });
                    Console.WriteLine(content);
                    break;
                default:
                    break;
            }
        }
        public static string SpaceConvert(string ipt)
        {
            return ipt.Replace("%20"," ")
                      .Replace("%22","\"")
                      .Replace("%0A","\n");
        }

        private static void Replace(string path,string oldContent, string newContent)
            => File.WriteAllText(path, Read(path).Replace(oldContent, newContent));

        private static void Write(string path, string content)
            => File.WriteAllText(path, content);

        private static void Move(string path, string newpath)
            => File.Move(path, newpath);

        private static void Copy(string path, string newpath)
            => File.Copy(path, newpath);

        private static void Delect(string path)
            => File.Delete(path);

        private static string Read(string path)
            => File.ReadAllText(path);
    }
}
