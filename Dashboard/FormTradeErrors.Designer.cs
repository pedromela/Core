
namespace Dashboard
{
    partial class FormTradeErrors
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
            this.dataGridViewTradeErrors = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTradeErrors)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewTradeErrors
            // 
            this.dataGridViewTradeErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTradeErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTradeErrors.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewTradeErrors.Name = "dataGridViewTradeErrors";
            this.dataGridViewTradeErrors.RowTemplate.Height = 25;
            this.dataGridViewTradeErrors.Size = new System.Drawing.Size(717, 438);
            this.dataGridViewTradeErrors.TabIndex = 0;
            // 
            // FormTradeErrors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 438);
            this.Controls.Add(this.dataGridViewTradeErrors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormTradeErrors";
            this.Text = "FormTradeErrors";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTradeErrors)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewTradeErrors;
    }
}