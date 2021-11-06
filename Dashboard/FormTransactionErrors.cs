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
    public partial class FormTransactionErrors : Form
    {
        public FormTransactionErrors(IEnumerable<Transaction> transactionErrors)
        {
            InitializeComponent();
            dataGridViewTransactionErrors.DataSource = transactionErrors;
        }
    }
}
