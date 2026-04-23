using Telinha.Factory;
using Telinha.Services;
using Telinha.Store;

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
            HostBox.PasswordChar = '\u200B';
            PortaBox.PasswordChar = '\u200B';
            UsuarioBox.PasswordChar = '\u200B';
            SenhaBox.PasswordChar = '\u200B';
            TokenDEEPLBox.ShortcutsEnabled = true;
            TokenTMDBBox.ShortcutsEnabled = true;
            HostBox.ShortcutsEnabled = true;
            PortaBox.ShortcutsEnabled = true;
            UsuarioBox.ShortcutsEnabled = true;
            SenhaBox.ShortcutsEnabled = true;
            TokenDEEPLBox.TextChanged += (s, e) => Hidden(TokenDEEPLBox);
            TokenTMDBBox.TextChanged += (s, e) => Hidden(TokenTMDBBox);
            HostBox.TextChanged += (s, e) => Hidden(HostBox);
            PortaBox.TextChanged += (s, e) => Hidden(PortaBox);
            UsuarioBox.TextChanged += (s, e) => Hidden(UsuarioBox);
            SenhaBox.TextChanged += (s, e) => Hidden(SenhaBox);
        }
        private static void Hidden(TextBoxBase txt)
        {
            txt.SelectionStart = 0;
            txt.SelectionLength = 0;
        }

        private async void SalvarButton_Click(object sender, EventArgs e)
        {
            try
            {
                var tmdbToken = TokenTMDBBox.Text?.Trim();
                var deeplToken = TokenDEEPLBox.Text?.Trim();
                var host = HostBox.Text?.Trim();
                var porta = PortaBox.Text?.Trim();
                var usuario = UsuarioBox.Text?.Trim();
                var senha = SenhaBox.Text?.Trim();


                if (string.IsNullOrWhiteSpace(tmdbToken) && string.IsNullOrWhiteSpace(deeplToken) && string.IsNullOrWhiteSpace(host) && string.IsNullOrWhiteSpace(porta) && string.IsNullOrWhiteSpace(usuario) && string.IsNullOrWhiteSpace(senha))
                {
                    MessageBox.Show("Preencha os campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(tmdbToken))
                {
                    await _tokenService.SalvarTokenAsync(
                        "TMDB",
                        tmdbToken,
                        "Chave de API do TMDB"
                    );
                }

                if (!string.IsNullOrWhiteSpace(deeplToken))
                {
                    await _tokenService.SalvarTokenAsync(
                        "DEEPL",
                        deeplToken,
                        "Chave de API do DeepL"
                    );
                }

                var configStore = new SecureConfigStore();
                var existing = configStore.Load() ?? new SecureConfigStore.ConfigData();

                existing.Host = host ?? existing.Host;
                existing.Porta = porta ?? existing.Porta;
                existing.Usuario = usuario ?? existing.Usuario;
                existing.Senha = senha ?? existing.Senha;

                configStore.Save(existing);

                MessageBox.Show("Chaves de acesso salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Hide();

                var tokenService = new TokenServices();
                var apiFactory = new ApiClientFactory(tokenService);

                using var home = new Home(apiFactory);
                home.ShowDialog();
            }
            catch (Exception ex)
            {
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
