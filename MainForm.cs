using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
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

        public static readonly string GameName = "Number Shuffle";

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
            Text = GameName;
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
            void InitSelectedNumbers()
            {
                foreach (var n in equation.SelectedItems)
                {
                    n.InitNumber();
                    n.Selected = false;
                }
                toolStripProgressBarTime.Value = 0;
            }
            string ConnectNumbers(Number[] numbers)
                => string.Concat(numbers.Select((l, i) => i == 0 ? l.Text : "+" + l.Text));
            if (equation.SelectedItems.Count() <= 0)
            {
                var suitable = Algorithms.Algorithms.Get0Groups(equation.Values.Select(v => v.Value).ToArray());
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
            var checkresult = equation.CheckedStatus;
            if (checkresult.ContainsNull)
            {
                equation.InitNumbers();
                equation.SelectedIndices = new int[] { };
                return;
            }
            if ((bool)checkresult)
            {
                InitSelectedNumbers();
                if (checkresult.ContainsDouble)
                {
                    Score += 2;
                    return;
                }
                Score++;
                return;
            }
            if (checkresult.ContainsUnknown)
            {
                InitSelectedNumbers();
                if (checkresult.ContainsDouble)
                {
                    Score -= 2;
                    return;
                }
                Score--;
                return;
            }
            if (equation.UncommonNumberSelected)
            {
                Lose(string.Format("{0} ≠ 0",
                    ConnectNumbers(equation.SelectedItems)));
                return;
            }
            Lose(string.Format("{0} = {1} ≠ 0",
                ConnectNumbers(equation.SelectedItems),
                equation.SelectedSum));
        }

        private DialogResult ShowMessageBox(string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
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
            if (showSumToolStripMenuItem.Checked)
            {
                toolStripProgressBarSumAbs.Maximum = Math.Max(equation.MaxSum, Math.Abs(equation.MinSum));
                var selected_sum = equation.SelectedSum;
                // TODO: Rewrite progressbar code
                if (equation.UncommonNumberSelected)
                {
                    toolStripProgressBarSumAbs.Value = 0;
                    toolStripStatusLabelSumAbs.Text = "Sum: NaN";
                    toolStripStatusLabelSumAbs.ForeColor = Color.Red;
                    return;
                }
                if (equation.IsSelected(Classes.NumberValue.NumberType.Infinitive))
                {
                    toolStripProgressBarSumAbs.Value = 0;
                    toolStripStatusLabelSumAbs.Text = "Sum: ∞";
                    toolStripStatusLabelSumAbs.ForeColor = Color.Green;
                    return;
                }
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
            var suitable = Algorithms.Algorithms.Get0Groups(equation.Values.Select(v => v.Value).ToArray());
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
                showSumToolStripMenuItem,
                aboutToolStripMenuItem,
                howtoplayToolStripMenuItem1,
                authorToolStripMenuItem
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
            var paused = pause_cbx.Checked;
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
                Pause(paused);
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
            Pause(paused);
        }

        private void extendedFeaturesToggleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paused = pause_cbx.Checked;
            Pause(true);
            if (extendedFeaturesToggleToolStripMenuItem.Checked)
                MessageBox.Show("By checking this option, the game will enable the Extended Features," +
                    "that offers more numbers to be adding into the game.\n" +
                    "To Apply the change, you may need to reset the game by clicking the 'Reset' button in the Actions Bar",
                    "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            equation.ExtendedFeaturesToggle = extendedFeaturesToggleToolStripMenuItem.Checked;
            Pause(paused);
        }

        private void howtoplayToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var paused = pause_cbx.Checked;
            Pause(true);
            MessageBox.Show("The rules are very simple. When the game lunches, there would be an equation of 7 numbers adding together. You would need to select numbers that added up to 0. For example, a simple 0 would be OK, or numbers like 1, -1, or even 1, 5, 3, 5, -7 -7. After this, the button that originally shows “Refresh” will turn to “Submit. You may click the button to submit the numbers. As long as the sum is 0, you will get 1 point, and the numbers will regenerate. \r\nBut if the sum is not 0, then you will lose the game. ",
                "How To Play");
            MessageBox.Show("When you think none of the numbers could added up to 0, you may deselect all numbers and then click “Refresh” to refresh the equation. The game will calculate if there is really no numbers could be selected. If yes, then the numbers will be refresh. If no, you will lose the game.",
                "How To Play");
            MessageBox.Show("If you want to restart the game, you may click the “Reset”button. And if you want to gain help, you may click the “Help” button, and the button will help you to find the numbers with sum of 0. There is a check box with label “Select All” for you to select or deselect all numbers instantly. To pause the game, you may check the check box with label “Pause”. Also, a timer will appear at the right corner. You score and used time of help appears at the upper of the timer.",
                "How To Play");
            MessageBox.Show("There are several options for better gaming experience. You can config the size of the font, or let the game to run at the very top of desktop in the menu strip labeled“View” at the top. You can let the game to show a small progress bar and display the sum of numbers by checking the “Show Sum” option in the “Option” menu strip at top. If you want the game to limit the time you submit, you can check the “Timing” option in the “Option” menu strip at top. For Windows users, you can check the “BSoD When Lose” option for more exciting gaming experiences. A message will pop up to tell you the detail of this option if you check it.",
                "How To Play");
            if (extendedFeaturesToggleToolStripMenuItem.Checked)
            {
                MessageBox.Show("Since you have enabled the “Extended Features Toggle”, here are the rules for the extended features. The game imported several special numbers into the game after the option was enabled.",
                    "How To Play");
                MessageBox.Show("- The Infinitive (∞) can literally be selected with any other numbers.\r\n- The Signum (±) can represent both positive or negative of the value. For example, (±5) can be selected with 5 to form a 0. Multiple Signums can be selected together.",
                    "How To Play");
                MessageBox.Show("- The Unknown (x) can hold one time of losing. For example, you selected 5, -1, and x. But -1 + 5 does not equals to 0. In regular case, you will lose the game. But with the Unknown, the game will only take you 1 points from the score instead of losing. The Unknown works once in one submit.",
                    "How To Play");
                MessageBox.Show("- The Double ([×2]) lets the score you gain to be double. For example, you are submitting 1, -1, [×2] results in two points of score. The Double could work with the Unknown, that you will lose two points instead of losing.\r\n- The Null (Null) helps you to refresh all the numbers without resetting the game.",
                    "How To Play");
            }
            else
                MessageBox.Show("For extended features, you may check the “Extended Features Toggle”. For more information about this option, you may use this help again after checking this option.",
                "How To Play");
            Pause(paused);
        }

        private void authorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paused = pause_cbx.Checked;
            Pause(true);
            MessageBox.Show(
                GameName + "\n" +
                "(C)opyright 2024 Idad Wind.\n" +
                "All rights reserved.\n" +
                "Click 'Help' to open GitHub page", "About",
                0, MessageBoxIcon.Information, 0, 0,
                "https://github.com/WillamSun/NumberGamePlus");
            Pause(paused);
        }
    }
}
