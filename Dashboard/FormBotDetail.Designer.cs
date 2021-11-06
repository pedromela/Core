
namespace Dashboard
{
    partial class FormBotDetail
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
            this.btnBacktest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBacktest
            // 
            this.btnBacktest.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnBacktest.FlatAppearance.BorderSize = 0;
            this.btnBacktest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBacktest.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnBacktest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.btnBacktest.Location = new System.Drawing.Point(0, 376);
            this.btnBacktest.Name = "btnBacktest";
            this.btnBacktest.Size = new System.Drawing.Size(701, 23);
            this.btnBacktest.TabIndex = 6;
            this.btnBacktest.Text = "Backtest";
            this.btnBacktest.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnBacktest.UseVisualStyleBackColor = true;
            this.btnBacktest.Click += new System.EventHandler(this.btnBacktest_Click);
            // 
            // FormBotDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(701, 399);
            this.Controls.Add(this.btnBacktest);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormBotDetail";
            this.Text = "FormBotDetail";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBacktest;
    }
}