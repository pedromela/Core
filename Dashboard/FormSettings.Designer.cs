
namespace Dashboard
{
    partial class FormSettings
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
            this.checkBoxLogging = new System.Windows.Forms.CheckBox();
            this.checkBoxVerbose = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBoxLogging
            // 
            this.checkBoxLogging.AutoSize = true;
            this.checkBoxLogging.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.checkBoxLogging.Location = new System.Drawing.Point(62, 77);
            this.checkBoxLogging.Name = "checkBoxLogging";
            this.checkBoxLogging.Size = new System.Drawing.Size(70, 19);
            this.checkBoxLogging.TabIndex = 0;
            this.checkBoxLogging.Text = "Logging";
            this.checkBoxLogging.UseVisualStyleBackColor = true;
            this.checkBoxLogging.CheckedChanged += new System.EventHandler(this.checkBoxLogging_CheckedChanged);
            // 
            // checkBoxVerbose
            // 
            this.checkBoxVerbose.AutoSize = true;
            this.checkBoxVerbose.Checked = true;
            this.checkBoxVerbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVerbose.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.checkBoxVerbose.Location = new System.Drawing.Point(62, 113);
            this.checkBoxVerbose.Name = "checkBoxVerbose";
            this.checkBoxVerbose.Size = new System.Drawing.Size(67, 19);
            this.checkBoxVerbose.TabIndex = 1;
            this.checkBoxVerbose.Text = "Verbose";
            this.checkBoxVerbose.UseVisualStyleBackColor = true;
            this.checkBoxVerbose.CheckedChanged += new System.EventHandler(this.checkBoxVerbose_CheckedChanged);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.checkBoxVerbose);
            this.Controls.Add(this.checkBoxLogging);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSettings";
            this.Text = "FormSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxLogging;
        private System.Windows.Forms.CheckBox checkBoxVerbose;
    }
}