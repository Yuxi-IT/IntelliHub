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

        public static bool RunAll(string input,out string output)
        {
            int i = 0;
            output = "NotFound";
            var cmds = input.Split("\n");
            foreach (var line in cmds)
            {
                var pars = line.Split(" ");
                switch (pars[0].ToLower())
                {
                    case "file":
                        var file = FileParser.Parse(pars, out string FileOpt);
                        output = FileOpt;
                        i++;
                        return file;

                    case "web":
                        var web = WebParser.Parse(pars, out string WebOpt);
                        output = WebOpt;
                        i++;
                        return web;

                    case "shell":
                        var shell = ShellExecutor.Execute(pars, out string ShellOpt);
                        output = ShellOpt;
                        i++;
                        return shell;

                    case "webdriver":
                        output = WebDriverExecutor.Execute(pars);
                        i++;
                        break;

                    case "window":
                        var window = WindowParser.Parse(pars, out string WindowOpt);
                        i++;
                        output = WindowOpt;
                        return window;
                }
            }
            return true;
        }

        public static bool Run(string input, out string output)
        {
            int i = 0;
            output = "NotFound";
            input = input.Split('\n')[0];
            var pars = input.Split(" ");
            switch (pars[0].ToLower())
            {
                case "file":
                    var file = FileParser.Parse(pars, out string FileOpt);
                    output = FileOpt;
                    i++;
                    return file;

                case "web":
                    var web = WebParser.Parse(pars, out string WebOpt);
                    output = WebOpt;
                    i++;
                    return web;

                case "shell":
                    var shell = ShellExecutor.Execute(pars, out string ShellOpt);
                    output = ShellOpt;
                    i++;
                    return shell;

                case "webdriver":
                    output = WebDriverExecutor.Execute(pars);
                    i++;
                    break;

                case "window":
                    var window = WindowParser.Parse(pars, out string WindowOpt);
                    i++;
                    output = WindowOpt;
                    return window;
            }
            return true;
        }
    }
}
