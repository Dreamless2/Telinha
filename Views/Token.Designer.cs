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
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // TokenTMDBBox
            // 
            TokenTMDBBox.Location = new Point(77, 22);
            TokenTMDBBox.Name = "TokenTMDBBox";
            TokenTMDBBox.PlaceholderText = "API TMDB";
            TokenTMDBBox.Size = new Size(754, 23);
            TokenTMDBBox.TabIndex = 0;
            // 
            // TokenDEEPLBox
            // 
            TokenDEEPLBox.Location = new Point(77, 60);
            TokenDEEPLBox.Name = "TokenDEEPLBox";
            TokenDEEPLBox.PlaceholderText = "API DEEPL";
            TokenDEEPLBox.Size = new Size(754, 23);
            TokenDEEPLBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 30);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 2;
            label1.Text = "API TMDB";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 68);
            label2.Name = "label2";
            label2.Size = new Size(61, 15);
            label2.TabIndex = 3;
            label2.Text = "API DEEPL";
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
            PanelBottom.Location = new Point(0, 740);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(929, 44);
            PanelBottom.TabIndex = 6;
            // 
            // SalvarButton
            // 
            SalvarButton.Location = new Point(253, 658);
            SalvarButton.Name = "SalvarButton";
            SalvarButton.Size = new Size(180, 41);
            SalvarButton.TabIndex = 7;
            SalvarButton.Text = "Salvar ";
            SalvarButton.UseVisualStyleBackColor = true;
            // 
            // SairButton
            // 
            SairButton.Location = new Point(489, 658);
            SairButton.Name = "SairButton";
            SairButton.Size = new Size(180, 41);
            SairButton.TabIndex = 8;
            SairButton.Text = "Sair";
            SairButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(98, 440);
            label3.Name = "label3";
            label3.Size = new Size(32, 15);
            label3.TabIndex = 9;
            label3.Text = "Host";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(98, 482);
            label4.Name = "label4";
            label4.Size = new Size(35, 15);
            label4.TabIndex = 10;
            label4.Text = "Porta";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(98, 530);
            label5.Name = "label5";
            label5.Size = new Size(47, 15);
            label5.TabIndex = 11;
            label5.Text = "Usuário";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(271, 599);
            label6.Name = "label6";
            label6.Size = new Size(39, 15);
            label6.TabIndex = 12;
            label6.Text = "Senha";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(TokenDEEPLBox);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(TokenTMDBBox);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(51, 216);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(844, 100);
            groupBox1.TabIndex = 13;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // Token
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(929, 784);
            Controls.Add(groupBox1);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(SairButton);
            Controls.Add(SalvarButton);
            Controls.Add(PanelBottom);
            Controls.Add(PanelTopTitle);
            Controls.Add(PanelTopBar);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Token";
            StartPosition = FormStartPosition.CenterScreen;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
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
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private GroupBox groupBox1;
    }
}