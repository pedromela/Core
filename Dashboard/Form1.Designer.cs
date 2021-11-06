namespace Dashboard
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlNav = new System.Windows.Forms.Panel();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnTradeErrors = new System.Windows.Forms.Button();
            this.btnTransactionErrors = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnBotRanking = new System.Windows.Forms.Button();
            this.btnUserList = new System.Windows.Forms.Button();
            this.btnBotList = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pnlFormLoader = new System.Windows.Forms.Panel();
            this.loadingBar = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.pnlFormLoader.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(30)))), ((int)(((byte)(54)))));
            this.panel1.Controls.Add(this.pnlNav);
            this.panel1.Controls.Add(this.btnOutput);
            this.panel1.Controls.Add(this.btnTradeErrors);
            this.panel1.Controls.Add(this.btnTransactionErrors);
            this.panel1.Controls.Add(this.btnSettings);
            this.panel1.Controls.Add(this.btnBotRanking);
            this.panel1.Controls.Add(this.btnUserList);
            this.panel1.Controls.Add(this.btnBotList);
            this.panel1.Controls.Add(this.btnDashboard);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 580);
            this.panel1.TabIndex = 0;
            // 
            // pnlNav
            // 
            this.pnlNav.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.pnlNav.Location = new System.Drawing.Point(0, 193);
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Size = new System.Drawing.Size(3, 100);
            this.pnlNav.TabIndex = 8;
            // 
            // btnOutput
            // 
            this.btnOutput.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOutput.FlatAppearance.BorderSize = 0;
            this.btnOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOutput.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnOutput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.btnOutput.Location = new System.Drawing.Point(0, 282);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(200, 23);
            this.btnOutput.TabIndex = 9;
            this.btnOutput.Text = "Output";
            this.btnOutput.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            this.btnOutput.Leave += new System.EventHandler(this.btnOutput_Leave);
            // 
            // btnTradeErrors
            // 
            this.btnTradeErrors.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTradeErrors.FlatAppearance.BorderSize = 0;
            this.btnTradeErrors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTradeErrors.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnTradeErrors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.btnTradeErrors.Location = new System.Drawing.Point(0, 259);
            this.btnTradeErrors.Name = "btnTradeErrors";
            this.btnTradeErrors.Size = new System.Drawing.Size(200, 23);
            this.btnTradeErrors.TabIndex = 7;
            this.btnTradeErrors.Text = "Trade Errors";
            this.btnTradeErrors.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnTradeErrors.UseVisualStyleBackColor = true;
            this.btnTradeErrors.Click += new System.EventHandler(this.btnTradeErrors_Click);
            this.btnTradeErrors.Leave += new System.EventHandler(this.btnTradeErrors_Leave);
            // 
            // btnTransactionErrors
            // 
            this.btnTransactionErrors.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTransactionErrors.FlatAppearance.BorderSize = 0;
            this.btnTransactionErrors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTransactionErrors.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnTransactionErrors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.btnTransactionErrors.Location = new System.Drawing.Point(0, 236);
            this.btnTransactionErrors.Name = "btnTransactionErrors";
            this.btnTransactionErrors.Size = new System.Drawing.Size(200, 23);
            this.btnTransactionErrors.TabIndex = 6;
            this.btnTransactionErrors.Text = "Transaction Errors";
            this.btnTransactionErrors.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnTransactionErrors.UseVisualStyleBackColor = true;
            this.btnTransactionErrors.Click += new System.EventHandler(this.btnTransactionErrors_Click);
            this.btnTransactionErrors.Leave += new System.EventHandler(this.btnTransactionErrors_Leave);
            // 
            // btnSettings
            // 
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.btnSettings.Location = new System.Drawing.Point(0, 557);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(200, 23);
            this.btnSettings.TabIndex = 5;
            this.btnSettings.Text = "Settings";
            this.btnSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            this.btnSettings.Leave += new System.EventHandler(this.btnSettings_Leave);
            // 
            // btnBotRanking
            // 
            this.btnBotRanking.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnBotRanking.FlatAppearance.BorderSize = 0;
            this.btnBotRanking.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBotRanking.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnBotRanking.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.btnBotRanking.Location = new System.Drawing.Point(0, 213);
            this.btnBotRanking.Name = "btnBotRanking";
            this.btnBotRanking.Size = new System.Drawing.Size(200, 23);
            this.btnBotRanking.TabIndex = 4;
            this.btnBotRanking.Text = "Bot Ranking";
            this.btnBotRanking.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnBotRanking.UseVisualStyleBackColor = true;
            this.btnBotRanking.Click += new System.EventHandler(this.btnBotRanking_Click);
            this.btnBotRanking.Leave += new System.EventHandler(this.btnBotRanking_Leave);
            // 
            // btnUserList
            // 
            this.btnUserList.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUserList.FlatAppearance.BorderSize = 0;
            this.btnUserList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUserList.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnUserList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.btnUserList.Location = new System.Drawing.Point(0, 190);
            this.btnUserList.Name = "btnUserList";
            this.btnUserList.Size = new System.Drawing.Size(200, 23);
            this.btnUserList.TabIndex = 3;
            this.btnUserList.Text = "User List";
            this.btnUserList.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnUserList.UseVisualStyleBackColor = true;
            this.btnUserList.Click += new System.EventHandler(this.btnUserList_Click);
            this.btnUserList.Leave += new System.EventHandler(this.btnUserList_Leave);
            // 
            // btnBotList
            // 
            this.btnBotList.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnBotList.FlatAppearance.BorderSize = 0;
            this.btnBotList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBotList.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnBotList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.btnBotList.Location = new System.Drawing.Point(0, 167);
            this.btnBotList.Name = "btnBotList";
            this.btnBotList.Size = new System.Drawing.Size(200, 23);
            this.btnBotList.TabIndex = 2;
            this.btnBotList.Text = "Bot List";
            this.btnBotList.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBotList.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnBotList.UseVisualStyleBackColor = true;
            this.btnBotList.Click += new System.EventHandler(this.btnBotList_Click);
            this.btnBotList.Leave += new System.EventHandler(this.btnBotList_Leave);
            // 
            // btnDashboard
            // 
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.btnDashboard.Location = new System.Drawing.Point(0, 144);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(200, 23);
            this.btnDashboard.TabIndex = 1;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnDashboard.UseVisualStyleBackColor = true;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            this.btnDashboard.Leave += new System.EventHandler(this.btnDashboard_Leave);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 144);
            this.panel2.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Dashboard.Properties.Resources.shark_logo;
            this.pictureBox1.Location = new System.Drawing.Point(60, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(63, 63);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(161)))), ((int)(((byte)(176)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 17);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(0, 32);
            this.lblTitle.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.pnlFormLoader);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.searchBox);
            this.panel3.Controls.Add(this.lblTitle);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(200, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(760, 580);
            this.panel3.TabIndex = 2;
            // 
            // pnlFormLoader
            // 
            this.pnlFormLoader.Controls.Add(this.loadingBar);
            this.pnlFormLoader.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFormLoader.Location = new System.Drawing.Point(0, 103);
            this.pnlFormLoader.Name = "pnlFormLoader";
            this.pnlFormLoader.Size = new System.Drawing.Size(760, 477);
            this.pnlFormLoader.TabIndex = 4;
            // 
            // loadingBar
            // 
            this.loadingBar.Location = new System.Drawing.Point(18, 198);
            this.loadingBar.Name = "loadingBar";
            this.loadingBar.Size = new System.Drawing.Size(730, 62);
            this.loadingBar.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(723, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 25);
            this.button1.TabIndex = 3;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // searchBox
            // 
            this.searchBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(79)))), ((int)(((byte)(99)))));
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchBox.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.searchBox.Location = new System.Drawing.Point(387, 25);
            this.searchBox.Multiline = true;
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(292, 20);
            this.searchBox.TabIndex = 2;
            this.searchBox.Text = "Search for some thing...";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(960, 580);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.pnlFormLoader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Panel pnlNav;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnBotRanking;
        private System.Windows.Forms.Button btnUserList;
        private System.Windows.Forms.Button btnBotList;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pnlFormLoader;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.ProgressBar loadingBar;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnTransactionErrors;
        private System.Windows.Forms.Button btnTradeErrors;
        private System.Windows.Forms.Button btnOutput;
    }
}

