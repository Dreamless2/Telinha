namespace Telinha.Views
{
    partial class Sobre
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
            PanelBottom = new Panel();
            PanelTopBar = new Panel();
            PanelTopTitle = new Panel();
            label1 = new Label();
            panel1 = new Panel();
            RestSharpLabel = new Label();
            FreeSqlLabel = new Label();
            label4 = new Label();
            label5 = new Label();
            PanelTopTitle.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = Color.FromArgb(4, 52, 72);
            PanelBottom.Dock = DockStyle.Bottom;
            PanelBottom.Location = new Point(0, 508);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(892, 44);
            PanelBottom.TabIndex = 5;
            // 
            // PanelTopBar
            // 
            PanelTopBar.BackColor = Color.FromArgb(4, 52, 72);
            PanelTopBar.Dock = DockStyle.Top;
            PanelTopBar.Location = new Point(0, 0);
            PanelTopBar.Name = "PanelTopBar";
            PanelTopBar.Size = new Size(892, 29);
            PanelTopBar.TabIndex = 4;
            // 
            // PanelTopTitle
            // 
            PanelTopTitle.BackColor = Color.FromArgb(5, 74, 105);
            PanelTopTitle.Controls.Add(label1);
            PanelTopTitle.Dock = DockStyle.Top;
            PanelTopTitle.Location = new Point(0, 29);
            PanelTopTitle.Name = "PanelTopTitle";
            PanelTopTitle.Size = new Size(892, 58);
            PanelTopTitle.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(10, 9);
            label1.Name = "label1";
            label1.Size = new Size(91, 40);
            label1.TabIndex = 0;
            label1.Text = "Sobre";
            // 
            // panel1
            // 
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(FreeSqlLabel);
            panel1.Controls.Add(RestSharpLabel);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 87);
            panel1.Name = "panel1";
            panel1.Size = new Size(892, 421);
            panel1.TabIndex = 7;
            // 
            // RestSharpLabel
            // 
            RestSharpLabel.AutoSize = true;
            RestSharpLabel.Font = new Font("Segoe UI", 12F);
            RestSharpLabel.Location = new Point(12, 43);
            RestSharpLabel.Name = "RestSharpLabel";
            RestSharpLabel.Size = new Size(52, 21);
            RestSharpLabel.TabIndex = 0;
            RestSharpLabel.Text = "label2";
            // 
            // FreeSqlLabel
            // 
            FreeSqlLabel.AutoSize = true;
            FreeSqlLabel.Font = new Font("Segoe UI", 12F);
            FreeSqlLabel.Location = new Point(12, 83);
            FreeSqlLabel.Name = "FreeSqlLabel";
            FreeSqlLabel.Size = new Size(52, 21);
            FreeSqlLabel.TabIndex = 1;
            FreeSqlLabel.Text = "label3";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(12, 123);
            label4.Name = "label4";
            label4.Size = new Size(52, 21);
            label4.TabIndex = 2;
            label4.Text = "label4";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(10, 163);
            label5.Name = "label5";
            label5.Size = new Size(52, 21);
            label5.TabIndex = 3;
            label5.Text = "label5";
            // 
            // Sobre
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(892, 552);
            Controls.Add(panel1);
            Controls.Add(PanelTopTitle);
            Controls.Add(PanelBottom);
            Controls.Add(PanelTopBar);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Sobre";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sobre";
            PanelTopTitle.ResumeLayout(false);
            PanelTopTitle.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel PanelBottom;
        private Panel PanelTopBar;
        private Panel PanelTopTitle;
        private Label label1;
        private Panel panel1;
        private Label RestSharpLabel;
        private Label label5;
        private Label label4;
        private Label FreeSqlLabel;
    }
}