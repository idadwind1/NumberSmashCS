using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using NumberGamePlus.Components;

namespace NumberGamePlus
{
    public partial class MainForm : Form
    {
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                score_lbl.Text = value.ToString();
            }
        }

        private int _score = 0;

        public int HelpUsedTime
        {
            get => _help_used_time;
            set
            {
                _help_used_time = value;
                help_time_lbl.Text = value.ToString();
            }
        }

        private int _help_used_time = 0;

        public readonly Random RandomGenerator;
        public MainForm()
        {
            InitializeComponent();
            RandomGenerator = new Random();
            equation.RandomGenerator = RandomGenerator;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            equation.InitNumbers();
            timer1.Start();
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '+':
                    fontToolStripMenuItem_Click(sender, e);
                    break;
                case '-':
                    fontToolStripMenuItem1_Click(sender, e);
                    break;
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Font = new Font(Font.FontFamily, Font.Size + 1);
            menuStrip1.Font = new Font(menuStrip1.Font.FontFamily, menuStrip1.Font.Size + 1);
            statusStrip1.Font = new Font(statusStrip1.Font.FontFamily, statusStrip1.Font.Size + 1);
            equation.Font = new Font(equation.Font.FontFamily, equation.Font.Size + 1);
        }

        private void fontToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Font.Size > 1)
                Font = new Font(Font.FontFamily, Font.Size - 1);
            if (menuStrip1.Font.Size > 1)
                menuStrip1.Font = new Font(menuStrip1.Font.FontFamily, menuStrip1.Font.Size - 1);
            if (statusStrip1.Font.Size > 1)
                statusStrip1.Font = new Font(statusStrip1.Font.FontFamily, statusStrip1.Font.Size - 1);
            if (equation.Font.Size > 1)
                equation.Font = new Font(equation.Font.FontFamily, equation.Font.Size - 1);
        }

        private void groupBoxScore_Resize(object sender, EventArgs e)
        {
            var lblWidth = score_lbl.Width;
            if (lblWidth <= 0) return;
            var size = score_lbl.Font.Size;
            score_lbl.AutoSize = true;
            score_lbl.Dock = DockStyle.None;
            if (score_lbl.Width > lblWidth)
                while (score_lbl.Width > lblWidth)
                {
                    size -= 0.25F;
                    if (size <= 0) return;
                    score_lbl.Font = new Font(score_lbl.Font.FontFamily, size, FontStyle.Regular, GraphicsUnit.World);
                }
            else if (score_lbl.Width < lblWidth)
                while (score_lbl.Width < lblWidth)
                {
                    size += 0.25F;
                    score_lbl.Font = new Font(score_lbl.Font.FontFamily, size, FontStyle.Regular, GraphicsUnit.World);
                }
            score_lbl.AutoSize = false;
            score_lbl.Dock = DockStyle.Fill;
            score_lbl.Width = lblWidth;
        }

        private void submit_btn_Click(object sender, EventArgs e)
        {
            string ConnectNumbers(Number[] numbers)
                => string.Concat(numbers.Select((l, i) => i == 0 ? l.Text : "+" + l.Text));
            if (equation.SelectedItems.Count() <= 0)
            {
                var suitable = Algorithms.Algorithms.Get0Groups(equation.Values);
                if (suitable.Length == 0)
                {
                    equation.InitNumbers();
                    return;
                }
                Lose(string.Concat(
                    suitable
                    .Select(i => ConnectNumbers(
                        i.Select(j => equation.Numbers[j]).ToArray()) + "\n")));
                return;
            }
            if (equation.SelectedSum == 0)
            {
                foreach (var n in equation.SelectedItems)
                {
                    n.InitNumber();
                    n.Selected = false;
                }
                Score++;
                toolStripProgressBarTime.Value = 0;
                return;
            }
            Lose(string.Format("{0} = {1} ≠ 0",
                ConnectNumbers(equation.SelectedItems),
                equation.SelectedSum));
        }

        private DialogResult ShowMessageBox(string message, string title ="", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            DialogResult result;
            pause_cbx.Checked = true;
            result = MessageBox.Show(message, title, buttons, icon);
            pause_cbx.Checked = false;
            return result;
        }

        private void reset_btn_Click(object sender, EventArgs e)
        {
            if (!game_over && ShowMessageBox("Sure to reset?\nAll progress will be lost",
                "Reset", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) ==
                DialogResult.Cancel) return;
            seconds = hundred_milliseconds = Score = HelpUsedTime = 0;
            timing_lbl.Text = "0:00";
            pause_lbl.Text = "Game Paused";
            pause_lbl.Visible = false;
            equation.SelectedItems = new Number[] { };
            equation.InitNumbers();
            SetEnabled(this, true, null);
            SetMenuStripEnabled(menuStrip1, true, null, null);
            game_over = false;
            if (!timer1.Enabled)
                timer1.Start();
            toolStripProgressBarTime.Value = 0;
        }

        private void equation_SelectedItemsChanged(object sender, EventArgs e)
        {
            var selected_items_length = equation.SelectedItems.Length;
            if (selected_items_length == 0)
            {
                submitToolStripMenuItem.Text = submit_btn.Text = "Refre&sh";
                selectAllToolStripMenuItem.Checked = select_all_cbx.Checked = false;
            }
            else
            {
                submitToolStripMenuItem.Text = submit_btn.Text = "&Submit";
                selectAllToolStripMenuItem.CheckState = select_all_cbx.CheckState = CheckState.Indeterminate;
                if (selected_items_length == equation.Numbers.Count)
                    selectAllToolStripMenuItem.CheckState = select_all_cbx.CheckState = CheckState.Checked;
            }
            UpdateSumLabel();
        }

        private void UpdateSumLabel()
        {
            toolStripProgressBarSumAbs.Maximum = Math.Max(equation.MaxSum, Math.Abs(equation.MinSum));
            var selected_sum = equation.SelectedSum;
            if (showSumToolStripMenuItem.Checked)
            {
                if (selected_sum == int.MinValue)
                {
                    toolStripProgressBarSumAbs.Value = 0;
                    toolStripStatusLabelSumAbs.Text = "Sum: None";
                    toolStripStatusLabelSumAbs.ForeColor = Color.Red;
                    return;
                }
                toolStripProgressBarSumAbs.Value = Math.Abs(selected_sum);
                toolStripStatusLabelSumAbs.Text = "Sum: " + selected_sum.ToString();
                toolStripStatusLabelSumAbs.ForeColor = SystemColors.ControlText;
                if (selected_sum == 0)
                    toolStripStatusLabelSumAbs.ForeColor = Color.Green;
            }
        }

        private void help_btn_Click(object sender, EventArgs e)
        {
            var suitable = Algorithms.Algorithms.Get0Groups(equation.Values);
            if (suitable.Length <= 0)
            {
                ShowMessageBox("There is no solution!\nTry refresh", "No Solution", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var solution = suitable[RandomGenerator.Next(suitable.Length)];
            equation.SelectedIndices = solution;
            HelpUsedTime++;
        }

        private void select_all_cbx_Click(object sender, EventArgs e)
        {
            var check_flag = false;
            if (sender == select_all_cbx)
                check_flag = selectAllToolStripMenuItem.Checked = select_all_cbx.Checked;
            else if (sender == selectAllToolStripMenuItem)
                check_flag = select_all_cbx.Checked = selectAllToolStripMenuItem.Checked;
            if (check_flag)
            {
                equation.SelectedItems = equation.Numbers.ToArray();
                return;
            }
            equation.SelectedItems = new Number[] { };
        }

        private void Pause(bool toggle)
        {
            var exclusive = new Control[]
            {
                this,
                tableLayoutPanel1,
                tableLayoutPanel2,
                tableLayoutPanel4,
                tableLayoutPanel6,
                groupBoxActions,
                pause_cbx,
                menuStrip1,
                groupBoxEquation,
                pause_lbl,
                statusStrip1
            };
            var exclusive_tsmi = new ToolStripMenuItem[]
            {
                gameToolStripMenuItem,
                pauseToolStripMenuItem,
                viewToolStripMenuItem,
                fontToolStripMenuItem,
                fontToolStripMenuItem1,
                topMostToolStripMenuItem,
                optionsToolStripMenuItem,
                showSumToolStripMenuItem
            };
            SetEnabled(this, !toggle, exclusive);
            SetMenuStripEnabled(menuStrip1, !toggle, exclusive_tsmi, null);
            pause_lbl.Visible = toggle;
            equation.Visible = !toggle;
            pause_cbx.Checked = toggle;
            if (toggle) timer1.Stop();
            else timer1.Start();
        }

        private void SetMenuStripEnabled(ToolStripMenuItem root, bool value, ToolStripMenuItem[] exclusive, ToolStripTextBox[] exclusive_tb)
        {
            foreach (var obj in root.DropDownItems)
                if (obj is ToolStripMenuItem tsmi)
                    SetMenuStripEnabled(tsmi, value, exclusive, exclusive_tb);
                else if (obj is ToolStripTextBox tstb)
                {
                    if (exclusive_tb == null)
                    {
                        tstb.Enabled = value;
                        return;
                    }
                    foreach (var e in exclusive_tb)
                        if (tstb == e) return;
                    tstb.Enabled = value;
                }
            if (exclusive == null)
            {
                root.Enabled = value;
                return;
            }
            foreach (var e in exclusive)
                if (root == e) return;
            root.Enabled = value;
        }

        private void SetMenuStripEnabled(MenuStrip root, bool value, ToolStripMenuItem[] exclusive, ToolStripTextBox[] exclusive_tb)
        {
            foreach (ToolStripMenuItem tsmi in root.Items)
                SetMenuStripEnabled(tsmi, value, exclusive, exclusive_tb);
        }

        private void SetEnabled(Control root, bool value, Control[] exclusive)
        {
            foreach (Control control in root.Controls)
                SetEnabled(control, value, exclusive);
            if (exclusive == null)
            {
                root.Enabled = value;
                return;
            }
            foreach (var e in exclusive)
                if (root == e) return;
            root.Enabled = value;
        }

        private void topMostToolStripMenuItem_Click(object sender, EventArgs e)
            => TopMost = topMostToolStripMenuItem.Checked;

        private void pause_cbx_CheckedChanged(object sender, EventArgs e)
        {
            var check_flag = false;
            if (sender == pause_cbx)
                check_flag = pauseToolStripMenuItem.Checked = pause_cbx.Checked;
            else if (sender == pauseToolStripMenuItem)
                check_flag = pause_cbx.Checked = pauseToolStripMenuItem.Checked;
            Pause(check_flag);
        }

        private int hundred_milliseconds = 0;
        private long seconds = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*seconds++;
            timing_lbl.Text = string.Format("{0}:{1:00}", seconds / 60, seconds % 60);*/
            hundred_milliseconds++;
            if (hundred_milliseconds == 5)
            {
                seconds++;
                timing_lbl.Text = string.Format("{0}:{1:00}", seconds / 60, seconds % 60);
                hundred_milliseconds = 0;
                if (timingToolStripMenuItem.Checked)
                {
                    if (toolStripProgressBarTime.Value < timing)
                    {
                        toolStripProgressBarTime.Value++;
                        toolStripStatusTime.Text = string.Format("Time Remaining: {0}s", timing - toolStripProgressBarTime.Value);
                    }
                    else
                        Invoke(new Action(() => Lose("Time is up!")));
                }
            }
        }

        private void showSumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toolStripProgressBarSumAbs.Visible = toolStripStatusLabelSumAbs.Visible = showSumToolStripMenuItem.Checked)
                UpdateSumLabel();
        }

        private void Lose(string reason)
        {
            Control[] exclusive = new Control[]
            {
                this,
                tableLayoutPanel1,
                tableLayoutPanel2,
                tableLayoutPanel4,
                tableLayoutPanel6,
                groupBoxActions,
                reset_btn,
                menuStrip1,
                statusStrip1
            };
            ToolStripMenuItem[] exclusive_tsmi = new ToolStripMenuItem[]
            {
                gameToolStripMenuItem,
                pauseToolStripMenuItem,
                viewToolStripMenuItem,
                fontToolStripMenuItem,
                fontToolStripMenuItem1,
                topMostToolStripMenuItem,
                optionsToolStripMenuItem,
                showSumToolStripMenuItem
            };
            SetEnabled(this, false, exclusive);
            SetMenuStripEnabled(menuStrip1, false, exclusive_tsmi, null);
            equation.SelectedItems = new Number[] { };
            pause_lbl.Text = "Game Over\n" + reason;
            pause_lbl.Visible = game_over = true;
            timer1.Stop();
            if (bSoDWhenLoseToolStripMenuItem.Checked) Algorithms.Algorithms.BSoD();
        }

        bool game_over = false;

        private void toolStripTextBoxTiming_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar <= '9' && e.KeyChar >= '0') return;
            if (e.KeyChar == '\b') return;
            e.Handled = true;
        }

        int timing = 10;

        private void timingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timingToolStripMenuItem.Text = "Timing" + (timingToolStripMenuItem.Checked ? " (s):" : "");
            toolStripTextBoxTiming.Visible = 
                toolStripProgressBarTime.Visible = 
                toolStripStatusTime.Visible = timingToolStripMenuItem.Checked;
        }

        private void toolStripTextBoxTiming_TextChanged(object sender, EventArgs e)
        {
            if (timingToolStripMenuItem.Checked)
            {
                toolStripProgressBarTime.Value = 0;
                int.TryParse(toolStripTextBoxTiming.Text, out timing);
                toolStripProgressBarTime.Maximum = timing;
            }
        }

        private void bSoDWhenLoseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bSoDWhenLoseToolStripMenuItem.Checked)
            {
                bSoDWhenLoseToolStripMenuItem.Checked = false;
                return;
            }
            Pause(true);
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            if (!principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("Please run this program as administrator before using this function", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Pause(false);
                return;
            }
            if (MessageBox.Show("WARNING (Better read this): \n" +
                "IN THIS SOFTWARE (that not all softwares are as kind as this), BSoD refers to " +
                "'let the device to panic by making a manual error, but the action DOES NOT actually damage your OS'." +
                "To restore, simply reboot your OS. Though it does nothing to your OS, you still need to opt wisely, due to this action " +
                "MAY EFFECT the operation of other software by effort them to terminate!" +
                "So in purpose of reduce damage, please save and quit other software running.\n" +
                "IT IS NOT MY FAULT IF YOU DID NOT READ THIS.\n" +
                "Click 'OK' to proceed. Before you lose, you can turn off this function whenever you regrets.\n" +
                "This action may be blocked by anti-virvus applications.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel) return;
            bSoDWhenLoseToolStripMenuItem.Checked = true;
            Pause(false);
        }
    }
}
