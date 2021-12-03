
namespace Dashboard
{
    partial class FormBotList
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
            this.dataGridViewBotList = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBotList)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewBotList
            // 
            this.dataGridViewBotList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBotList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewBotList.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewBotList.Name = "dataGridViewBotList";
            this.dataGridViewBotList.RowTemplate.Height = 25;
            this.dataGridViewBotList.Size = new System.Drawing.Size(717, 438);
            this.dataGridViewBotList.TabIndex = 0;
            this.dataGridViewBotList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dataGridViewBotList_MouseDoubleClick);
            // 
            // FormBotList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(717, 438);
            this.Controls.Add(this.dataGridViewBotList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormBotList";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "FormBotList";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBotList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewBotList;
    }
}