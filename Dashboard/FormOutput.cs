using System;
using System.Threading;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class FormOutput : Form
    {
        TypeAssistant assistant;

        public FormOutput()
        {
            InitializeComponent();
            assistant = new TypeAssistant();
            assistant.Idled += assistant_Idled;
            timer1.Start();
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

        void assistant_Idled(object sender, EventArgs e)
        {
            this.Invoke(
            new MethodInvoker(() =>
            {
                outputTextbox.SelectionStart = outputTextbox.Text.Length;
                // scroll it automatically
                outputTextbox.ScrollToCaret();
                // do your job here
            }));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            outputTextbox.Clear();
        }

        private void outputTextbox_TextChanged(object sender, EventArgs e)
        {
            assistant.TextChanged();
        }
    }
}
