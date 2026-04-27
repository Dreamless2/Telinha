using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telinha.Views
{
    public partial class Sobre : Form
    {
        public Sobre()
        {
            InitializeComponent();
            Load += Sobre_Load;
        }

        private void Sobre_Load(object sender, EventArgs e)
        {
            // Configurações para o formulário "Sobre"

        }
    }
}
