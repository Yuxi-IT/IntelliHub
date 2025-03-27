param(
    [string]$action,
    [int]$x,
    [int]$y
)

Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;

public class InputSimulator
{
    // 键盘模拟
    [DllImport("user32.dll", SetLastError=true)]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    
    // 鼠标模拟
    [DllImport("user32.dll", SetLastError=true)]
    public static extern bool SetCursorPos(int X, int Y);
    
    [DllImport("user32.dll", SetLastError=true)]
    public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
    
    // 鼠标事件常量
    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    public const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    
    // 虚拟键码
    public const byte VK_RETURN = 0x0D;
    public const byte VK_TAB = 0x09;
    public const byte VK_ESC = 0x1B;
    public const byte VK_SPACE = 0x20;
    public const byte VK_CONTROL = 0x11;
    public const byte VK_MENU = 0x12;  // ALT键
    public const byte VK_SHIFT = 0x10;
    public const byte VK_F1 = 0x70;
    public const byte VK_F12 = 0x7B;
}
"@

function Send-Key {
    param(
        [byte]$keyCode,
        [bool]$keyUp = $false
    )
    $flags = 0x0001  # KEYEVENTF_EXTENDEDKEY
    if($keyUp) { $flags = $flags -bor 0x0002 }  # KEYEVENTF_KEYUP
    
    [InputSimulator]::keybd_event($keyCode, 0, $flags, [UIntPtr]::Zero)
}

function Send-Click {
    param(
        [int]$posX,
        [int]$posY,
        [bool]$rightButton = $false
    )
    if($posX -ne 0 -and $posY -ne 0) {
        [InputSimulator]::SetCursorPos($posX, $posY)
    }
    
    if($rightButton) {
        [InputSimulator]::mouse_event([InputSimulator]::MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 50
        [InputSimulator]::mouse_event([InputSimulator]::MOUSEEVENTF_RIGHTUP, 0, 0, 0, [UIntPtr]::Zero)
    } else {
        [InputSimulator]::mouse_event([InputSimulator]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 50
        [InputSimulator]::mouse_event([InputSimulator]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    }
}

# 主程序
switch ($action.ToLower()) {
    # 单个字母/数字键
    { $_ -match "^[a-z0-9]$" } {
        $keyCode = [byte][char]$_
        Send-Key $keyCode $false
        Start-Sleep -Milliseconds 50
        Send-Key $keyCode $true
        break
    }
    
    # 特殊按键
    "enter"     { Send-Key [InputSimulator]::VK_RETURN $false; Start-Sleep -Milliseconds 50; Send-Key [InputSimulator]::VK_RETURN $true }
    "tab"       { Send-Key [InputSimulator]::VK_TAB $false; Start-Sleep -Milliseconds 50; Send-Key [InputSimulator]::VK_TAB $true }
    "esc"       { Send-Key [InputSimulator]::VK_ESC $false; Start-Sleep -Milliseconds 50; Send-Key [InputSimulator]::VK_ESC $true }
    "space"     { Send-Key [InputSimulator]::VK_SPACE $false; Start-Sleep -Milliseconds 50; Send-Key [InputSimulator]::VK_SPACE $true }
    "ctrl"      { Send-Key [InputSimulator]::VK_CONTROL $false; Start-Sleep -Milliseconds 50; Send-Key [InputSimulator]::VK_CONTROL $true }
    "alt"       { Send-Key [InputSimulator]::VK_MENU $false; Start-Sleep -Milliseconds 50; Send-Key [InputSimulator]::VK_MENU $true }
    "shift"     { Send-Key [InputSimulator]::VK_SHIFT $false; Start-Sleep -Milliseconds 50; Send-Key [InputSimulator]::VK_SHIFT $true }
    
    # 功能键
    "f1"        { Send-Key [InputSimulator]::VK_F1 $false; Start-Sleep -Milliseconds 50; Send-Key [InputSimulator]::VK_F1 $true }
    "f12"       { Send-Key [InputSimulator]::VK_F12 $false; Start-Sleep -Milliseconds 50; Send-Key [InputSimulator]::VK_F12 $true }
    
    # 鼠标操作
    "click"     { Send-Click $x $y $false }
    "rightclick" { Send-Click $x $y $true }
    "move"      { 
        if($x -eq 0 -or $y -eq 0) {
            Write-Host "需要指定坐标参数，例如: .\key.ps1 move 500 500"
        } else {
            [InputSimulator]::SetCursorPos($x, $y) 
        }
    }
    
    default {
        Write-Host "未知操作，支持的按键:"
        Write-Host "  字母/数字: a, b, c... 1, 2, 3..."
        Write-Host "  特殊按键: enter, tab, esc, space, ctrl, alt, shift"
        Write-Host "  功能键: f1-f12"
        Write-Host "  鼠标操作: click, rightclick, move x y"
    }
}