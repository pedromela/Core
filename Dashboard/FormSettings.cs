using System;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
            checkBoxVerbose.Checked = UtilsLib.UtilsLib.Verbose;
            checkBoxLogging.Checked = UtilsLib.UtilsLib.Logging;
        }

        private void checkBoxVerbose_CheckedChanged(object sender, EventArgs e)
        {
            UtilsLib.UtilsLib.Verbose = checkBoxVerbose.Checked;
            SignalsEngine.SignalsEngine.Verbose = checkBoxVerbose.Checked;
            BrokerLib.BrokerLib.Verbose = checkBoxVerbose.Checked;
            BotEngine.BotEngine.Verbose = checkBoxVerbose.Checked;
            BotLib.BotLib.Verbose = checkBoxVerbose.Checked;
        }

        private void checkBoxLogging_CheckedChanged(object sender, EventArgs e)
        {
            UtilsLib.UtilsLib.Logging = checkBoxLogging.Checked;
            SignalsEngine.SignalsEngine.Logging = checkBoxLogging.Checked;
            BrokerLib.BrokerLib.Logging = checkBoxLogging.Checked;
            BotEngine.BotEngine.Logging = checkBoxLogging.Checked;
            BotLib.BotLib.Logging = checkBoxLogging.Checked;
        }
    }
}
