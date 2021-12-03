using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Dashboard
{
    public class TextBoxWriter : TextWriter
    {
        private Control myControl;
        public TextBoxWriter(Control control) 
        {
            myControl = control;
        }

        public override void Write(char value)
        {
            myControl.Invoke(new Action(() => myControl.Text += value));
        }

        public override void Write(string value)
        {
            myControl.Invoke(new Action(() => myControl.Text += value));
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}
