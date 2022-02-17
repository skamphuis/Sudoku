using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuLib
{
    public class MiniSeries
    {
        public MiniSeries(List<Square> squares)
        {
            foreach (Square sq in squares)
            {
                //add event handlers
                sq.OnSquareExcludedChanged += new Square.SquareExcludedChangedHandler(sq_OnSquareExcludedChanged);
                sq.OnSquareSolved += new Square.SquareSolvedHandler(sq_OnSquareSolved);
            }
        }

        void sq_OnSquareSolved(Square sender, SquareSolvedEventArgs e)
        {
            //
        }

        void sq_OnSquareExcludedChanged(Square sender, ExcludedChangedEventArgs e)
        {
            //
        }

        private List<Square> _squares = new List<Square>();
        public List<Square> Squares
        {
            get { return _squares; }
        }

        private List<int> _mustContainValues = new List<int>();
        public List<int> MustContainValues
        {
            get { return _mustContainValues; }
        }

        public void MustContainValue(int mustContainValue)
        {
            if(!MustContainValues.Contains(mustContainValue))
            {
                MustContainValues.Add(mustContainValue);
                //just raise the event. The series will respond by excluding the value in other series
                //which will eventually leave only one option for a value in a series. 
                //This will be solved by the series
                OnMustContainValueAdded(this,new MustContainValueAddedEventArgs(mustContainValue));
            }
        }

        private List<int> _cantContainValues = new List<int>();
        public List<int> CantContainValues
        {
            get { return _cantContainValues; }
        }

        public void CantContainValue(int cantContainValue)
        {
            if (!CantContainValues.Contains(cantContainValue))
            {
                CantContainValues.Add(cantContainValue);

                //Exclude this value in any of the squares
                foreach (Square sq in this.Squares)
                {
                    sq.ExcludeValue(cantContainValue);
                }
                //no event needs to be raised, because the squares already do so
                //OnCantContainValueAdded(this, new CantContainValueAddedEventArgs(CantContainValue));
            }
        }

        public void ExcludeValue(int value)
        {
            //Exclude this value in each square
            //raising events is not needed at this time, because the squares themselves will do so
        }

        public delegate void MustContainValueAddedHandler(MiniSeries sender, MustContainValueAddedEventArgs e);
        public event MustContainValueAddedHandler OnMustContainValueAdded;

        public delegate void CantContainValueAddedHandler(MiniSeries sender, CantContainValueAddedEventArgs e);
        public event CantContainValueAddedHandler OnCantContainValueAdded;

    }
    public class MustContainValueAddedEventArgs : EventArgs
    {
        public MustContainValueAddedEventArgs()
        {
        }

        public MustContainValueAddedEventArgs(int addedValue)
        {
            this.AddedValue = addedValue;
        }

        private int _addedValue;
        public int AddedValue
        {
            get { return _addedValue; }
            set { _addedValue = value; }
        }
    }

    public class CantContainValueAddedEventArgs : EventArgs
    {
        public CantContainValueAddedEventArgs()
        {
        }

        public CantContainValueAddedEventArgs(int addedValue)
        {
            this.AddedValue = addedValue;
        }

        private int _addedValue;
        public int AddedValue
        {
            get { return _addedValue; }
            set { _addedValue = value; }
        }
    }
}
