using IntelliHub.Models;
using IntelliHub.Models.Parser;
using System.Text;
using System.Text.RegularExpressions;

namespace IntelliHubTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConfigModel.Initialize();
            Console.OutputEncoding = Encoding.UTF8;
            while (true)
            {
                var runCmd = Console.ReadLine();
                runCmd = Regex.Replace(runCmd, @"(?<!\\)\\(?![\\\""'nrtbf])", @"\\");
                if (!string.IsNullOrEmpty(runCmd))
                {
                    FunParser.Run(runCmd, out string output);
                    Console.WriteLine($"Result：{output}");
                }
            }
        }
    }
}
