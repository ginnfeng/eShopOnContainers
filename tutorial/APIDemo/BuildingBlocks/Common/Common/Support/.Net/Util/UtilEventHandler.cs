﻿using System;

namespace Common.Support.Net.Util
{
    public class UtilEventArgs : EventArgs
    {
        public object Content { get; set; }
    }
    public delegate void UtilEventHandler(object sender, UtilEventArgs e);    
}
