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
            groupBox2 = new GroupBox();
            HostBox = new TextBox();
            PortaBox = new TextBox();
            UsuarioBox = new TextBox();
            SenhaBox = new TextBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
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
            label3.Location = new Point(10, 29);
            label3.Name = "label3";
            label3.Size = new Size(32, 15);
            label3.TabIndex = 9;
            label3.Text = "Host";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(10, 61);
            label4.Name = "label4";
            label4.Size = new Size(35, 15);
            label4.TabIndex = 10;
            label4.Text = "Porta";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(10, 101);
            label5.Name = "label5";
            label5.Size = new Size(47, 15);
            label5.TabIndex = 11;
            label5.Text = "Usuário";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(10, 134);
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
            groupBox1.Location = new Point(41, 111);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(844, 100);
            groupBox1.TabIndex = 13;
            groupBox1.TabStop = false;
            groupBox1.Text = "API";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(SenhaBox);
            groupBox2.Controls.Add(UsuarioBox);
            groupBox2.Controls.Add(PortaBox);
            groupBox2.Controls.Add(HostBox);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label5);
            groupBox2.Location = new Point(41, 243);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(844, 175);
            groupBox2.TabIndex = 14;
            groupBox2.TabStop = false;
            groupBox2.Text = "Dados";
            // 
            // HostBox
            // 
            HostBox.Location = new Point(77, 26);
            HostBox.Name = "HostBox";
            HostBox.PlaceholderText = "Host";
            HostBox.Size = new Size(754, 23);
            HostBox.TabIndex = 13;
            HostBox.Text = "localhost";
            // 
            // PortaBox
            // 
            PortaBox.Location = new Point(77, 58);
            PortaBox.Name = "PortaBox";
            PortaBox.PlaceholderText = "Porta";
            PortaBox.Size = new Size(754, 23);
            PortaBox.TabIndex = 14;
            PortaBox.Text = "3306";
            // 
            // UsuarioBox
            // 
            UsuarioBox.Location = new Point(77, 93);
            UsuarioBox.Name = "UsuarioBox";
            UsuarioBox.PlaceholderText = "Usuário";
            UsuarioBox.Size = new Size(754, 23);
            UsuarioBox.TabIndex = 15;
            // 
            // SenhaBox
            // 
            SenhaBox.Location = new Point(77, 131);
            SenhaBox.Name = "SenhaBox";
            SenhaBox.PlaceholderText = "Senha";
            SenhaBox.Size = new Size(754, 23);
            SenhaBox.TabIndex = 16;
            // 
            // Token
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(929, 784);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
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
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
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
        private GroupBox groupBox2;
        private TextBox SenhaBox;
        private TextBox UsuarioBox;
        private TextBox PortaBox;
        private TextBox HostBox;
    }
}