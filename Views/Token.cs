using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telinha.Services;

namespace Telinha
{
    public partial class Token : Form
    {
        public Token()
        {
            InitializeComponent();
            SalvarButton.Click += SalvarButton_Click!;
            SairButton.Click += SairButton_Click!;
        }
        private async void SalvarButton_Click(object sender, EventArgs e)
        {
            // Lógica para salvar o token
            var tmdbToken = TokenTMDBBox.Text;
            var deeplToken = TokenDEEPLBox.Text;
            var tmdbDescription = "Token de acesso à API do TMDB";
            var deeplDescription = "Token de acesso à API do DeepL";
            var tokenService = new TokenServices();
            await tokenService.SalvarTokenAsync("TMDB", tmdbToken, tmdbDescription);
            await tokenService.SalvarTokenAsync("DEEPL", deeplToken, deeplDescription);
            MessageBox.Show("Tokens salvos com sucesso!");
        }
        private void SairButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
