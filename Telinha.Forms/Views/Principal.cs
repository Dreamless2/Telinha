using Autofac;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telinha.Views;

namespace Telinha.Forms.Views
{
    private readonly ILifetimeScope _scope;
    public partial class Principal : Form
    {
        public Principal(ILifetimeScope scope)
        {
            InitializeComponent();
            HomeButton.Click += HomeButton_Click;
            SobreButton.Click += SobreButton_Click;
            SairButton.Click += SairButton_Click;
        }

        private void SairButton_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SobreButton_Click(object? sender, EventArgs e)
        {
            var sobre = new Sobre();
            sobre.ShowDialog();
        }

        private void HomeButton_Click(object? sender, EventArgs e)
        {
            var home = _scope.Resolve<Home>();
            home.ShowDialog();
        }
    }
}
