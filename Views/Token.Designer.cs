namespace Telinha
{
    partial class Token
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
            TokenTMDBBox = new TextBox();
            TokenDEEPLBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            PanelTopTitle = new Panel();
            PanelTopBar = new Panel();
            PanelBottom = new Panel();
            SalvarButton = new Button();
            SairButton = new Button();
            SuspendLayout();
            // 
            // TokenTMDBBox
            // 
            TokenTMDBBox.Location = new Point(128, 111);
            TokenTMDBBox.Name = "TokenTMDBBox";
            TokenTMDBBox.PasswordChar = '*';
            TokenTMDBBox.PlaceholderText = "Token TMDB";
            TokenTMDBBox.Size = new Size(754, 23);
            TokenTMDBBox.TabIndex = 0;
            // 
            // TokenDEEPLBox
            // 
            TokenDEEPLBox.Location = new Point(128, 163);
            TokenDEEPLBox.Name = "TokenDEEPLBox";
            TokenDEEPLBox.PasswordChar = '*';
            TokenDEEPLBox.PlaceholderText = "Token DEEPL";
            TokenDEEPLBox.Size = new Size(754, 23);
            TokenDEEPLBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(47, 114);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 2;
            label1.Text = "Token TMDB";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(47, 166);
            label2.Name = "label2";
            label2.Size = new Size(75, 15);
            label2.TabIndex = 3;
            label2.Text = "Token DEEPL";
            // 
            // PanelTopTitle
            // 
            PanelTopTitle.BackColor = Color.FromArgb(5, 74, 105);
            PanelTopTitle.Dock = DockStyle.Top;
            PanelTopTitle.Location = new Point(0, 29);
            PanelTopTitle.Name = "PanelTopTitle";
            PanelTopTitle.Size = new Size(929, 58);
            PanelTopTitle.TabIndex = 5;
            // 
            // PanelTopBar
            // 
            PanelTopBar.BackColor = Color.FromArgb(4, 52, 72);
            PanelTopBar.Dock = DockStyle.Top;
            PanelTopBar.Location = new Point(0, 0);
            PanelTopBar.Name = "PanelTopBar";
            PanelTopBar.Size = new Size(929, 29);
            PanelTopBar.TabIndex = 4;
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = Color.FromArgb(4, 52, 72);
            PanelBottom.Dock = DockStyle.Bottom;
            PanelBottom.Location = new Point(0, 320);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(929, 44);
            PanelBottom.TabIndex = 6;
            // 
            // SalvarButton
            // 
            SalvarButton.Location = new Point(257, 239);
            SalvarButton.Name = "SalvarButton";
            SalvarButton.Size = new Size(180, 41);
            SalvarButton.TabIndex = 7;
            SalvarButton.Text = "Salvar ";
            SalvarButton.UseVisualStyleBackColor = true;
            // 
            // SairButton
            // 
            SairButton.Location = new Point(493, 239);
            SairButton.Name = "SairButton";
            SairButton.Size = new Size(180, 41);
            SairButton.TabIndex = 8;
            SairButton.Text = "Sair";
            SairButton.UseVisualStyleBackColor = true;
            // 
            // Token
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(929, 364);
            Controls.Add(SairButton);
            Controls.Add(SalvarButton);
            Controls.Add(PanelBottom);
            Controls.Add(PanelTopTitle);
            Controls.Add(PanelTopBar);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(TokenDEEPLBox);
            Controls.Add(TokenTMDBBox);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Token";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TokenTMDBBox;
        private TextBox TokenDEEPLBox;
        private Label label1;
        private Label label2;
        private Panel PanelTopTitle;
        private Panel PanelTopBar;
        private Panel PanelBottom;
        private Button SalvarButton;
        private Button SairButton;
    }
}