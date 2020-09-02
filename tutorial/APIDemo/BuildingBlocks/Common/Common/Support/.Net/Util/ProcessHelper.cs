////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/21/2008 10:45:16 AM 
// Description: ProcessHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Common.Support.Net.Util
{
    public class ProcessHelper
    {
        public ProcessHelper()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdShow">1:SW_NORMAL  2:SW_MINIMIZE  3:SW_MAXIMIZE</param>
        public void ShowWindow(int cmdShow)
        {
            foreach (var proc in attachedProcesses)
	        {
                if(proc.MainWindowHandle!=null)
                    Support.WinApi.WinFunction.ShowWindow(proc.MainWindowHandle,cmdShow);
            }
        }
        public void SetForegroundWindow()
        {
            foreach (var proc in attachedProcesses)
	        {
                if(proc.MainWindowHandle!=null)
                    Support.WinApi.WinFunction.SetForegroundWindow(proc.MainWindowHandle);
            }
        }

        public void KillAllProcess()
        {
            foreach (var proc in attachedProcesses)
	        {
                proc.Kill();
	        }
            attachedProcesses.Clear();
        }

        public bool AttachProcessByName(string name)
        {
            return AttachProcess(new List<Process>(Process.GetProcessesByName(name)));                        
        }

        public bool AttachProcess(Func<Process,bool> cond )
        {
            var processes = from proc in Process.GetProcesses()
                            where cond(proc)
                            select proc;            
            return AttachProcess(processes.ToList());            
        }
        public void AttachProcess(Process process)
        {
            attachedProcesses.Add(process);
        }
        public bool AttachProcess(List<Process> processes)
        {
            attachedProcesses.Clear();
            attachedProcesses.AddRange(processes);
            return attachedProcesses.Count != 0;
        }

        List<Process> attachedProcesses=new List<Process>();
    }
}
