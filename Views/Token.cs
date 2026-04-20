using Telinha.Factory;
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

                var tokenService = new TokenServices();
                var apiFactory = new ApiClientFactory(tokenService);

                using var home = new Home(apiFactory);
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
