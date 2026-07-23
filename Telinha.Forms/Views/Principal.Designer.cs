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
            components = new System.ComponentModel.Container();
            PanelTopBar = new Panel();
            label7 = new Label();
            PanelTopTitle = new Panel();
            PanelBottom = new Panel();
            label1 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            label2 = new Label();
            PanelTopTitle.SuspendLayout();
            SuspendLayout();
            // 
            // PanelTopBar
            // 
            PanelTopBar.BackColor = Color.FromArgb(4, 52, 72);
            PanelTopBar.Dock = DockStyle.Top;
            PanelTopBar.Location = new Point(0, 58);
            PanelTopBar.Name = "PanelTopBar";
            PanelTopBar.Size = new Size(1245, 96);
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
            PanelTopTitle.Size = new Size(1245, 58);
            PanelTopTitle.TabIndex = 16;
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = Color.FromArgb(4, 52, 72);
            PanelBottom.Dock = DockStyle.Bottom;
            PanelBottom.Location = new Point(0, 726);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(1245, 44);
            PanelBottom.TabIndex = 17;
            // 
            // label1
            // 
            label1.Location = new Point(368, 343);
            label1.Name = "label1";
            label1.Size = new Size(509, 84);
            label1.TabIndex = 18;
            label1.Text = "Hoje são 23 de julho de 2026";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            // 
            // label2
            // 
            label2.Location = new Point(368, 427);
            label2.Name = "label2";
            label2.Size = new Size(509, 84);
            label2.TabIndex = 19;
            label2.Text = "Hoje são 23 de julho de 2026";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Principal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1245, 770);
            Controls.Add(label2);
            Controls.Add(label1);
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
        private Panel PanelTopBar;
        private Label label7;
        private Panel PanelTopTitle;
        private Panel PanelBottom;
        private Label label1;
        private System.Windows.Forms.Timer timer1;
        private Label label2;
    }
}