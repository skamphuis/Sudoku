using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSolver
{
    public partial class Import: Form
    {
        public Import()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public string Json
        {
            get { return txtJson.Text; }
            set { txtJson.Text = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Json = "";
            this.Close();
        }
    }
}
