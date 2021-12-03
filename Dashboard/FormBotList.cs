using BotEngine.Bot;
using BotLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class FormBotList : Form
    {
        public FormBotList(IEnumerable<BotParameters> botParameters)
        {
            InitializeComponent();
            dataGridViewBotList.DataSource = botParameters;
        }

        private void dataGridViewBotList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Controls.Clear();
            FormBotDetail formBotDetail = new FormBotDetail() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formBotDetail.FormBorderStyle = FormBorderStyle.None;
            Controls.Add(formBotDetail);
            formBotDetail.Show();
        }
    }
}
