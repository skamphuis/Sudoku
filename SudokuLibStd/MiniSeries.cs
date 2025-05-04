using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuLib
{
    public class MiniSeries
    {
        /// <summary>
        /// Constructor for a miniseries
        /// </summary>
        /// <param name="size">Size of the miniseries, is the Sqrt of the board size</param>
        public MiniSeries(int size)
        {
            this.Size = size;
            this.Squares = new List<Square>();
            for (int i = 0; i < size; i++)
            {
                this.Squares.Add(null);
            }
        }

        public MiniSeries(List<Square> squares) : this(squares.Count)
        {
            foreach (var sq in squares)
            {
                //add event handlers
                sq.OnSquareExcludedChanged += sq_OnSquareExcludedChanged;
                sq.OnSquareSolved += sq_OnSquareSolved;
            }
        }

        public int Size { get; private set; }

        void sq_OnSquareSolved(object sender, SquareSolvedEventArgs e)
        {
            //
        }

        void sq_OnSquareExcludedChanged(object sender, ExcludedChangedEventArgs e)
        {
            //
        }

        public List<Square> Squares { get; private set; }

        public List<int> MustContainValues { get; } = [];

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

        public List<int> CantContainValues { get; } = [];

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

        public int AddedValue { get; set; }
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

        public int AddedValue { get; set; }
    }
}
