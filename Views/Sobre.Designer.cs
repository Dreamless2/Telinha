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
            pictureBox1 = new PictureBox();
            SobreButton = new Button();
            PanelTopTitle.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = Color.FromArgb(4, 52, 72);
            PanelBottom.Dock = DockStyle.Bottom;
            PanelBottom.Location = new Point(0, 429);
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
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(SobreButton);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 87);
            panel1.Name = "panel1";
            panel1.Size = new Size(892, 342);
            panel1.TabIndex = 7;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.tmdb;
            pictureBox1.Location = new Point(516, 45);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(229, 131);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 10;
            pictureBox1.TabStop = false;
            // 
            // SobreButton
            // 
            SobreButton.BackColor = Color.FromArgb(4, 52, 72);
            SobreButton.FlatAppearance.BorderSize = 0;
            SobreButton.FlatStyle = FlatStyle.Flat;
            SobreButton.ForeColor = Color.White;
            SobreButton.Location = new Point(356, 272);
            SobreButton.Name = "SobreButton";
            SobreButton.Size = new Size(180, 41);
            SobreButton.TabIndex = 9;
            SobreButton.Text = "Fechar";
            SobreButton.UseVisualStyleBackColor = false;
            // 
            // Sobre
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(892, 473);
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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel PanelBottom;
        private Panel PanelTopBar;
        private Panel PanelTopTitle;
        private Label label1;
        private Panel panel1;
        private Button SobreButton;
        private PictureBox pictureBox1;
    }
}