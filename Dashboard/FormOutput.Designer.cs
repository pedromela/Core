
namespace Dashboard
{
    partial class FormOutput
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.outputTextbox = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // outputTextbox
            // 
            this.outputTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTextbox.Location = new System.Drawing.Point(0, 0);
            this.outputTextbox.Name = "outputTextbox";
            this.outputTextbox.Size = new System.Drawing.Size(717, 438);
            this.outputTextbox.TabIndex = 0;
            this.outputTextbox.Text = "";
            this.outputTextbox.TextChanged += new System.EventHandler(this.outputTextbox_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 600000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 438);
            this.Controls.Add(this.outputTextbox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOutput";
            this.Text = "FormOutput";
            this.Load += new System.EventHandler(this.FormOutput_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox outputTextbox;
        private System.Windows.Forms.Timer timer1;
    }
}