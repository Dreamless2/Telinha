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
        private readonly TokenServices _tokenService = new(); // 🔥 mantém instância única
        public Token()
        {
            InitializeComponent();
            SalvarButton.Click += SalvarButton_Click!;
            SairButton.Click += SairButton_Click!;
        }
        private async void SalvarButton_Click(object sender, EventArgs e)
        {
            try
            {
                var tmdbToken = TokenTMDBBox.Text?.Trim();
                var deeplToken = TokenDEEPLBox.Text?.Trim();

                if (string.IsNullOrWhiteSpace(tmdbToken) && string.IsNullOrWhiteSpace(deeplToken))
                {
                    MessageBox.Show("Informe pelo menos um token.");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(tmdbToken))
                {
                    await _tokenService.SalvarTokenAsync(
                        "TMDB",
                        tmdbToken,
                        "Token de acesso à API do TMDB"
                    );
                }

                if (!string.IsNullOrWhiteSpace(deeplToken))
                {
                    await _tokenService.SalvarTokenAsync(
                        "DEEPL",
                        deeplToken,
                        "Token de acesso à API do DeepL"
                    );
                }

                MessageBox.Show("Tokens salvos com sucesso!");

                Hide();

                using var home = new Home();
                home.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar tokens:\n{ex.Message}");
            }
        }
        private void SairButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja realmente sair?", "Confirmação", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
