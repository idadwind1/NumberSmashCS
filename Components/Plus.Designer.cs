namespace NumberGamePlus.Components
{
    partial class Plus
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.plus_lbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // plus_lbl
            // 
            this.plus_lbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plus_lbl.Location = new System.Drawing.Point(0, 0);
            this.plus_lbl.Name = "plus_lbl";
            this.plus_lbl.Size = new System.Drawing.Size(150, 150);
            this.plus_lbl.TabIndex = 0;
            this.plus_lbl.Text = "+";
            this.plus_lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Plus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.plus_lbl);
            this.Name = "Plus";
            this.Resize += new System.EventHandler(this.Plus_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label plus_lbl;
    }
}
