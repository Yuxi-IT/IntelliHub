using IntelliHub.Models;
using System;
using System.Diagnostics;

namespace IntelliHub.Models.Parser
{
    public static class FileParser
    {
        public static bool Parse(string[] cmds, out string output)
        {
            output = string.Empty;

            try
            {
                Debug.WriteLine($"操作文件 {cmds[1].ToLower()}");

                switch (cmds[1].ToLower())
                {
                    case "write":
                        output = Write(SpaceConvert(cmds[2]), SpaceConvert(string.Join(" ", cmds, 3, cmds.Length - 3)));
                        return true;

                    case "move":
                        output = Move(SpaceConvert(cmds[2]), SpaceConvert(cmds[3]));
                        return true;

                    case "copy":
                        output = Copy(SpaceConvert(cmds[2]), SpaceConvert(cmds[3]));
                        return true;

                    case "del":
                        output = Delete(SpaceConvert(cmds[2]));
                        return true;

                    case "replace":
                        output = Replace(SpaceConvert(cmds[2]), SpaceConvert(string.Join(" ", cmds, 3, cmds.Length - 3)), SpaceConvert(string.Join(" ", cmds, 4, cmds.Length - 4)));
                        return true;

                    case "read":
                        output = Read(SpaceConvert(cmds[2]));
                        Program.msg.Add(
                            new Message()
                            {
                                Role = "assistant",
                                Content = output
                            });
                        return true;

                    default:
                        output = "未知命令";
                        return false;
                }
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

        public static string Replace(string path, string oldContent, string newContent)
        {
            try
            {
                string fileContent = Read(path);
                fileContent = fileContent.Replace(oldContent, newContent);
                File.WriteAllText(path, fileContent);
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Replace failed: {ex.Message}";
            }
        }

        public static string Write(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content);
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Write failed: {ex.Message}";
            }
        }

        public static string Delect(string path)
        {
            try
            {
                File.Delete(path);
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Delect failed: {ex.Message}";
            }
        }

        public static string Move(string path, string newPath)
        {
            try
            {
                File.Move(path, newPath);
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Move failed: {ex.Message}";
            }
        }

        public static string Copy(string path, string newPath)
        {
            try
            {
                File.Copy(path, newPath);
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Copy failed: {ex.Message}";
            }
        }

        public static string Delete(string path)
        {
            try
            {
                File.Delete(path);
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Delete failed: {ex.Message}";
            }
        }

        private static string Read(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                throw new Exception($"Read failed: {ex.Message}");
            }
        }

    }
}