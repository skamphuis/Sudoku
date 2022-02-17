using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SudokuSolver
{
    public partial class SudokuForm : Form
    {
        public SudokuForm()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            pnlSudoku.Controls.Clear();
            SudokuBoard sb =new SudokuBoard();

            sb.Top = 0;
            sb.Left = 0;
            
            pnlSudoku.Controls.Add(sb);

            sb.Board.OnBoardEvent += new SudokuLib.Board.BoardEventHandler(Board_OnBoardEvent);
            this.propertyGrid1.SelectedObject = sb.Board;
            pnlSudoku.Select();
        }

        void Board_OnBoardEvent(SudokuLib.Board sender, EventArgs e)
        {
            this.propertyGrid1.Refresh();
        }

    }
}