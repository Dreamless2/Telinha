using Telinha.Factory;
using Telinha.Infrastructure.Logging;
using Telinha.Services;

namespace Telinha
{
    public partial class Token : Form
    {
        private readonly TokenServices _tokenService;
        public Token(TokenServices tokenService)
        {
            _tokenService = tokenService;
            InitializeComponent();
            SalvarButton.Click += SalvarButton_Click!;
            SairButton.Click += SairButton_Click!;
            TokenDEEPLBox.PasswordChar = '\u200B';
            TokenTMDBBox.PasswordChar = '\u200B';
            TokenDEEPLBox.ShortcutsEnabled = true;
            TokenTMDBBox.ShortcutsEnabled = true;
            TokenDEEPLBox.TextChanged += (s, e) => Hidden(TokenDEEPLBox, s!, e);
            TokenTMDBBox.TextChanged += (s, e) => Hidden(TokenTMDBBox, s!, e);

        }

        private void Hidden(TextBoxBase txt, object sender, EventArgs e)
        {
            txt.SelectionStart = 0;
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
                        "Chave API do TMDB"
                    );
                }

                if (!string.IsNullOrWhiteSpace(deeplToken))
                {
                    await _tokenService.SalvarTokenAsync(
                        "DEEPL",
                        deeplToken,
                        "Chave API do DeepL"
                    );
                }

                MessageBox.Show("Salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Hide();

                var tokenService = new TokenServices();
                var apiFactory = new ApiClientFactory(tokenService);

                using var home = new Home(apiFactory);
                home.ShowDialog();
            }
            catch (Exception ex)
            {
                LogServices.Error(ex, "Erro ao salvar token.");
                MessageBox.Show($"Erro ao salvar:\n{ex.Message}");
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
