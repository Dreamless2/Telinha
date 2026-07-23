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
            PanelTopTitle.SuspendLayout();
            SuspendLayout();
            // 
            // PanelTopBar
            // 
            PanelTopBar.BackColor = Color.FromArgb(4, 52, 72);
            PanelTopBar.Dock = DockStyle.Top;
            PanelTopBar.Location = new Point(0, 58);
            PanelTopBar.Name = "PanelTopBar";
            PanelTopBar.Size = new Size(1215, 96);
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
            PanelTopTitle.Size = new Size(1215, 58);
            PanelTopTitle.TabIndex = 16;
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = Color.FromArgb(4, 52, 72);
            PanelBottom.Dock = DockStyle.Bottom;
            PanelBottom.Location = new Point(0, 568);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(1215, 44);
            PanelBottom.TabIndex = 17;
            // 
            // Principal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1215, 612);
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
    }
}