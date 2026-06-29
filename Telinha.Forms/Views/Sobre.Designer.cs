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
            LinkTMDB = new LinkLabel();
            PanelTopBar = new Panel();
            PanelTopTitle = new Panel();
            label1 = new Label();
            panel1 = new Panel();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            SobreButton = new Button();
            label5 = new Label();
            PanelBottom.SuspendLayout();
            PanelTopTitle.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // PanelBottom
            // 
            PanelBottom.BackColor = Color.FromArgb(4, 52, 72);
            PanelBottom.Controls.Add(LinkTMDB);
            PanelBottom.Dock = DockStyle.Bottom;
            PanelBottom.Location = new Point(0, 424);
            PanelBottom.Name = "PanelBottom";
            PanelBottom.Size = new Size(565, 44);
            PanelBottom.TabIndex = 5;
            // 
            // LinkTMDB
            // 
            LinkTMDB.AutoSize = true;
            LinkTMDB.LinkColor = Color.White;
            LinkTMDB.Location = new Point(225, 15);
            LinkTMDB.Name = "LinkTMDB";
            LinkTMDB.Size = new Size(114, 15);
            LinkTMDB.TabIndex = 15;
            LinkTMDB.TabStop = true;
            LinkTMDB.Text = "The Movie Database";
            LinkTMDB.LinkClicked += LinkTMDB_LinkClicked;
            // 
            // PanelTopBar
            // 
            PanelTopBar.BackColor = Color.FromArgb(4, 52, 72);
            PanelTopBar.Dock = DockStyle.Top;
            PanelTopBar.Location = new Point(0, 0);
            PanelTopBar.Name = "PanelTopBar";
            PanelTopBar.Size = new Size(565, 29);
            PanelTopBar.TabIndex = 4;
            // 
            // PanelTopTitle
            // 
            PanelTopTitle.BackColor = Color.FromArgb(5, 74, 105);
            PanelTopTitle.Controls.Add(label1);
            PanelTopTitle.Dock = DockStyle.Top;
            PanelTopTitle.Location = new Point(0, 29);
            PanelTopTitle.Name = "PanelTopTitle";
            PanelTopTitle.Size = new Size(565, 58);
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
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(SobreButton);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 87);
            panel1.Name = "panel1";
            panel1.Size = new Size(565, 337);
            panel1.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(313, 86);
            label4.Name = "label4";
            label4.Size = new Size(104, 21);
            label4.TabIndex = 14;
            label4.Text = "1.0.0 (64 bits)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(313, 20);
            label3.Name = "label3";
            label3.Size = new Size(190, 65);
            label3.TabIndex = 13;
            label3.Text = "Telinha";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(12, 217);
            label2.Name = "label2";
            label2.Size = new Size(443, 17);
            label2.TabIndex = 12;
            label2.Text = "This product uses the TMDb API but is not endorsed or certified by TMDb.";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(12, 20);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(261, 194);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 11;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Forms.Properties.Resources.tmdb;
            pictureBox1.Location = new Point(313, 136);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(205, 78);
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
            SobreButton.Location = new Point(192, 262);
            SobreButton.Name = "SobreButton";
            SobreButton.Size = new Size(180, 41);
            SobreButton.TabIndex = 9;
            SobreButton.Text = "Fechar";
            SobreButton.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(313, 112);
            label5.Name = "label5";
            label5.Size = new Size(205, 21);
            label5.TabIndex = 15;
            label5.Text = "Desenvolvido por Tiago.NET";
            // 
            // Sobre
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(565, 468);
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
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
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
        private PictureBox pictureBox2;
        private Label label2;
        private Label label3;
        private Label label4;
        private LinkLabel LinkTMDB;
        private Label label5;
    }
}