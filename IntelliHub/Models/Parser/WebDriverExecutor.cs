using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;

namespace IntelliHub.Models.Parser
{
    public class WebDriverExecutor
    {
        public static ChromeOptions options = new ChromeOptions();
        public static IWebDriver driver = new ChromeDriver(options);
        // 初始化 WebDriver

        public static string Execute(string[] cmds)
        {
            driver.Navigate().GoToUrl("https://www.YuxiIT.com.cn");
            if (cmds.Length < 2)
            {
                Debug.WriteLine("Invalid command format. Usage: <commandType> <args>");
                return "Invalid command format. Usage: <commandType> <args>";
            }

            string commandType = cmds[1].ToLower();

            try
            {
                switch (commandType)
                {
                    case "bing":
                        if (cmds.Length < 3)
                        {
                            Debug.WriteLine("Invalid command format for Bing. Usage: Bing <url>");
                            return "Invalid command format for Bing. Usage: Bing <url>";
                        }
                        var bingResults = "";
                        int i = 1;
                        foreach (var item in Bing(SpaceConvert(cmds[2])))
                        {
                            var iS = $"{i++}.{item.Title} | {item.Link}";
                            Console.WriteLine(iS);
                            bingResults += $"{iS}\n";
                        }
                        Program.msg.Add(new Message
                        {
                            Content = bingResults,
                            Role = "assistant"
                        });
                        break;

                    case "openurl":
                        if (cmds.Length < 3)
                        {
                            Debug.WriteLine("Invalid command format for OpenUrl. Usage: OpenUrl <url>");
                            return "Invalid command format for OpenUrl. Usage: OpenUrl <url>";
                        }
                        OpenUrl(SpaceConvert(cmds[2]));
                        break;

                    case "executejs":
                        if (cmds.Length < 3)
                        {
                            Debug.WriteLine("Invalid command format for ExecuteJavaScript. Usage: ExecuteJavaScript <script>");
                            return "Invalid command format for ExecuteJavaScript. Usage: ExecuteJavaScript <script>";
                        }
                        string script = SpaceConvert(string.Join(" ", cmds, 2, cmds.Length - 2));
                        Debug.WriteLine($"执行JS：{script}");
                        ExecuteJavaScript(script);
                        break;

                    default:
                        Debug.WriteLine($"Unknown command type: {commandType}");
                        return $"Unknown command type: {commandType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing command '{commandType}': {ex.Message}");
                return $"Error executing command '{commandType}': {ex.Message}";
            }
            return "Suceess";
        }

        public static List<(string Title, string Link)> Bing(string key)
        {
            OpenUrl($"https://www.bing.com/search?q={key}");
            List<(string Title, string Link)> results = new List<(string Title, string Link)>();

            try
            {
                var resultDiv = driver.FindElement(By.Id("b_results"));

                var liElements = resultDiv.FindElements(By.TagName("li"));

                foreach (var li in liElements)
                {
                    try
                    {
                        var h2Element = li.FindElement(By.TagName("h2"));
                        var aElement = h2Element.FindElement(By.TagName("a"));

                        string title = aElement.Text;
                        string link = aElement.GetAttribute("href");

                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(link))
                        {
                            results.Add((title, link));
                        }
                    }
                    catch (NoSuchElementException)
                    {
                        continue;
                    }
                }
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"Error finding search results: {ex.Message}");
            }

            return results;
        }

        public static void OpenUrl(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        public static void ExecuteJavaScript(string script)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript(script);
        }

        public static string SpaceConvert(string ipt)
        {
            return ipt.Replace("%20", " ")
                      .Replace("%22", "\"")
                      .Replace("%0A", "\n");
        }
    }
}