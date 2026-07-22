using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telinha.Forms.Views
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
            HomeButton.Click += HomeButton_Click;
        }

        private void HomeButton_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
