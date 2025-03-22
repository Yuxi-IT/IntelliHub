using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IntelliHub.Models.Parser
{
    public class FuncModel
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public List<string>? Parameters { get; set; }
    }

    public class FunParser
    {
        /// <summary>
        /// 将自然语言描述的指令解析为函数名与参数
        /// </summary>
        /// <param name="input">对于要操作的指令的描述</param>
        /// <returns></returns>
        public static FuncModel Parse(string input)
        {
            var func = new FuncModel();


            return func;
        }

        public static bool Run(string input,out string output)
        {
            int i = 0;
            output = "NotFound";
            foreach (var line in input.Split("\n"))
            {
                var cmds = input.Split(" ");
                switch (cmds[0].ToLower())
                {
                    case "file":
                        var file = FileParser.Parse(cmds, out string FileOpt); i++;
                        output = FileOpt;
                        i++;
                        return file;

                    case "web":
                        var web = WebParser.Parse(cmds, out string WebOpt);
                        output = WebOpt;
                        i++;
                        return web;

                    case "shell":
                        var shell = ShellExecutor.Execute(cmds, out string ShellOpt);
                        output = ShellOpt;
                        i++;
                        return shell;
                    case "webdriver":
                        output = WebDriverExecutor.Execute(cmds); i++;
                        break;
                }
            }
            Debug.WriteLine($"解析 {i} 条指令");
            return true;
        }
    }
}
