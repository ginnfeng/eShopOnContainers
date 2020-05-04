////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/2/2010 5:39:19 PM 
// Description: MatrixTrigerEventArgs.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;

namespace Support.Net.Container
{
    public enum MatrixEventType
    {
        Update,
        Insert,
        Remove
    }
    public class MatrixSensorEventArgs : EventArgs
    {
        public MatrixSensorEventArgs()
        {
            EventType = MatrixEventType.Update;
            MediumTransmitters = new List<object>();
        }
        public List<object> MediumTransmitters { get; internal set; }

        public MatrixEventType EventType { get; set; }
    }
}
