namespace NumberGamePlus.Components
{
    partial class Number
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
            this.Number_lbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Number_lbl
            // 
            this.Number_lbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Number_lbl.Location = new System.Drawing.Point(0, 0);
            this.Number_lbl.Name = "Number_lbl";
            this.Number_lbl.Size = new System.Drawing.Size(44, 16);
            this.Number_lbl.TabIndex = 0;
            this.Number_lbl.Text = "label1";
            this.Number_lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Number
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.Number_lbl);
            this.Name = "Number";
            this.Size = new System.Drawing.Size(44, 16);
            this.Resize += new System.EventHandler(this.Number_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Number_lbl;
    }
}
