using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NumberGamePlus.Components
{
    public partial class Number : UserControl
    {
        public int value;
        public Random RandomGenerator
        {
            get => _RandomGenerator;
            set
            {
                _RandomGenerator = value;
                InitNumber();
            }
        }
        public Random _RandomGenerator;

        public void InitNumber()
        {
            var random = RandomGenerator.Next(-9, 9);
            value = random;
            if (random < 0)
                Number_lbl.Text = string.Format("(-{0})", random);
            else
                Number_lbl.Text = random.ToString();
        }

        public Number()
        {
            InitializeComponent();
        }

        public Number(Random randomGenerator)
        {
            InitializeComponent();
            RandomGenerator = randomGenerator;
        }

        private void Number_Resize(object sender, EventArgs e)
        {
            Number_lbl.Font = new Font(Number_lbl.Font.FontFamily, Height, FontStyle.Regular);
        }
    }
}
