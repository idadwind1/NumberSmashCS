using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Markup;

namespace NumberGamePlus.Components
{
    public partial class Number : UserControl
    {
        [Browsable(false)]
        public override string Text
        {
            get => Value < 0 ? string.Format("({0})", Value) : Value.ToString();
            set => throw new ReadOnlyException();
        }

        private int _Value;
        public int Value
        {
            get => _Value;
            set
            {
                _Value = value;
                if (ValueChanged != null)
                    ValueChanged(this, EventArgs.Empty);
            }
        }

        public bool Selected
        {
            get => number_cbx.Checked;
            set => number_cbx.Checked = value;
        }

        public Random RandomGenerator
        {
            get => _RandomGenerator;
            set
            {
                if (value == null) return;
                _RandomGenerator = value;
                InitNumber();
            }
        }
        private Random _RandomGenerator;

        public void InitNumber()
        {
            var random = RandomGenerator.Next(-9, 9);
            Value = random;
            if (random < 0)
                number_cbx.Text = string.Format("({0})", random);
            else
                number_cbx.Text = random.ToString();
        }

        public Number()
        {
            InitializeComponent();
        }

        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                number_cbx.Font = value;
            }
        }

        private void Number_Resize(object sender, EventArgs e)
        {
            /*var lblWidth = Number_lbl.Width;
            if (lblWidth <= 0) return;
            var size = Number_lbl.Font.Size;
            Number_lbl.AutoSize = true;
            Number_lbl.Dock = DockStyle.None;
            if (Number_lbl.Width > lblWidth)
                while (Number_lbl.Width > lblWidth)
                {
                    size -= 0.25F;
                    if (size <= 0) return;
                    Number_lbl.Font = new Font(Number_lbl.Font.FontFamily, size, FontStyle.Regular, GraphicsUnit.World);
                }
            else if (Number_lbl.Width < lblWidth)
                while (Number_lbl.Width < lblWidth)
                {
                    size += 0.25F;
                    Number_lbl.Font = new Font(Number_lbl.Font.FontFamily, size, FontStyle.Regular, GraphicsUnit.World);
                }
            Number_lbl.AutoSize = false;
            Number_lbl.Dock = DockStyle.Fill;
            Number_lbl.Width = lblWidth;*/
        }

        public delegate void SelectedChangedHandle(object sender, EventArgs e);

        public event SelectedChangedHandle SelectedChanged;

        private void number_cbx_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectedChanged != null)
                SelectedChanged(this, EventArgs.Empty);
        }

        public delegate void ValueChangedHandle(object sender, EventArgs e);

        public event ValueChangedHandle ValueChanged;
    }
}
