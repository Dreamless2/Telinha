using Autofac;
using System.Runtime.InteropServices;
using Telinha.Views;

namespace Telinha.Forms.Views
{
    public partial class Principal : Form
    {
        private readonly ILifetimeScope _scope;

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ReleaseCapture();

        [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
        private static partial int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        public Principal(ILifetimeScope scope)
        {
            InitializeComponent();
            _scope = scope;
            /*HomeButton.Click += HomeButton_Click;
            SobreButton.Click += SobreButton_Click;
            SairButton.Click += SairButton_Click;*/
            TimerHora.Tick += TimerHora_Tick;
            PanelTopBar.MouseDown += PanelTopBar_MouseDown;
        }

        private void TimerHora_Tick(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PanelTopBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void SairButton_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja realmente sair?", "Confirmação", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
