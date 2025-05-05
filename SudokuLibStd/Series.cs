using System;
using System.Collections.Generic;
using System.Linq;
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

            MiniSeriesHorizontal = [];
            MiniSeriesVertical = [];
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
                var values = new List<int>();
                var known = KnownValues;

                for (int i = 1; i <= this.Size; i++)
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

        public List<MiniSeries> MiniSeriesHorizontal { get; }
        public List<MiniSeries> MiniSeriesVertical { get; }
        public List<MiniSeries> AllMiniSeries => MiniSeriesHorizontal.Concat(MiniSeriesVertical).ToList();
        public void AddMiniSeriesHorizontal(MiniSeries miniSeries)
        {
            MiniSeriesHorizontal.Add(miniSeries);
            miniSeries.OnMustContainValueAdded += miniSeries_OnMustContainValueAdded;
            miniSeries.OnCantContainValueAdded += miniSeries_OnCantContainValueAdded;
        }

        void miniSeries_OnMustContainValueAdded(MiniSeries sender, MustContainValueAddedEventArgs e)
        {
            // tell the other miniseries (not containing any of the squares of the passed in miniseries)
            // they can't contain this value
            foreach (MiniSeries ms in AllMiniSeries)
            {
                if (ms.Squares.TrueForAll(
                        s => e.MiniSeries.Squares.TrueForAll(
                            s1 => s.Number != s1.Number)))
                {
                    ms.ExcludeValue(e.AddedValue);
                }
            }
        }

        public void AddMiniSeriesVertical(MiniSeries miniSeries)
        {
            MiniSeriesVertical.Add(miniSeries);
            miniSeries.OnMustContainValueAdded += miniSeries_OnMustContainValueAdded;
            miniSeries.OnCantContainValueAdded += miniSeries_OnCantContainValueAdded;
        }

        private void miniSeries_OnCantContainValueAdded(MiniSeries sender, CantContainValueAddedEventArgs e)
        {
            // TODO?
        }

        void square_OnSquareSolved(object sender, SquareSolvedEventArgs e)
        {
            //loop through the other squares in this series to 
            //inform them about a new exluded value
            foreach (var square in Squares)
            {
                if (square.Number != ((Square)sender).Number)
                {
                    square.ExcludeValue(e.KnownValue);
                }
            }

            // if a value occurs in only one miniseries,
            // we can inform the other miniseries of all the series it is part of
            // and exclude this value from the other miniseries
            // this also means the miniseries should be a reference and
            // therefore be created on board level, like the series themselves

            findSinglePossibilities();
            //findMiniSeriesMustContain(e.KnownValue);
        }

        void square_OnSquareExcludedChanged(object sender, ExcludedChangedEventArgs e)
        {
            //We'll need to find out if this exclusion of a value leaves
            //a single place for this value;
            if(e.IsExcluded)
            {
                //this square is excluded from this value, so we can check if this value is now a single possibility in this series
                findSinglePossibilities();
                //findMiniSeriesMustContain(e.ChangedValue);
            }
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

        /// <summary>
        /// This method tries to find miniseries in this series of which it is now known
        /// they must contain the value that is now excluded from a square
        /// </summary>
        /// <param name="valueExcludedFromSquare"></param>
        public void findMiniSeriesMustContain(int valueExcludedFromSquare)
        {
            // loop through all horizontal miniseries.
            // if only 1 miniseries has this value as possibleValue,
            // it must be in that miniseries
            if(MiniSeriesHorizontal.Count(ms => ms.Squares.Any(s => s.PossibleValues.Contains(valueExcludedFromSquare))) == 1)
            {
                //this miniseries must contain the value
                MiniSeries ms = MiniSeriesHorizontal.First(ms => ms.Squares.Any(s => s.PossibleValues.Contains(valueExcludedFromSquare)));
                ms.MustContainValue(valueExcludedFromSquare);
            }
            if(MiniSeriesVertical.Count(ms => ms.Squares.Any(s => s.PossibleValues.Contains(valueExcludedFromSquare))) == 1)
            {
                //this miniseries must contain the value
                MiniSeries ms = MiniSeriesVertical.First(ms => ms.Squares.Any(s => s.PossibleValues.Contains(valueExcludedFromSquare)));
                ms.MustContainValue(valueExcludedFromSquare);
            }
        }
    }
}
