using BrokerLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class FormTradeErrors : Form
    {
        public FormTradeErrors(IEnumerable<Trade> tradeErrors)
        {
            InitializeComponent();
            dataGridViewTradeErrors.DataSource = tradeErrors;
        }
    }
}
