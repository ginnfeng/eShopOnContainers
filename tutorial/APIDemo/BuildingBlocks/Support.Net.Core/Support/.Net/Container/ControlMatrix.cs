////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/29/2010 5:41:52 PM 
// Description: Map.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Linq;
using Support.Net.Util;
using System.Collections.ObjectModel;


namespace Support.Net.Container
{
    public partial class ControlMatrix<TRowUnit, TColumnUnit, TElement> : MatrixSensorBase
    {
        public ControlMatrix(IList<TRowUnit> rowUnitList, IList<TColumnUnit> columnUnitList, TElement[,] elements)
        {
            if (elements != null && (rowUnitList.Count != elements.GetLength(0) || columnUnitList.Count != elements.GetLength(1)))
                throw new IndexOutOfRangeException("Support.Net.Util.ControlMatrix()");
            rows = Clone(rowUnitList, (idx, unit) => new MatrixRow(unit, idx));
            columns = Clone(columnUnitList, (idx, unit) => new MatrixColumn(unit, idx));
            matrixElements = new MatrixElement[rows.Count(), columns.Count()];
            if (elements != null)
            {
                for (int i = 0; i < rows.Count(); i++)
                    for (int j = 0; j < columns.Count(); j++)
                    {
                        var element = DoGetPoint(i, j);
                        element.Element = elements[i, j];
                    }
            }
        }
        public ControlMatrix(IList<TRowUnit> rowUnitList, IList<TColumnUnit> columnUnitList)
            : this(rowUnitList, columnUnitList, null)
        {
        }
        public MatrixElement GetElementByIndex(int rowIdx, int columnIdx)
        {
            return DoGetPoint(rowIdx, columnIdx);
        }
        public MatrixElement GetElement(TRowUnit rowIdx, TColumnUnit columnIdx)
        {
            int posRow = GetPosition(ref rows, row => rowIdx.Equals(row.Unit));
            var posColumn = GetPosition(ref columns, column => columnIdx.Equals(column.Unit));
            return GetElementByIndex(posRow, posColumn);
        }

        public MatrixElement[] GetRowVector(TRowUnit rowIdx)
        {
            int index = GetPosition(ref rows, row => rowIdx.Equals(row.Unit));
            return GetRowVectorByIndex(index);
        }
        public MatrixElement[] GetRowVectorByIndex(int index)
        {
            return DoGetElements(index, columns.Length, (pos, idx) => matrixElements[pos, idx]);
        }

        public MatrixElement[] GetColumnVector(TColumnUnit columnIdx)
        {
            int index = GetPosition(ref columns, column => columnIdx.Equals(column.Unit));
            return GetColumnVectorByIndex(index);
        }
        public MatrixElement[] GetColumnVectorByIndex(int index)
        {
            return DoGetElements(index, rows.Length, (pos, idx) => matrixElements[idx, pos]);
        }
        public int RowLength { get { return rows.Length; } }
        public int ColumnLength { get { return columns.Length; } }

        //private void OnProcessTriger(object owner, MatrixSensorEventArgs eventArgs)
        //{
        //    if (TrigerEvent != null)
        //    {
        //        TrigerEvent(owner, eventArgs);
        //    }
        //}
        //public event Action<object, MatrixSensorEventArgs> TrigerEvent;

        private MatrixElement[] DoGetElements(int index, int length, Func<int, int, MatrixElement> getElement)
        {
            if (index < 0) return null;
            MatrixElement[] xPoints = new MatrixElement[length];
            for (int i = 0; i < length; i++)
            {
                xPoints[i] = getElement(index, i);
            }
            return xPoints;
        }

        private int GetPosition<TObject>(ref TObject[] array, Func<TObject, bool> isEquals)
        {
            int pos = 0;
            try
            {
                array.First(obj => { if (!isEquals(obj)) { pos++; return false; };return true; });
                return pos;
            }
            catch (System.InvalidOperationException)
            {
                return -1;
            }
        }
        private MatrixElement DoGetPoint(int rowIdx, int columnIdx)
        {
            if (rowIdx < 0 || rowIdx >= rows.Length || columnIdx < 0 || columnIdx >= columns.Length) return null;
            var element = matrixElements[rowIdx, columnIdx];
            if (element == null)
            {
                element = new MatrixElement(rows[rowIdx], columns[columnIdx]);
                element.ProcessTrigerEvent += this.OnProcessTriger;
                matrixElements[rowIdx, columnIdx] = element;
            }
            return element;
        }
        private TMatrixUnit[] Clone<TUnit, TMatrixUnit>(IList<TUnit> units, Func<int, TUnit, TMatrixUnit> convert)
        {
            TMatrixUnit[] matrixUnits = new TMatrixUnit[units.Count];
            int pos = 0;
            units.ForEach(unit => { matrixUnits[pos] = convert(pos, unit); pos++; });
            return matrixUnits;
        }
        public ReadOnlyCollection<MatrixRow> Rows
        { 
            get{return new List<MatrixRow>(rows).AsReadOnly();}
        }
        public ReadOnlyCollection<MatrixColumn> Columns
        {
            get { return new List<MatrixColumn>(columns).AsReadOnly(); }
        }
        private MatrixRow[] rows;
        private MatrixColumn[] columns;
        private MatrixElement[,] matrixElements;
    }
}
