using Autofac;
using Telinha.Factory;
using Telinha.Services;

namespace Telinha
{
    public partial class Token : Form
    {
        private readonly AppConfigServices _configService = new();
        private readonly ILifetimeScope _scope;
        public Token(ILifetimeScope scope, AppConfigServices configService)
        {
            InitializeComponent();
            _scope = scope;

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
                var config = new AppConfigServices.AppConfig
                {
                    TMDB = TokenTMDBBox.Text?.Trim(),
                    DEEPL = TokenDEEPLBox.Text?.Trim(),
                    Host = HostBox.Text?.Trim(),
                    Porta = PortaBox.Text?.Trim(),
                    Usuario = UsuarioBox.Text?.Trim(),
                    Senha = SenhaBox.Text?.Trim()
                };

                if (string.IsNullOrWhiteSpace(config.TMDB) &&
                    string.IsNullOrWhiteSpace(config.DEEPL) &&
                    string.IsNullOrWhiteSpace(config.Host) &&
                    string.IsNullOrWhiteSpace(config.Porta) &&
                    string.IsNullOrWhiteSpace(config.Usuario) &&
                    string.IsNullOrWhiteSpace(config.Senha))
                {
                    MessageBox.Show("Preencha os campos para continuar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _configService.Save(config);

                MessageBox.Show("Dados salvos com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Hide();

                // 🔥 Pede pro Autofac criar o Home com todas as dependências
                var home = _scope.Resolve<Home>();
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
