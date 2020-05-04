////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/9/2010 11:39:23 AM 
// Description: BasicSensorContainer.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;

namespace Support.Net.Container
{
    public class BasicSensorContainer : MatrixSensorBase
    {
        public BasicSensorContainer()
        {
        }
        public void Foreach(Action<MatrixSensorBase> action)
        {
            sensors.ForEach(action);
        }
        public void Add(MatrixSensorBase sensor)
        {
            if (sensors.Contains(sensor)) return;
            base.PlugInTriger(sensor);
            sensors.Add(sensor);
        }
        public void Remove(MatrixSensorBase sensor)
        {
            base.UnPlugInTriger(sensor);
            sensors.Remove(sensor);
        }

        public MatrixSensorBase Find(Predicate<MatrixSensorBase> match)
        {
            return sensors.Find(match);
        }

        public int Count { get { return sensors.Count; } }
        private List<MatrixSensorBase> sensors = new List<MatrixSensorBase>();
    }
}
