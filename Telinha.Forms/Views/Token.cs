using Autofac;
using System.Runtime.InteropServices;
using Telinha.Core.Services;
using Telinha.Forms.Views;

namespace Telinha
{
    public partial class Token : Form
    {
        private readonly ILifetimeScope _scope;
        private readonly AppConfigServices _configService;

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ReleaseCapture();

        [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
        private static partial int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public Token(ILifetimeScope scope, AppConfigServices configService)
        {
            InitializeComponent();
            _scope = scope;
            _configService = configService;
            SalvarButton.Click += SalvarButton_Click!;
            SairButton.Click += SairButton_Click!;
            TokenDEEPLBox.ShortcutsEnabled = true;
            TokenTMDBBox.ShortcutsEnabled = true;
            HostBox.ShortcutsEnabled = true;
            PortaBox.ShortcutsEnabled = true;
            UsuarioBox.ShortcutsEnabled = true;
            SenhaBox.ShortcutsEnabled = true;
            PanelTopBar.MouseDown += PanelTopBar_MouseDown;
        }
        private void PanelTopBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
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
                    MessageBox.Show("Preencha os campos para continuar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _configService.Save(config);
                MessageBox.Show("Dados salvos com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Hide();
                var home = _scope.Resolve<Principal>();
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
                Application.Exit();
        }
    }
}
