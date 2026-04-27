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
            this.Text = "Sobre o Aplicativo";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            // Adiciona um rótulo com informações sobre o aplicativo
            Label lblInfo = new Label();
            lblInfo.Text = "Este é um aplicativo de exemplo para demonstrar a tela 'Sobre'.\n" +
                           "Desenvolvido por: Seu Nome\n" +
                           "Versão: 1.0.0\n" +
                           "Data de lançamento: 2024-06-01";
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(20, 20);
            this.Controls.Add(lblInfo);
            // Adiciona um botão para fechar a janela
            Button btnFechar = new Button();
            btnFechar.Text = "Fechar";
            btnFechar.Location = new Point(150, 200);
            btnFechar.Click += (s, args) => this.Close();
            this.Controls.Add(btnFechar);
        }
    }
}
