using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NumberGamePlus.Classes;
using static NumberGamePlus.Classes.NumberValue;

namespace NumberGamePlus.Components
{
    public partial class Number : UserControl
    {
        [Browsable(false), ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => Value.ValueString;
            set => throw new ReadOnlyException();
        }

        private NumberValue _Value = new NumberValue();

        [Browsable(false), ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NumberValue Value
        {
            get => _Value;
            set
            {
                _Value = value;
                ValueChanged(this, new EventArgs());
            }
        }

        public bool Selected
        {
            get => number_cbx.Checked;
            set => number_cbx.Checked = value;
        }

        public bool ExtendedFeaturesToggle = false;

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

        public void InitNumber() => InitNumber(ExtendedFeaturesToggle);

        private void InitNumber(bool extended_features)
        {
            var new_value = 0;
            NumberType numberType = NumberType.Common;
            if (!extended_features)
            {
                new_value = RandomGenerator.Next(-9, 10);
                Value = new NumberValue(NumberType.Common, new_value);
                number_cbx.Text = Value.ValueString;
                return;
            }
            var possibilities = Enumerable.Repeat(0, 90).ToList();
            possibilities.AddRange(Enumerable.Repeat(1, 5));
            possibilities.AddRange(Enumerable.Range(2, 5));
            var value = possibilities[RandomGenerator.Next(possibilities.Count)];
            switch (value)
            {
                case 0: //Common
                    InitNumber(false);
                    return;
                case 1: //Signum
                    numberType = NumberType.Signum;
                    new_value = RandomGenerator.Next(1, 10);
                    break;
                case 2: //Infinitive
                    numberType = NumberType.Infinitive;
                    break;
                case 3: //Double
                    numberType = NumberType.Double;
                    break;
                case 4: //Unkown
                    numberType = NumberType.Unknown;
                    break;
                case 5: //Null
                    numberType = NumberType.Null;
                    break;
                case 6:
                    numberType = NumberType.TimesZero;
                    break;
            }
            if (value >= 2)
                new_value = int.MinValue;
            Value = new NumberValue(numberType, new_value);
            number_cbx.Text = Value.ValueString;
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

        /*private void Number_Resize(object sender, EventArgs e)
        {
            var lblWidth = Number_lbl.Width;
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
            Number_lbl.Width = lblWidth;
        }*/

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
