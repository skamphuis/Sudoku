using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuLib
{
    public enum SeriesType
    {
        Row,Column,Box
    }
    public class Series
    {
        public Series(SeriesType seriesType, int seriesIndex)
        {
            this.SeriesType = seriesType;
            this.SeriesIndex = seriesIndex;
        }

        private List<Square> _squares=new List<Square>();
        public List<Square> Squares
        {
            get { return _squares; }
        }

        public void AddSquare(Square square)
        {
            _squares.Add(square);
            square.OnSquareSolved += square_OnSquareSolved;
            square.OnSquareExcludedChanged += new Square.SquareExcludedChangedHandler(square_OnSquareExcludedChanged);
            //square.OnSquareUnSolved += new Square.SquareUnSolvedHandler(square_OnSquareUnSolved);
        }

        public List<int> KnownValues
        {
            get {
                List<int> _values = new List<int>();
                foreach (Square sq in Squares)
                {
                    if (sq.IsPreset || sq.IsSolved) { _values.Add(sq.Value); }
                }
                return _values; }
        }

        public List<int> NeededValues
        {
            get
            {
                List<int> _values = new List<int>();
                List<int> _known = KnownValues;

                for (int i = 1; i <= Squares.Count; i++)
                {
                    if (!_known.Contains(i))
                    {
                        _values.Add(i);
                    }
                }
                
                return _values;
            }
        }

        private SeriesType _seriesType;
        public SeriesType SeriesType
        {
            get { return _seriesType; }
            set { _seriesType = value; }
        }
        private int _seriesIndex;
        public int SeriesIndex
        {
            get { return _seriesIndex; }
            set { _seriesIndex = value; }
        }

        private List<MiniSeries> _miniSeriesHorizontal = new List<MiniSeries>();
        public List<MiniSeries> MiniSeriesHorizontal
        {
            get { return _miniSeriesHorizontal; }
        }
        public void AddMiniSeriesHorizontal(MiniSeries miniSeries)
        {
            MiniSeriesHorizontal.Add(miniSeries);
            miniSeries.OnMustContainValueAdded += new MiniSeries.MustContainValueAddedHandler(miniSeriesHorizontal_OnMustContainValueAdded);
        }

        void miniSeriesHorizontal_OnMustContainValueAdded(MiniSeries sender, MustContainValueAddedEventArgs e)
        {
            //tell the other miniseries they can't contain this value
            foreach (MiniSeries ms in MiniSeriesHorizontal)
            {
                if (!ms.Equals(sender))
                {
                    ms.ExcludeValue(e.AddedValue);
                }
            }
        }

        private List<MiniSeries> _miniSeriesVertical;
        public List<MiniSeries> MiniSeriesVertical
        {
            get { return _miniSeriesVertical; }
        }
        public void AddMiniSeriesVertical(MiniSeries miniSeries)
        {
            MiniSeriesVertical.Add(miniSeries);
            miniSeries.OnMustContainValueAdded += new MiniSeries.MustContainValueAddedHandler(miniSeriesVertical_OnMustContainValueAdded);
        }

        void miniSeriesVertical_OnMustContainValueAdded(MiniSeries sender, MustContainValueAddedEventArgs e)
        {
            //tell the other miniseries they can't contain this value
            foreach (MiniSeries ms in MiniSeriesVertical)
            {
                if (!ms.Equals(sender))
                {
                    ms.ExcludeValue(e.AddedValue);
                }
            }
        }


        void square_OnSquareSolved(object sender, SquareSolvedEventArgs e)
        {
            //loop through the other squares in this series to 
            //inform them about a new exluded value
            foreach (Square square in _squares)
            {
                if (square.Number != ((Square)sender).Number)
                {
                    square.ExcludeValue(e.KnownValue);
                }
            }

            findSinglePossibilities();

            //if (seriesIsSolved)
            //{
            //    OnSeriesSolved(this, new EventArgs());
            //}
        }

        void square_OnSquareExcludedChanged(Square sender, ExcludedChangedEventArgs e)
        {
            //We'll need to find out if this exclusion of a value leaves
            //a single place for this value;
            findSinglePossibilities();
        }

        /// <summary>
        /// This method tries to solve square on series-level.
        /// For example: if only one square has 7 in it's possible values, it can be set to 7
        /// although it had more possible values itself
        /// </summary>
        private void findSinglePossibilities()
        {
            foreach (int val in this.NeededValues)
            {
                //set found indicator to false
                List<Square> foundSquares=new List<Square>();

                //loop through square to see if there's only 1 with this possible value
                foreach (Square square in _squares)
                {
                    if (!square.IsKnown && square.PossibleValues.Contains(val))
                    {
                        foundSquares.Add(square);
                        if (foundSquares.Count>1)
                        {
                            //this value was found before: found it again, so not solved
                            break;
                        }
                    }
                }

                //now see if there's only one square with this possible value
                if(foundSquares.Count==1)
                {
                    foundSquares[0].SolvedValue = val;
                }
            }
        }
        //private void square_OnSquareUnSolved(Square sender, SquareSolvedEventArgs e)
        //{
        //    //loop through the other squares in this series to 
        //    //inform them about a new available value
        //    foreach (Square square in _squares)
        //    {
        //        if (square.Number != sender.Number)
        //        {
        //            square.IncludeValue(e.KnownValue);
        //        }
        //    }
        //}

        //public delegate void SeriesSolvedHandler(Series sender, EventArgs e);
        //public event SeriesSolvedHandler OnSeriesSolved;
    }
}
