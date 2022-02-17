using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SudokuSolver
{
    public partial class SudokuBoard : UserControl
    {
        public SudokuBoard()
        {
            InitializeComponent();
            this.Board = new SudokuLib.Board();

            this.Board.OnSudokuSolved += new SudokuLib.Board.SudokuSolvedHandler(Board_OnSudokuSolved);
            drawBoard();
        }

        void series_OnSeriesSolved(SudokuLib.Series sender, EventArgs e)
        {
            foreach (SudokuLib.Square sq in sender.Squares)
            {
                this.Controls["square" + sq.Number.ToString()].BackColor = Color.LightGreen;
            }
        }

        void Board_OnSudokuSolved(SudokuLib.Board sender, EventArgs e)
        {
            this.BackColor = Color.LightGreen;
        }

        private SudokuLib.Board _board;
        public SudokuLib.Board Board
        {
            get { return _board; }
            set
            {
                _board = value;

            }
        }

        private void drawBoard()
        {
            this.Controls.Clear();

            int maxLeft = 0;
            int maxTop = 0;

            SudokuSquare ctlSq;

            this.SuspendLayout();

            foreach (SudokuLib.Square square in this.Board.AllSquares)
            {
                int boxSize = (int)Math.Sqrt(Board.Size);
                ctlSq = new SudokuSquare(square);
                ctlSq.Left = 4 + (square.ColumnIndex * (ctlSq.Width + 2)) + (6 * (int)Math.Floor((double)square.ColumnIndex / boxSize));

                ctlSq.Top = 4 + (square.RowIndex * (ctlSq.Height + 1)) + (6 * (int)Math.Floor((double)square.RowIndex / boxSize));
                this.Controls.Add(ctlSq);
                ctlSq.Click += new EventHandler(ctlSq_Click);

                if (ctlSq.Left > maxLeft)
                {
                    maxLeft = ctlSq.Left;
                    this.Width = maxLeft + ctlSq.Width;
                }
                if (ctlSq.Top > maxTop)
                {
                    maxTop = ctlSq.Top;
                    this.Height = maxTop + ctlSq.Height;
                }
            }
            this.Width += 4;
            this.Height += 4;

            this.ResumeLayout();
        }

        void ctlSq_Click(object sender, EventArgs e)
        {
            SetValue setValueForm;

            setValueForm = new SetValue(((SudokuSquare)sender).MySquare);
        }

    }
}
