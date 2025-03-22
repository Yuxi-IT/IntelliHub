using System;
using System.Diagnostics;
using System.Text;

namespace IntelliHub.Models.Parser
{
    public class ShellExecutor
    {
        public static void Execute(string[] cmds)
        {
            if (cmds.Length < 3)
            {
                Debug.WriteLine("Invalid command format.");
                return;
            }

            string commandType = cmds[1].ToLower();
            string command = SpaceConvert(string.Join(" ", cmds, 2, cmds.Length - 2));

            Debug.WriteLine(commandType);

            switch (commandType)
            {
                case "ps":
                    ExecutePowerShell(command, false);
                    break;
                case "pshidden":
                    ExecutePowerShell(command, true);
                    break;
                case "cmd":
                    ExecuteCmd(command, false);
                    break;
                case "cmdhidden":
                    ExecuteCmd(command, true);
                    break;

                default:
                    Debug.WriteLine("Unknown command type.");
                    break;
            }
        }

        private static string SpaceConvert(string input)
        {
            return input.Replace("%20", " ");
        }

        private static void ExecutePowerShell(string command, bool hidden)
        {
            ExecuteCommand("powershell.exe", command, hidden);
        }

        private static void ExecuteCmd(string command, bool hidden)
        {
            ExecuteCommand("cmd.exe", "/C " + command, hidden);
        }

        private static void ExecuteCommand(string fileName, string arguments, bool hidden)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = hidden
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                        Console.WriteLine(output);
                    if (!string.IsNullOrEmpty(error))
                        Console.WriteLine("Error: " + error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Command execution failed: " + ex.Message);
            }
        }
    }
}
