namespace SudokuSolver
{
    partial class SudokuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SudokuForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.pnlSudoku = new System.Windows.Forms.Panel();
            this.pnlSudokuStatus = new System.Windows.Forms.Panel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1.SuspendLayout();
            this.pnlSudokuStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(801, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(40, 22);
            this.btnNew.Text = "&New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // pnlSudoku
            // 
            this.pnlSudoku.AutoScroll = true;
            this.pnlSudoku.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSudoku.Location = new System.Drawing.Point(0, 25);
            this.pnlSudoku.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlSudoku.Name = "pnlSudoku";
            this.pnlSudoku.Size = new System.Drawing.Size(441, 432);
            this.pnlSudoku.TabIndex = 1;
            // 
            // pnlSudokuStatus
            // 
            this.pnlSudokuStatus.Controls.Add(this.propertyGrid1);
            this.pnlSudokuStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSudokuStatus.Location = new System.Drawing.Point(441, 25);
            this.pnlSudokuStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlSudokuStatus.Name = "pnlSudokuStatus";
            this.pnlSudokuStatus.Size = new System.Drawing.Size(360, 432);
            this.pnlSudokuStatus.TabIndex = 2;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(360, 432);
            this.propertyGrid1.TabIndex = 0;
            // 
            // SudokuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(801, 457);
            this.Controls.Add(this.pnlSudokuStatus);
            this.Controls.Add(this.pnlSudoku);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SudokuForm";
            this.Text = "SudokuSolver";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlSudokuStatus.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel pnlSudoku;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.Panel pnlSudokuStatus;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}

