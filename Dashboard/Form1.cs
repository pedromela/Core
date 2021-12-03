using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reactive.Linq;
using System.Threading;

namespace Dashboard
{
    public partial class Form1 : Form
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        BotEngine.BotEngine _botEngine;

        public Form1()
        {
            UtilsLib.UtilsLib.Logging = false;
            SignalsEngine.SignalsEngine.Logging = false;
            BrokerLib.BrokerLib.Logging = false;
            BotEngine.BotEngine.Logging = false;
            BotLib.BotLib.Logging = false;

            UtilsLib.UtilsLib.Verbose = true;
            SignalsEngine.SignalsEngine.Verbose = true;
            BrokerLib.BrokerLib.Verbose = true;
            BotEngine.BotEngine.Verbose = true;
            BotLib.BotLib.Verbose = true;

            InitializeComponent();
            loadingBar.Visible = true;
            loadingBar.Maximum = 100;
            loadingBar.Value = 0;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            pnlNav.Height = btnDashboard.Height;
            pnlNav.Top = btnDashboard.Top;
            pnlNav.Left = btnDashboard.Left;
            btnDashboard.BackColor = Color.FromArgb(46, 51, 73);

            _botEngine = new BotEngine.BotEngine(true);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            var threadParameters = new ThreadStart(delegate { Loading(); });
            var thread2 = new Thread(threadParameters);
            thread2.Start();
            //if (!backgroundWorker1.IsBusy)
            //    backgroundWorker1.RunWorkerAsync();
        }

        private void Loading()
        {
            if (loadingBar.InvokeRequired)
            {
                Action safeLoad = delegate { Loading(); };
                loadingBar.Invoke(safeLoad);
            }
            else
            {
                IDisposable sub = _botEngine.Init().Subscribe((loading) =>
                {
                    loadingBar.Value = loading;
                    if (loading == 100)
                    {
                        _botEngine.UpdateCycle();
                        _botEngine.Evolutions();
                        _botEngine.Started = true;
                        loadingBar.Visible = false;
                        lblTitle.Text = "Dashboard";
                        pnlFormLoader.Controls.Clear();
                        FormDashboard formDashboard = new FormDashboard() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
                        formDashboard.FormBorderStyle = FormBorderStyle.None;
                        pnlFormLoader.Controls.Add(formDashboard);
                        formDashboard.Show();
                    }
                });
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnDashboard.Height;
            pnlNav.Top = btnDashboard.Top;
            pnlNav.Left = btnDashboard.Left;
            btnDashboard.BackColor = Color.FromArgb(46, 51, 73);

            lblTitle.Text = "Dashboard";
            pnlFormLoader.Controls.Clear();
            FormDashboard formDashboard = new FormDashboard() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formDashboard.FormBorderStyle = FormBorderStyle.None;
            pnlFormLoader.Controls.Add(formDashboard);
            formDashboard.Show();
        }

        private void btnBotList_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnBotList.Height;
            pnlNav.Top = btnBotList.Top;
            pnlNav.Left = btnBotList.Left;
            btnBotList.BackColor = Color.FromArgb(46, 51, 73);

            lblTitle.Text = "Bot List";
            pnlFormLoader.Controls.Clear();
            FormBotList formBotList = new FormBotList(_botEngine.GetBotParametersList()) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formBotList.FormBorderStyle = FormBorderStyle.None;
            pnlFormLoader.Controls.Add(formBotList);
            formBotList.Show();
        }

        private void btnUserList_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnUserList.Height;
            pnlNav.Top = btnUserList.Top;
            pnlNav.Left = btnUserList.Left;
            btnUserList.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void btnBotRanking_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnBotRanking.Height;
            pnlNav.Top = btnBotRanking.Top;
            pnlNav.Left = btnBotRanking.Left;
            btnBotRanking.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnSettings.Height;
            pnlNav.Top = btnSettings.Top;
            pnlNav.Left = btnSettings.Left;
            btnSettings.BackColor = Color.FromArgb(46, 51, 73);

            lblTitle.Text = "Settings";
            pnlFormLoader.Controls.Clear();
            FormSettings formSettings = new FormSettings() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formSettings.FormBorderStyle = FormBorderStyle.None;
            pnlFormLoader.Controls.Add(formSettings);
            formSettings.Show();
        }

        private void btnTransactionErrors_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnTransactionErrors.Height;
            pnlNav.Top = btnTransactionErrors.Top;
            pnlNav.Left = btnTransactionErrors.Left;
            btnTransactionErrors.BackColor = Color.FromArgb(46, 51, 73);

            lblTitle.Text = "Transaction errors";
            pnlFormLoader.Controls.Clear();
            FormTransactionErrors formTransactionErrors = new FormTransactionErrors(_botEngine.GetTransactionErrors()) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formTransactionErrors.FormBorderStyle = FormBorderStyle.None;
            pnlFormLoader.Controls.Add(formTransactionErrors);
            formTransactionErrors.Show();
        }

        private void btnTradeErrors_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnTradeErrors.Height;
            pnlNav.Top = btnTradeErrors.Top;
            pnlNav.Left = btnTradeErrors.Left;
            btnTradeErrors.BackColor = Color.FromArgb(46, 51, 73);

            lblTitle.Text = "Trade errors";
            pnlFormLoader.Controls.Clear();
            FormTradeErrors formTradeErrors = new FormTradeErrors(_botEngine.GetTradeErrors()) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formTradeErrors.FormBorderStyle = FormBorderStyle.None;
            pnlFormLoader.Controls.Add(formTradeErrors);
            formTradeErrors.Show();
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnOutput.Height;
            pnlNav.Top = btnOutput.Top;
            pnlNav.Left = btnOutput.Left;
            btnOutput.BackColor = Color.FromArgb(46, 51, 73);

            lblTitle.Text = "Output";
            pnlFormLoader.Controls.Clear();
            FormOutput formOutput = new FormOutput() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formOutput.FormBorderStyle = FormBorderStyle.None;
            pnlFormLoader.Controls.Add(formOutput);
            formOutput.Show();
        }

        private void btnDashboard_Leave(object sender, EventArgs e)
        {
            btnDashboard.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnBotList_Leave(object sender, EventArgs e)
        {
            btnBotList.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnUserList_Leave(object sender, EventArgs e)
        {
            btnUserList.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnBotRanking_Leave(object sender, EventArgs e)
        {
            btnBotRanking.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnSettings_Leave(object sender, EventArgs e)
        {
            btnSettings.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnTransactionErrors_Leave(object sender, EventArgs e)
        {
            btnTransactionErrors.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnTradeErrors_Leave(object sender, EventArgs e)
        {
            btnTradeErrors.BackColor = Color.FromArgb(24, 30, 54);
        }
        private void btnOutput_Leave(object sender, EventArgs e)
        {
            btnOutput.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Loading();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
