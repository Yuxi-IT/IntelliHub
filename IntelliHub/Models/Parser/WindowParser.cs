using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace IntelliHub.Models.Parser
{
    public class WindowInfo
    {
        public string Title { get; set; }
        public int ProcessId { get; set; }
        public IntPtr Handle { get; set; }
        public string ProcessName { get; set; }

        public override string ToString()
        {
            return $"标题: {Title ?? "无标题"}, 进程ID: {ProcessId}, 句柄: {Handle}, 进程名: {ProcessName}";
        }
    }

    public class WindowParser
    {
        // Windows API 声明
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CloseWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // 窗口显示状态常量
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_RESTORE = 9;

        // SetWindowPos 标志
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;

        // 窗口Z序位置
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        public static bool Parse(string[] cmds, out string output)
        {
            output = string.Empty;

            try
            {
                if (cmds.Length < 2)
                {
                    output = "参数不足，至少需要一个操作类型";
                    return false;
                }

                Debug.WriteLine($"窗口操作 {cmds[1].ToLower()}");

                switch (cmds[1].ToLower())
                {
                    case "enum":
                        var windows = GetAllWindows();
                        var sb = new StringBuilder();
                        foreach (var window in windows)
                        {
                            sb.AppendLine($"标题: {window.Title}, PID: {window.ProcessId}, 句柄: {window.Handle}");
                        }
                        output = sb.ToString();
                        return true;

                    case "enumvis":
                        var visibleWindows = GetVisibleWindows();
                        var visibleSb = new StringBuilder();
                        foreach (var window in visibleWindows)
                        {
                            visibleSb.AppendLine($"标题: {window.Title}, PID: {window.ProcessId}, 句柄: {window.Handle}");
                        }
                        output = visibleSb.ToString();
                        return true;

                    case "max":
                        if (cmds.Length < 3 || !IntPtr.TryParse(cmds[2], out IntPtr maxHandle))
                        {
                            output = "需要指定有效的窗口句柄";
                            return false;
                        }
                        return ShowWindow(maxHandle, SW_SHOWMAXIMIZED);

                    case "min":
                        if (cmds.Length < 3 || !IntPtr.TryParse(cmds[2], out IntPtr minHandle))
                        {
                            output = "需要指定有效的窗口句柄";
                            return false;
                        }
                        return ShowWindow(minHandle, SW_SHOWMINIMIZED);

                    case "close":
                        if (cmds.Length < 3 || !IntPtr.TryParse(cmds[2], out IntPtr closeHandle))
                        {
                            output = "需要指定有效的窗口句柄";
                            return false;
                        }
                        return CloseWindow(closeHandle) || DestroyWindow(closeHandle);

                    case "topmost": // 持续置顶
                        if (cmds.Length < 3 || !IntPtr.TryParse(cmds[2], out IntPtr topmostHandle))
                        {
                            output = "需要指定有效的窗口句柄";
                            return false;
                        }
                        return SetWindowPos(topmostHandle, HWND_TOPMOST, 0, 0, 0, 0,
                            SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);

                    case "top": // 单次置顶
                        if (cmds.Length < 3 || !IntPtr.TryParse(cmds[2], out IntPtr topHandle))
                        {
                            output = "需要指定有效的窗口句柄";
                            return false;
                        }
                        return SetWindowPos(topHandle, HWND_TOP, 0, 0, 0, 0,
                            SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);

                    case "bottom": // 置底
                        if (cmds.Length < 3 || !IntPtr.TryParse(cmds[2], out IntPtr bottomHandle))
                        {
                            output = "需要指定有效的窗口句柄";
                            return false;
                        }
                        return SetWindowPos(bottomHandle, HWND_BOTTOM, 0, 0, 0, 0,
                            SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);

                    default:
                        output = $"未知命令: {cmds[1]}";
                        return false;
                }
            }
            catch (Exception ex)
            {
                output = $"操作失败: {ex.Message}";
                return false;
            }
        }

        private static List<WindowInfo> GetAllWindows()
        {
            var windows = new List<WindowInfo>();
            EnumWindows((hWnd, lParam) =>
            {
                // 获取窗口标题
                int length = GetWindowTextLength(hWnd);
                if (length > 0)
                {
                    var sb = new StringBuilder(length + 1);
                    GetWindowText(hWnd, sb, sb.Capacity);
                    string title = sb.ToString();

                    // 获取进程ID
                    GetWindowThreadProcessId(hWnd, out uint processId);

                    // 获取进程名
                    string processName = string.Empty;
                    try
                    {
                        var process = Process.GetProcessById((int)processId);
                        processName = process.ProcessName;
                    }
                    catch { }

                    windows.Add(new WindowInfo
                    {
                        Handle = hWnd,
                        Title = title,
                        ProcessId = (int)processId,
                        ProcessName = processName
                    });
                }
                return true;
            }, IntPtr.Zero);
            return windows;
        }

        private static List<WindowInfo> GetVisibleWindows()
        {
            var allWindows = GetAllWindows();
            var visibleWindows = new List<WindowInfo>();

            foreach (var window in allWindows)
            {
                if (IsWindowVisible(window.Handle))
                {
                    visibleWindows.Add(window);
                }
            }

            return visibleWindows;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
    }
}