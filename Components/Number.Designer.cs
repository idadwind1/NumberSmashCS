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
            this.number_cbx = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // number_cbx
            // 
            this.number_cbx.Appearance = System.Windows.Forms.Appearance.Button;
            this.number_cbx.Cursor = System.Windows.Forms.Cursors.Hand;
            this.number_cbx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.number_cbx.Location = new System.Drawing.Point(0, 0);
            this.number_cbx.Name = "number_cbx";
            this.number_cbx.Size = new System.Drawing.Size(148, 119);
            this.number_cbx.TabIndex = 1;
            this.number_cbx.Text = "checkBox1";
            this.number_cbx.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.number_cbx.UseVisualStyleBackColor = true;
            this.number_cbx.CheckedChanged += new System.EventHandler(this.number_cbx_CheckedChanged);
            // 
            // Number
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.number_cbx);
            this.Name = "Number";
            this.Size = new System.Drawing.Size(148, 119);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox number_cbx;
    }
}
