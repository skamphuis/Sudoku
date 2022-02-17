using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SudokuSolver
{
    public partial class SetValue : Form
    {
        public SetValue(SudokuLib.Square square)
        {
            InitializeComponent();

            mySquare = square;

            if (mySquare.IsKnown)
            {
                MessageBox.Show("This square is already solved or known.");
                return;
            }
            foreach (int val in square.ExcludedValues)
            {
                this.Controls["button" + val.ToString()].Enabled = false;
            }
            
            this.ShowDialog();
        }

        private SudokuLib.Square mySquare;

        private void button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Name == buttonNoValue.Name)
            {
                //mySquare.UnsetValue();
            }
            else
            {
                try
                {
                    mySquare.PresetValue = int.Parse(btn.Name.Substring(btn.Name.Length - 1));

                }
                catch (SudokuLib.InvalidPresetException)
                {
                    MessageBox.Show("Sorry, this wasn't possible.");
                    btn.Enabled = false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            this.Close();
        }
    }
}