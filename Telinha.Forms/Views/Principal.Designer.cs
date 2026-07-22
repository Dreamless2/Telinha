namespace Telinha.Forms.Views
{
    partial class Principal
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
            SairButton = new Button();
            SalvarButton = new Button();
            PanelTopBar = new Panel();
            label7 = new Label();
            PanelTopTitle = new Panel();
            PanelBottom = new Panel();
            PanelTopTitle.SuspendLayout();
            SuspendLayout();
            // 
            // SairButton
            // 
            SairButton.Location = new Point(232, 525);
            SairButton.Name = "SairButton";
            SairButton.Size = new Size(180, 41);
            SairButton.TabIndex = 19;
            SairButton.Text = "Sair";
            SairButton.UseVisualStyleBackColor = true;
            // 
            // SalvarButton
            // 
            SalvarButton.Location = new Point(106, 187);
            SalvarButton.Name = "SalvarButton";
            SalvarButton.Size = new Size(180, 41);
            SalvarButton.TabIndex = 18;
            SalvarButton.Text = "Salvar ";
            SalvarButton.UseVisualStyleBackColor = true;
            // 
            // PanelTopBar
            // 
            PanelTopBar.BackColor = Color.FromArgb(4, 52, 72);
            PanelTopBar.Dock = DockStyle.Top;
            PanelTopBar.Location = new Point(0, 58);
            PanelTopBar.Name = "PanelTopBar";
            PanelTopBar.Size = new Size(504, 29);
            PanelTopBar.TabIndex = 15;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.White;
            label7.Location = new Point(14, 9);
            label7.Name = "label7";
            label7.Size = new Size(105, 40);
            label7.TabIndex = 0;
            label7.Text = "Acesso";
            // 
            // PanelTopTitle
            // 
            PanelTopTitle.BackColor = Color.FromArgb(5, 74, 105);
            PanelTopTitle.Controls.Add(label7);
            PanelTopTitle.Dock = DockStyle.Top;
            PanelTopTitle.Location = new Point(0, 0);
            PanelTopTitle.Name = "PanelTopTitle";
            PanelTopTitle.Size = new Size(504, 58);
            PanelTopTitle.TabIndex = 16;
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = Color.FromArgb(4, 52, 72);
            PanelBottom.Dock = DockStyle.Bottom;
            PanelBottom.Location = new Point(0, 694);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(504, 44);
            PanelBottom.TabIndex = 17;
            // 
            // Principal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(504, 738);
            Controls.Add(SairButton);
            Controls.Add(SalvarButton);
            Controls.Add(PanelTopBar);
            Controls.Add(PanelTopTitle);
            Controls.Add(PanelBottom);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Principal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Principal";
            PanelTopTitle.ResumeLayout(false);
            PanelTopTitle.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button SairButton;
        private Button SalvarButton;
        private Panel PanelTopBar;
        private Label label7;
        private Panel PanelTopTitle;
        private Panel PanelBottom;
    }
}