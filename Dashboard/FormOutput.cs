using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class FormOutput : Form
    {
        public FormOutput()
        {
            InitializeComponent();
        }

        private void FormOutput_Load(object sender, EventArgs e)
        {
            var threadParameters = new ThreadStart(delegate { Loading(); });
            var thread2 = new Thread(threadParameters);
            thread2.Start();
        }

        private void Loading() 
        {
            if (outputTextbox.InvokeRequired)
            {
                Action safeLoad = delegate { Loading(); };
                outputTextbox.Invoke(safeLoad);
            }
            else
            {
                TextBoxWriter writer = new TextBoxWriter(outputTextbox);
                Console.SetOut(writer);
            }
        }
    }
}
