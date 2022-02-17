using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace SudokuLib
{
    public class Board
    {
        #region Constructors

        public Board()
        {
            this.Size = 9;

            int i=0;

            //first instanciate all series
            createAllSeries();

            Square sq;
            for (int iRow = 0; iRow < ((int)Size); iRow++)
            {
                for (int iCol = 0; iCol < ((int)Size); iCol++)
                {
                    i++;
                    sq = new Square(i, (int)Size);

                    AllSquares.Add(sq);
                    _columns[iCol].AddSquare(sq);
                    _rows[iRow].AddSquare(sq);

                    int boxcol = (int)Math.Floor((decimal)(iCol / 3));
                    int boxrow = (int)Math.Floor((decimal)(iRow / 3));

                    _boxes[boxcol + (boxrow * 3)].AddSquare(sq);

                    sq.OnSquareSolved += new Square.SquareSolvedHandler(sq_OnSquareSolved);
                    //sq.OnSquareUnSolved += new Square.SquareUnSolvedHandler(sq_OnSquareUnSolved);

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

        void sq_OnSquareSolved(Square sender, SquareSolvedEventArgs e)
        {
            _knownSquares++;
            if (e.IsPreset)
            { _presetSquares++; }
            else
            { _solvedSquares++; }

            OnBoardEvent(this, EventArgs.Empty);

            bool sudokuSolved = true;
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
                OnSudokuSolved(this, new EventArgs());
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
            get { return _size; }
            set {
                if (AllSquares.Count > 0)
                {
                    throw new Exception("Size cannot be set after the board has been initialised.");
                }
                else
                {
                    _size = value;
                }
            }
        }
        #endregion

        #region Series
        private List<Series> _allSeries;
        [Category("Squares"), DisplayName("All Series"), Browsable(false)]
        public List<Series> AllSeries
        {
            get { return _allSeries; }
        }

        private List<Series> _rows;
        [Category("Squares"), DisplayName("Rows"), Browsable(false)]
        public List<Series> Rows
        {
            get { return _rows; }
        }

        private List<Series> _columns;
        [Category("Squares"), DisplayName("Columns"), Browsable(false)]
        public List<Series> Columns
        {
            get { return _columns; }
        }

        private List<Series> _boxes;
        [Category("Squares"), DisplayName("Boxes"), Browsable(false)]
        public List<Series> Boxes
        {
            get { return _boxes; }
        }

        private void createAllSeries()
        {
            _columns = new List<Series>();
            _rows = new List<Series>();
            _boxes = new List<Series>();
            _allSeries=new List<Series>();

            createSeries(_columns,SeriesType.Column);
            createSeries(_rows,SeriesType.Row);
            createSeries(_boxes,SeriesType.Box);


        }
        private void createSeries(List<Series> series,SeriesType seriesType)
        {
            Series _series;
            for (int i = 1; i <= (int)(Size); i++)
            {
                _series = new Series(seriesType, i - 1);
                //_series.OnSeriesSolved += new Series.SeriesSolvedHandler(series_OnSeriesSolved);
                series.Add(_series);
                _allSeries.Add(_series);
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
        private List<Square> _allSquares = new List<Square>();
        [Category("Squares"), DisplayName("All Squares"), Browsable(false)]
        public List<Square> AllSquares
        {
            get { return _allSquares; }
            set { _allSquares = value; }
        }

        private int _solvedSquares = 0;
        [Category("Status"), ReadOnly(true), DisplayName("Solved Squares"),
        Description("The number of already solved squares.")]
        public int SolvedSquares
        {
            get
            {
                return _solvedSquares;
            }
        }

        private int _presetSquares = 0;
        [Category("Status"), ReadOnly(true), DisplayName("Preset Squares"),
        Description("The number of preset squares.")]
        public int PresetSquares
        {
            get
            {
                return _presetSquares;
            }
        }

        private int _knownSquares = 0;
        [Category("Status"), ReadOnly(true), DisplayName("Known Squares"),
        Description("The number of known squares.")]
        public int KnownSquares
        {
            get
            {
                return _knownSquares;
            }
        }

        //General event to tell "something happened"
        public delegate void BoardEventHandler(Board sender, EventArgs e);
        public event BoardEventHandler OnBoardEvent;

#endregion
    }
}
