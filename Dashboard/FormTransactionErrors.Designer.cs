﻿
namespace Dashboard
{
    partial class FormTransactionErrors
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
            this.dataGridViewTransactionErrors = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTransactionErrors)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewTransactionErrors
            // 
            this.dataGridViewTransactionErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTransactionErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTransactionErrors.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewTransactionErrors.Name = "dataGridViewTransactionErrors";
            this.dataGridViewTransactionErrors.RowTemplate.Height = 25;
            this.dataGridViewTransactionErrors.Size = new System.Drawing.Size(701, 399);
            this.dataGridViewTransactionErrors.TabIndex = 0;
            // 
            // FormTransactionErrors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 399);
            this.Controls.Add(this.dataGridViewTransactionErrors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormTransactionErrors";
            this.Text = "FormTransactionErrors";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTransactionErrors)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewTransactionErrors;
    }
}