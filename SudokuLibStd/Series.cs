using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuLib
{
    public enum SeriesType
    {
        Row, Column, Box
    }
    public class Series
    {
        public Series(int size, SeriesType seriesType, int seriesIndex)
        {
            this.Size = size;
            this.SeriesType = seriesType;
            this.SeriesIndex = seriesIndex;

            if (seriesType is SeriesType.Row or SeriesType.Box)
            {
                MiniSeriesHorizontal = [];
                for (var i = 0; i < BoxSize; i++)
                {
                    AddMiniSeriesHorizontal(new MiniSeries(BoxSize));
                }
            }
            if (seriesType is SeriesType.Column or SeriesType.Box)
            {
                MiniSeriesVertical = [];
                for (var i = 0; i < BoxSize; i++)
                {
                    AddMiniSeriesVertical(new MiniSeries(BoxSize));
                }
            }
        }

        public int Size { get; private set; }
        public int BoxSize => (int)Math.Sqrt(Size);
        public List<Square> Squares { get; } = [];

        public void AddSquare(Square square)
        {
            Squares.Add(square);
            square.OnSquareSolved += square_OnSquareSolved;
            square.OnSquareExcludedChanged += square_OnSquareExcludedChanged;
            //square.OnSquareUnSolved += new Square.SquareUnSolvedHandler(square_OnSquareUnSolved);

            AddSquareToMiniSeries(square);
        }

        private void AddSquareToMiniSeries(Square square)
        {
            //add square to the correct miniseries
            if (SeriesType is SeriesType.Row or SeriesType.Box)
            {
                // this is a row, so add to the correct miniseries
                // based on the boxColumnIndex and then the boxColumnPosition
                MiniSeriesHorizontal[square.BoxColumnIndex].Squares[square.BoxColumnPos] = square;
            }
            else if (SeriesType is SeriesType.Column or SeriesType.Box)
            {
                // this is a column, so add to the correct miniseries
                // based on the boxRowIndex and then the boxRowPosition
                MiniSeriesVertical[square.BoxRowIndex].Squares[square.BoxRowPos] = square;
            }
        }
        public List<int> KnownValues
        {
            get
            {
                var values = new List<int>();
                foreach (var sq in Squares)
                {
                    if (sq.IsPreset || sq.IsSolved) { values.Add(sq.Value); }
                }
                return values;
            }
        }

        public List<int> NeededValues
        {
            get
            {
                List<int> values = new List<int>();
                List<int> known = KnownValues;

                for (int i = 1; i <= Squares.Count; i++)
                {
                    if (!known.Contains(i))
                    {
                        values.Add(i);
                    }
                }

                return values;
            }
        }

        public SeriesType SeriesType { get; set; }

        public int SeriesIndex { get; set; }

        public List<MiniSeries> MiniSeriesHorizontal { get; } = new List<MiniSeries>();

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

        public List<MiniSeries> MiniSeriesVertical { get; }

        public void AddMiniSeriesVertical(MiniSeries miniSeries)
        {
            MiniSeriesVertical.Add(miniSeries);
            miniSeries.OnMustContainValueAdded += miniSeriesVertical_OnMustContainValueAdded;
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
            foreach (Square square in Squares)
            {
                if (square.Number != ((Square)sender).Number)
                {
                    square.ExcludeValue(e.KnownValue);
                }
            }

            findSinglePossibilities();
        }

        void square_OnSquareExcludedChanged(object sender, ExcludedChangedEventArgs e)
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
                List<Square> foundSquares = new List<Square>();

                //loop through square to see if there's only 1 with this possible value
                foreach (Square square in Squares)
                {
                    if (!square.IsKnown && square.PossibleValues.Contains(val))
                    {
                        foundSquares.Add(square);
                        if (foundSquares.Count > 1)
                        {
                            //this value was found before: found it again, so not solved
                            break;
                        }
                    }
                }

                //now see if there's only one square with this possible value
                if (foundSquares.Count == 1)
                {
                    foundSquares[0].SolvedValue = val;
                }
            }
        }
    }
}
