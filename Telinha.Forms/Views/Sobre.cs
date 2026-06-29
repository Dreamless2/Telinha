using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Telinha.Views
{
    public partial class Sobre : Form
    {
        public Sobre()
        {
            InitializeComponent();
        }

        private void LinkTMDB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                this.LinkTMDB.LinkVisited = true;
                string url = "https://www.themoviedb.org/";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível abrir o link: " + ex.Message);
            }
        }
    }
}
