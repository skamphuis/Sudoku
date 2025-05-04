using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace SudokuLib
{
    public class Board
    {
        #region Constructors

        public Board(int size)
        {
            this.Size = size;

            int i = 0;

            //first instantiate all series
            CreateAllSeries();

            Square sq;
            for (int iRow = 0; iRow < ((int)Size); iRow++)
            {
                for (int iCol = 0; iCol < ((int)Size); iCol++)
                {
                    i++;
                    sq = new Square(i, (int)Size);

                    AllSquares.Add(sq);
                    Columns![iCol].AddSquare(sq);
                    Rows![iRow].AddSquare(sq);

                    // which box is this square for?
                    var boxCol = (int)Math.Floor((decimal)(iCol / BoxSize));
                    var boxRow = (int)Math.Floor((decimal)(iRow / BoxSize));
                    Boxes![boxCol + (boxRow * BoxSize)].AddSquare(sq);

                    sq.OnSquareSolved += sq_OnSquareSolved;

                    //now find the right miniseries to put this square into
                }
            }
        }

        private void createMiniSeries()
        {
            //            int miniSeriesSize = (int)Math.Sqrt(this.Size);
            //            int maxCount = miniSeriesSize * Size

            //            List<MiniSeries> allMS = new List<MiniSeries>();

            //            //first the horizontal ones
            //            for (int i = 0; i < maxCount; i++)
            //            {
            //                Series row = Rows[Math.Floor((decimal)(i/miniSeriesSize))];

            //                int boxcol = (int)Math.Floor((decimal)(iCol / miniSeriesSize));
            //                int boxrow = (int)Math.Floor((decimal)(iRow / 3));
            //Series box = Boxes[
            //                MiniSeries ms;

            //                ms = new MiniSeries();
            //                s.AddMiniSeriesHorizontal(ms);


            //                ms = new MiniSeries();
            //                s.AddMiniSeriesVertical(ms);
            //            }

            //            foreach (Series s in _boxes)
            //            {
            //                int miniSeriesSize = (int)Math.Sqrt(this.Size);
            //                for (int i = 1; i <= miniSeriesSize; i++)
            //                {
            //                    MiniSeries ms;

            //                    ms= new MiniSeries();
            //                    s.AddMiniSeriesHorizontal(ms);


            //                    ms = new MiniSeries();
            //                    s.AddMiniSeriesVertical(ms);
            //                }
            //            }
        }

        #endregion

        //void sq_OnSquareUnSolved(Square sender, SquareSolvedEventArgs e)
        //{
        //    _knownSquares--;
        //    if (e.IsPreset)
        //    { _presetSquares--; }
        //    else
        //    { _solvedSquares--; }

        //    OnBoardEvent(this, EventArgs.Empty);
        //}

        private void sq_OnSquareSolved(object sender, SquareSolvedEventArgs e)
        {
            KnownSquares++;
            if (e.IsPreset)
            { PresetSquares++; }
            else
            { SolvedSquares++; }

            OnBoardEvent(this, EventArgs.Empty);

            var sudokuSolved = true;
            foreach (Square sq in AllSquares)
            {
                if (sq.IsKnown == false)
                {
                    sudokuSolved = false;
                    return;
                }
            }

            if (sudokuSolved)
            {
                OnSudokuSolved(this, EventArgs.Empty);
            }
        }
        public delegate void SudokuSolvedHandler(Board sender, EventArgs e);
        public event SudokuSolvedHandler OnSudokuSolved;

        #region Game Size Settings
        private int _size;
        [Category("Sudoku Settings"),
        Description("The size of each series in the Sudoku")]
        public int Size
        {
            get => _size;
            set
            {
                if (AllSquares.Count > 0)
                {
                    throw new Exception("Size cannot be set after the board has been initialised.");
                }
                if (value is < 4 or > 25)
                {
                    throw new Exception("Size must be between 4 and 16.");
                }
                else if (Math.Sqrt(value) % 1 != 0)
                {
                    throw new Exception("Size must be a perfect square.");
                }
                else
                {
                    _size = value;
                }
            }
        }

        public int BoxSize => (int)Math.Sqrt(Size);

        #endregion

        #region Series

        [Category("Squares"), DisplayName("All Series"), Browsable(false)]
        public List<Series> AllSeries { get; private set; }

        [Category("Squares"), DisplayName("Rows"), Browsable(false)]
        public List<Series> Rows { get; private set; }

        [Category("Squares"), DisplayName("Columns"), Browsable(false)]
        public List<Series> Columns { get; private set; }

        [Category("Squares"), DisplayName("Boxes"), Browsable(false)]
        public List<Series> Boxes { get; private set; }

        private void CreateAllSeries()
        {
            Columns = new List<Series>();
            Rows = new List<Series>();
            Boxes = new List<Series>();
            AllSeries = new List<Series>();

            CreateSeries(Columns, SeriesType.Column);
            CreateSeries(Rows, SeriesType.Row);
            CreateSeries(Boxes, SeriesType.Box);
        }
        private void CreateSeries(List<Series> series, SeriesType seriesType)
        {
            Series _series;
            for (int i = 0; i < (int)(Size); i++)
            {
                _series = new Series(Size, seriesType, i);
                //_series.OnSeriesSolved += new Series.SeriesSolvedHandler(series_OnSeriesSolved);
                series.Add(_series);
                AllSeries.Add(_series);
            }
        }

        void series_OnSeriesSolved(Series sender, EventArgs e)
        {
            OnBoardEvent(this, EventArgs.Empty);
        }
        #endregion

        public void SetUndoHistoryPoint()
        {

        }

        #region All Squares

        [Category("Squares"), DisplayName("All Squares"), Browsable(false)]
        public List<Square> AllSquares { get; set; } = new List<Square>();

        [Category("Status"), ReadOnly(true), DisplayName("Solved Squares"),
         Description("The number of already solved squares.")]
        public int SolvedSquares { get; private set; } = 0;

        [Category("Status"), ReadOnly(true), DisplayName("Preset Squares"),
         Description("The number of preset squares.")]
        public int PresetSquares { get; private set; } = 0;

        [Category("Status"), ReadOnly(true), DisplayName("Known Squares"),
         Description("The number of known squares.")]
        public int KnownSquares { get; private set; } = 0;

        //General event to tell "something happened"
        public delegate void BoardEventHandler(Board sender, EventArgs e);
        public event BoardEventHandler OnBoardEvent;

        #endregion
    }
}
