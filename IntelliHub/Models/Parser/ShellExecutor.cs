using System;
using System.Diagnostics;
using System.Text;

namespace IntelliHub.Models.Parser
{
    public class ShellExecutor
    {
        public static bool Execute(string[] cmds, out string output)
        {
            output = string.Empty;

            if (cmds.Length < 3)
            {
                output = "Invalid command format.";
                return false;
            }

            string commandType = cmds[1].ToLower();
            string command = SpaceConvert(string.Join(" ", cmds, 2, cmds.Length - 2));

            Debug.WriteLine(commandType);

            try
            {

                switch (commandType)
                {
                    case "ps":
                        output = ExecutePowerShell(command, false);
                        return true;

                    case "pshidden":
                        output = ExecutePowerShell(command, true);
                        return true;

                    case "cmd":
                        output = ExecuteCmd(command, false);
                        return true;

                    case "cmdhidden":
                        output = ExecuteCmd(command, true);
                        return true;

                    default:
                        output = "Unknown command type.";
                        return false;
                }
            }
            catch (Exception ex)
            {
                output = $"Command execution failed: {ex.Message}";
                return false;
            }
        }

        public static string SpaceConvert(string ipt)
        {
            return ipt.Replace("%20", " ")
                      .Replace("%22", "\"")
                      .Replace("%0A", "\n");
        }

        private static string ExecutePowerShell(string command, bool hidden)
        {
            return ExecuteCommand("powershell.exe", command, hidden);
        }

        private static string ExecuteCmd(string command, bool hidden)
        {
            return ExecuteCommand("cmd.exe", "/C " + command, hidden);
        }

        private static string ExecuteCommand(string fileName, string arguments, bool hidden)
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
                    CreateNoWindow = hidden,
                    StandardOutputEncoding = Encoding.UTF8, 
                    StandardErrorEncoding = Encoding.UTF8
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        return $"Error: {error}";
                    }
                    return output;
                }
            }
            catch (Exception ex)
            {
                return $"Command execution failed: {ex.Message}";
            }
        }
    }
}