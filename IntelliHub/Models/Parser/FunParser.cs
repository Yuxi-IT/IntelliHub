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

        public static bool Run(string input)
        {
            int i = 0;
            foreach (var line in input.Split("\n"))
            {
                var cmds = input.Split(" ");
                switch (cmds[0].ToLower())
                {
                    case "file":
                        FileParser.Parse(cmds); i++;
                        break;
                    case "web":
                        WebParser.Parse(cmds); i++;
                        break;
                    case "shell":
                        ShellExecutor.Execute(cmds); i++;
                        break;
                    case "webdriver":
                        WebDriverExecutor.Execute(cmds); i++;
                        break;
                }
            }
            Debug.WriteLine($"解析 {i} 条指令");
            return true;
        }
    }
}
