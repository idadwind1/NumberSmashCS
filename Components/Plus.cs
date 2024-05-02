using System;
using System.Drawing;
using System.Windows.Forms;

namespace NumberGamePlus.Components
{
    public partial class Plus : UserControl
    {
        public Plus()
        {
            InitializeComponent();
            TabStop = false;
        }

        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                plus_lbl.Font = value;
            }
        }

        private void Plus_Resize(object sender, EventArgs e)
        {
            /*var lblWidth = plus_lbl.Width;
            if (lblWidth <= 0) return;
            var size = plus_lbl.Font.Size;
            plus_lbl.AutoSize = true;
            plus_lbl.Dock = DockStyle.None;
            if (plus_lbl.Width > lblWidth)
                while (plus_lbl.Width > lblWidth)
                {
                    size -= 0.25F;
                    if (size <= 0) return;
                    plus_lbl.Font = new Font(plus_lbl.Font.FontFamily, size, FontStyle.Regular, GraphicsUnit.World);
                }
            else if (plus_lbl.Width < lblWidth)
                while (plus_lbl.Width < lblWidth)
                {
                    size += 0.25F;
                    plus_lbl.Font = new Font(plus_lbl.Font.FontFamily, size, FontStyle.Regular, GraphicsUnit.World);
                }
            plus_lbl.AutoSize = false;
            plus_lbl.Dock = DockStyle.Fill;
            plus_lbl.Width = lblWidth;*/
        }
    }
}
