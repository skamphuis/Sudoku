using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SudokuLib;

namespace SudokuSolver
{
    enum KnownValueType
    {
        Solved,
        Preset,
        Error
    }

    public partial class SudokuSquare : UserControl
    {
        private Font _defaultFont;
        private Font _focusFont;

        public SudokuSquare(Square square)
        {
            InitializeComponent();
            this.MySquare = square;

            _defaultFont = new Font(lblPos5.Font, FontStyle.Regular);
            _focusFont = new Font(lblPos5.Font, FontStyle.Bold);
        }

        private void initHandlers()
        {
            this.lblPos1.Click += new System.EventHandler(this.SudokuSquare_Click);
            this.lblPos2.Click += new System.EventHandler(this.SudokuSquare_Click);
            this.lblPos3.Click += new System.EventHandler(this.SudokuSquare_Click);
            this.lblPos4.Click += new System.EventHandler(this.SudokuSquare_Click);
            this.lblPos5.Click += new System.EventHandler(this.SudokuSquare_Click);
            this.lblPos6.Click += new System.EventHandler(this.SudokuSquare_Click);
            this.lblPos7.Click += new System.EventHandler(this.SudokuSquare_Click);
            this.lblPos8.Click += new System.EventHandler(this.SudokuSquare_Click);
            this.lblPos9.Click += new System.EventHandler(this.SudokuSquare_Click);
            this.lblValue.Click += new System.EventHandler(this.SudokuSquare_Click);

            _mySquare.OnSquareSolved += new Square.SquareSolvedHandler(MySquare_OnSquareSolved);
            //_mySquare.OnSquareUnSolved += new Square.SquareUnSolvedHandler(_mySquare_OnSquareUnSolved);
            _mySquare.OnSquareNotSolvable += new Square.SquareNotSolvableHandler(MySquare_OnSquareNotSolvable);
            _mySquare.OnSquareExcludedChanged += new Square.SquareExcludedChangedHandler(MySquare_OnSquareExcludedChanged);
        }

        private Color ColorError = Color.Red;
        private Color ColorExcluded = Color.White;
        private Color ColorPreset = Color.Blue;
        private Color ColorSolved = Color.Green;
        private Color ColorDefault = Color.Gray;

        private int _squareNumber;
        public int SquareNumber
        {
            get { return _squareNumber; }
            set { _squareNumber = value; }
        }

        private Square _mySquare;
        public Square MySquare
        {
            get { return _mySquare; }
            set
            {
                _mySquare = value;

                initHandlers();

                if (_mySquare.IsPreset)
                {
                    this.BackColor = ColorPreset;
                }
            }
        }

        void MySquare_OnSquareExcludedChanged(Square sender, ExcludedChangedEventArgs e)
        {
            Label lblValue = (Label)this.Controls["lblPos" + e.ChangedValue.ToString()];
            if (e.IsExcluded)
            {
                lblValue.ForeColor = ColorExcluded;
            }
            else
            { 
                lblValue.ForeColor = ColorDefault; 
            }
        }

        void MySquare_OnSquareNotSolvable(Square sender, EventArgs e)
        {
            setValue(int.MinValue, KnownValueType.Error);
        }

        void MySquare_OnSquareSolved(Square sender, SquareSolvedEventArgs e)
        {
            if (e.IsPreset)
            {
                setValue(e.KnownValue, KnownValueType.Preset);
            }
            else
            {
                setValue(e.KnownValue, KnownValueType.Solved);
            }
        }

        //void _mySquare_OnSquareUnSolved(Square sender, SquareSolvedEventArgs e)
        //{
        //    //e.KnownValue contains the preset value to unset
        //    if (e.IsPreset)
        //    {
        //        unSetValue(e.KnownValue, KnownValueType.Preset);
        //    }
        //    else
        //    {
        //        setValue(e.KnownValue, KnownValueType.Solved);
        //    }
        //}

        private void initPossibilities(bool bVisible)
        {
            foreach (Control label in this.Controls)
            {
                if (label.Name.StartsWith("lblPos"))
                {
                    if (MySquare.PossibleValues.Contains(int.Parse(label.Name.Substring(label.Name.Length - 1))))
                    {
                        label.ForeColor = ColorDefault;
                        label.Visible = bVisible;
                    }
                    else
                    {
                        label.ForeColor = ColorExcluded;
                        label.Visible = bVisible;
                    }

                }
                if (label.Name == "lblValue")
                {
                    label.ForeColor = ColorDefault;
                    label.Visible = !bVisible;
                }
            }
        }

        private void setValue(int value, KnownValueType valueType)
        {
            initPossibilities(false);
            lblValue.Text = value.ToString();

            switch (valueType)
            {
                case KnownValueType.Solved:
                    lblValue.ForeColor = ColorSolved;
                    break;
                case KnownValueType.Preset:
                    lblValue.ForeColor = ColorPreset;
                    break;
                case KnownValueType.Error:
                    lblValue.Text = "X";
                    lblValue.ForeColor = ColorError;
                    break;
                default:
                    lblValue.ForeColor = ColorDefault;
                    break;
            }
        }

        private void unSetValue(int value, KnownValueType valueType)
        {
            initPossibilities(true);
            lblValue.Text = "0";
        }

        private void SudokuSquare_Click(object sender, EventArgs e)
        {
            SetValue _setValueForm = new SetValue(this.MySquare);
        }

        private void SudokuSquare_Enter(object sender, EventArgs e)
        {
            Label lbl; 
            for (int index = 1; index < 10; index++)
            {
                if (MySquare.PossibleValues.Contains(index))
                {

                    lbl = ((Label)this.Controls["lblPos" + index.ToString()]);
                    lbl.Font = _focusFont;
                    lbl.ForeColor = ColorPreset;
                }
            }
        }

        private void SudokuSquare_Leave(object sender, EventArgs e)
        {
            Label lbl; 
            for (int index = 1; index < 10; index++)
            {
                if (MySquare.PossibleValues.Contains(index))
                {
                    lbl = ((Label)this.Controls["lblPos" + index.ToString()]);
                    lbl.Font = _defaultFont;
                    lbl.ForeColor = ColorDefault;
                }

            }
        }

        private void SudokuSquare_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("123456789".IndexOf(e.KeyChar) >= 0)
            {
                int value = int.Parse(e.KeyChar.ToString());
                if (MySquare.PossibleValues.Contains(value))
                {
                    MySquare.PresetValue = value;
                }
            }
        }

    }
}
