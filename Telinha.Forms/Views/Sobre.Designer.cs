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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Sobre));
            PanelBottom = new Panel();
            PanelTopBar = new Panel();
            PanelTopTitle = new Panel();
            label1 = new Label();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            SobreButton = new Button();
            pictureBox2 = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            LinkTMDB = new LinkLabel();
            PanelBottom.SuspendLayout();
            PanelTopTitle.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = Color.FromArgb(4, 52, 72);
            PanelBottom.Controls.Add(LinkTMDB);
            PanelBottom.Dock = DockStyle.Bottom;
            PanelBottom.Location = new Point(0, 427);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(743, 44);
            PanelBottom.TabIndex = 5;
            // 
            // PanelTopBar
            // 
            PanelTopBar.BackColor = Color.FromArgb(4, 52, 72);
            PanelTopBar.Dock = DockStyle.Top;
            PanelTopBar.Location = new Point(0, 0);
            PanelTopBar.Name = "PanelTopBar";
            PanelTopBar.Size = new Size(743, 29);
            PanelTopBar.TabIndex = 4;
            // 
            // PanelTopTitle
            // 
            PanelTopTitle.BackColor = Color.FromArgb(5, 74, 105);
            PanelTopTitle.Controls.Add(label1);
            PanelTopTitle.Dock = DockStyle.Top;
            PanelTopTitle.Location = new Point(0, 29);
            PanelTopTitle.Name = "PanelTopTitle";
            PanelTopTitle.Size = new Size(743, 58);
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
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(SobreButton);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 87);
            panel1.Name = "panel1";
            panel1.Size = new Size(743, 340);
            panel1.TabIndex = 7;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Forms.Properties.Resources.tmdb;
            pictureBox1.Location = new Point(277, 156);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(126, 54);
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
            SobreButton.Location = new Point(193, 262);
            SobreButton.Name = "SobreButton";
            SobreButton.Size = new Size(180, 41);
            SobreButton.TabIndex = 9;
            SobreButton.Text = "Fechar";
            SobreButton.UseVisualStyleBackColor = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(10, 16);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(261, 194);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 11;
            pictureBox2.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(277, 138);
            label2.Name = "label2";
            label2.Size = new Size(397, 15);
            label2.TabIndex = 12;
            label2.Text = "This product uses the TMDb API but is not endorsed or certified by TMDb.";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(277, 16);
            label3.Name = "label3";
            label3.Size = new Size(190, 65);
            label3.TabIndex = 13;
            label3.Text = "Telinha";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(277, 81);
            label4.Name = "label4";
            label4.Size = new Size(76, 15);
            label4.TabIndex = 14;
            label4.Text = "1.0.0 (64 bits)";
            // 
            // LinkTMDB
            // 
            LinkTMDB.AutoSize = true;
            LinkTMDB.LinkColor = Color.White;
            LinkTMDB.Location = new Point(80, 15);
            LinkTMDB.Name = "LinkTMDB";
            LinkTMDB.Size = new Size(114, 15);
            LinkTMDB.TabIndex = 15;
            LinkTMDB.TabStop = true;
            LinkTMDB.Text = "The Movie Database";
            LinkTMDB.LinkClicked += LinkTMDB_LinkClicked;
            // 
            // Sobre
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(743, 471);
            Controls.Add(panel1);
            Controls.Add(PanelTopTitle);
            Controls.Add(PanelBottom);
            Controls.Add(PanelTopBar);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Sobre";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sobre";
            PanelBottom.ResumeLayout(false);
            PanelBottom.PerformLayout();
            PanelTopTitle.ResumeLayout(false);
            PanelTopTitle.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
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
        private PictureBox pictureBox2;
        private Label label2;
        private Label label3;
        private Label label4;
        private LinkLabel LinkTMDB;
    }
}