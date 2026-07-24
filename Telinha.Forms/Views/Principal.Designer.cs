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
            PanelTopBar = new Panel();
            label7 = new Label();
            PanelTopTitle = new Panel();
            PanelBottom = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            PrincipalButton = new Button();
            SobreButton = new Button();
            FecharButton = new Button();
            PanelDesktop = new Panel();
            PanelTopTitle.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // PanelTopBar
            // 
            PanelTopBar.BackColor = Color.FromArgb(4, 52, 72);
            PanelTopBar.Dock = DockStyle.Top;
            PanelTopBar.Location = new Point(0, 58);
            PanelTopBar.Name = "PanelTopBar";
            PanelTopBar.Size = new Size(1605, 65);
            PanelTopBar.TabIndex = 15;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.White;
            label7.Location = new Point(14, 9);
            label7.Name = "label7";
            label7.Size = new Size(125, 40);
            label7.TabIndex = 0;
            label7.Text = "Principal";
            // 
            // PanelTopTitle
            // 
            PanelTopTitle.BackColor = Color.FromArgb(5, 74, 105);
            PanelTopTitle.Controls.Add(label7);
            PanelTopTitle.Dock = DockStyle.Top;
            PanelTopTitle.Location = new Point(0, 0);
            PanelTopTitle.Name = "PanelTopTitle";
            PanelTopTitle.Size = new Size(1605, 58);
            PanelTopTitle.TabIndex = 16;
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = Color.FromArgb(4, 52, 72);
            PanelBottom.Dock = DockStyle.Bottom;
            PanelBottom.Location = new Point(0, 1037);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(1605, 44);
            PanelBottom.TabIndex = 17;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(PrincipalButton);
            flowLayoutPanel1.Controls.Add(SobreButton);
            flowLayoutPanel1.Controls.Add(FecharButton);
            flowLayoutPanel1.Dock = DockStyle.Left;
            flowLayoutPanel1.Location = new Point(0, 123);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(173, 914);
            flowLayoutPanel1.TabIndex = 20;
            // 
            // PrincipalButton
            // 
            PrincipalButton.FlatAppearance.BorderSize = 0;
            PrincipalButton.FlatStyle = FlatStyle.Flat;
            PrincipalButton.Location = new Point(3, 3);
            PrincipalButton.Name = "PrincipalButton";
            PrincipalButton.Size = new Size(168, 60);
            PrincipalButton.TabIndex = 0;
            PrincipalButton.Text = "Principal";
            PrincipalButton.UseVisualStyleBackColor = true;
            // 
            // SobreButton
            // 
            SobreButton.FlatAppearance.BorderSize = 0;
            SobreButton.FlatStyle = FlatStyle.Flat;
            SobreButton.Location = new Point(3, 69);
            SobreButton.Name = "SobreButton";
            SobreButton.Size = new Size(168, 60);
            SobreButton.TabIndex = 1;
            SobreButton.Text = "Sobre";
            SobreButton.UseVisualStyleBackColor = true;
            // 
            // FecharButton
            // 
            FecharButton.FlatAppearance.BorderSize = 0;
            FecharButton.FlatStyle = FlatStyle.Flat;
            FecharButton.Location = new Point(3, 135);
            FecharButton.Name = "FecharButton";
            FecharButton.Size = new Size(168, 60);
            FecharButton.TabIndex = 2;
            FecharButton.Text = "Fechar";
            FecharButton.UseVisualStyleBackColor = true;
            // 
            // PanelDesktop
            // 
            PanelDesktop.Dock = DockStyle.Fill;
            PanelDesktop.Location = new Point(173, 123);
            PanelDesktop.Name = "PanelDesktop";
            PanelDesktop.Size = new Size(1432, 914);
            PanelDesktop.TabIndex = 21;
            // 
            // Principal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1605, 1081);
            Controls.Add(PanelDesktop);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(PanelTopBar);
            Controls.Add(PanelTopTitle);
            Controls.Add(PanelBottom);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Principal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Principal";
            PanelTopTitle.ResumeLayout(false);
            PanelTopTitle.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Panel PanelTopBar;
        private Label label7;
        private Panel PanelTopTitle;
        private Panel PanelBottom;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button PrincipalButton;
        private Button SobreButton;
        private Button FecharButton;
        private Panel PanelDesktop;
    }
}