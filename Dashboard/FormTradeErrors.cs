using BrokerLib.Models;
using System.Collections.Generic;
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
