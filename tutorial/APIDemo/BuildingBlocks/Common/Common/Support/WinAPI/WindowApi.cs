////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/21/2008 10:31:59 AM 
// Description: WindowAPI.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Common.Support.WinApi
{
    static public class CmdShow
    {
        public const int Hide   =   0;   
        public const int Normal   =   1   ;
        public const int Showminimized = 2;        
        public const int Maximize = 3;   
        public const int NoActivate =   4;
        public const int Show = 5;
        public const int Minimize=   6   ;
        public const int MinNoActive=   7;
        public const int ShowNA = 8;
        public const int Restore = 9;
        public const int ShowDefault = 10;
        public const int ForceMinimize = 11;
    }
    struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    } 
    static public class WinFunction
    {                
        /// <param name="hwnd"></param>
        /// <param name="nCmdShow">1:SW_NORMAL  2:SW_MINIMIZE  3:SW_MAXIMIZE</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        private static extern int DoShowWindow(IntPtr handleWinCmd, int cmdShow);
        public  static int ShowWindow(IntPtr handleWinCmd, int cmdShow)
        {
            return DoShowWindow(handleWinCmd,cmdShow);
        }
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", CharSet = CharSet.Auto)]
        private static extern bool DoSetForegroundWindow(IntPtr handleWinCmd);
        public static bool SetForegroundWindow(IntPtr handleWinCmd)
         {
             return DoSetForegroundWindow(handleWinCmd);
         }
        public static TimeSpan GetLastInput()
        {
            var plii = new LASTINPUTINFO();
            plii.cbSize = (uint)Marshal.SizeOf(plii);

            if (GetLastInputInfo(ref plii))
                return TimeSpan.FromMilliseconds(Environment.TickCount - plii.dwTime);
            else
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
       

    }
}
