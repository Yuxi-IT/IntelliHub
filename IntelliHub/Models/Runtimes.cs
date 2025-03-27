using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliHub.Models
{
    public class Runtimes
    {
        public static List<Message> msg = new()
        {
            new Message()
            {
                Role = "system",
                Content = @"
环境 - 已安装
-Python -Dotnet6.0/8.0/9.0 -PHP -JDK17 -NodeJS -ADB -Git -CMAKE
File - 文件操作类
-Write - path content
-Replace - path oldString newString
-Move - path newpath
-Copy - path newpath
-Read - path
-Del - path
Web - 网络访问类
-Download url savepath
-Post url content
-Get url
Shell - 命令执行类
-PS command
-Cmd command
-CmdHidden command
-PSHidden command
WebDriver - 浏览器操作
-ExecuteJS jscode
-OpenUrl url
-Bing url //搜索
Window - 窗口操作
-Enum：枚举所有窗口信息（标题/PID/句柄）
-EnumVis：枚举可见窗口信息
-Max：最大化窗口(要句柄)
-Min：最小化窗口
-Close：关闭窗口
-TopMost：持续置顶
-Top:单次置顶
-Bottom:置底
当前程序目录中的Script目录有ps脚本文件，可以用shell ps执行：
.\key.ps1 a # 模拟a键
.\key.ps1 enter # 模拟回车
.\key.ps1 f1 # 模拟F1
.\key.ps1 click # 鼠标左键
.\key.ps1 rightclick  # 鼠标右键
.\key.ps1 move 500 500 # 移动鼠标
如果删除文件d:\\1.txt，只需输出File Del d:\\1.txt
除了指令不要有任何多余的内容。不明白意图则回复:我不理解"
            }
        };
        public static string ApiKey = ConfigModel.Read("key");
    }
}
