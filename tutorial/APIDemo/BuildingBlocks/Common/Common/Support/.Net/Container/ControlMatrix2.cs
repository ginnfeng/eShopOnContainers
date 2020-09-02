////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/29/2010 5:41:52 PM 
// Description: Map.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Common.Support.Net.Container
{
    public interface IMatrixSensor
    {
        void Triger(MatrixSensorEventArgs eventArgs);
        void OnProcessTriger(object owner, MatrixSensorEventArgs eventArgs);
        event Action<object, MatrixSensorEventArgs> FireTrigerEvent;
        event Action<object, MatrixSensorEventArgs> ProcessTrigerEvent;
    }
    public abstract class MatrixSensorBase : IMatrixSensor
    {
        public string Id { get; set; }
        public void Triger(MatrixSensorEventArgs eventArgs)
        {
            Triger(this, eventArgs);
        }
        protected void Triger(object owner, MatrixSensorEventArgs eventArgs)
        {
            if (FireTrigerEvent != null)
            {
                if (eventArgs == null) eventArgs = new MatrixSensorEventArgs();
                if (owner != this) eventArgs.MediumTransmitters.Add(this);
                FireTrigerEvent(owner, eventArgs);
            }
        }
        public void OnProcessTriger(object owner, MatrixSensorEventArgs eventArgs)
        {
            if (ProcessTrigerEvent != null)
            {
                if (owner != this) eventArgs.MediumTransmitters.Add(this);
                ProcessTrigerEvent(owner, eventArgs);
            }
        }
        public event Action<object, MatrixSensorEventArgs> FireTrigerEvent;
        public event Action<object, MatrixSensorEventArgs> ProcessTrigerEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extTriger"></param>
        public void PlugInTriger(IMatrixSensor extTriger)
        {
            if (extTriger == null) return;
            extTriger.FireTrigerEvent += this.Triger;
            this.ProcessTrigerEvent -= extTriger.OnProcessTriger;
            this.ProcessTrigerEvent += extTriger.OnProcessTriger;
        }
        public void UnPlugInTriger(IMatrixSensor extTriger)
        {
            if (extTriger == null) return;
            extTriger.FireTrigerEvent -= this.Triger;
            this.ProcessTrigerEvent -= extTriger.OnProcessTriger;
        }
        /// <summary>
        /// Matrix內部元件(ControlMatrix,MatrixRow,MatrixRow)的互動
        /// </summary>
        /// <param name="extTriger"></param>
        public void BindingTriger(IMatrixSensor extTriger)
        {
            if (extTriger == null) return;
            extTriger.FireTrigerEvent += this.OnProcessTriger;
            this.FireTrigerEvent += extTriger.OnProcessTriger;
        }
    }
    public partial class ControlMatrix<TRowUnit, TColumnUnit, TElement>
    {
        public class MatrixRow : MatrixSensorBase
        {
            public MatrixRow(TRowUnit unit, int index)
            {
                Unit = unit;
                Index = index;
                base.PlugInTriger(unit as IMatrixSensor);
            }
            public TRowUnit Unit { get; private set; }
            public int Index { get; private set; }
        }
        public class MatrixColumn : MatrixSensorBase
        {
            public MatrixColumn(TColumnUnit unit, int index)
            {
                Unit = unit;
                Index = index;
                base.PlugInTriger(unit as IMatrixSensor);
            }
            public TColumnUnit Unit { get; private set; }
            public int Index { get; private set; }
        }
        public class MatrixElement : MatrixSensorBase
        {
            public MatrixElement(MatrixRow row, MatrixColumn column)
            {
                Row = row;
                Column = column;
                base.BindingTriger(Row);
                base.BindingTriger(Column);
            }

            public MatrixRow Row { get; private set; }
            public MatrixColumn Column { get; private set; }
            public TElement Element
            {
                get { return element; }
                set
                {
                    if (element.Equals(value)) return;
                    var extTriger = value as IMatrixSensor;
                    base.UnPlugInTriger(extTriger);
                    element = value;
                    base.PlugInTriger(extTriger);
                }
            }
            private TElement element;
        }

    }
}
