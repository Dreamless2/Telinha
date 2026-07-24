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
            /*PrincipalButton.Click += PrincipalButton_Click;
            SobreButton.Click += SobreButton_Click;
            FecharButton.Click += FecharButton_Click;*/
            TimerHora.Tick += TimerHora_Tick;
            Load += Principal_Load;
            PanelTopBar.MouseDown += PanelTopBar_MouseDown;
        }

        private void Principal_Load(object? sender, EventArgs e)
        {
            TimerHora.Enabled = true;
            TimerHora.Start();
        }

        private void TimerHora_Tick(object? sender, EventArgs e)
        {
            var time = DateTime.Now;
            LabelHora.Text = "Hora: " + time.ToString("T");
        }

        private void PanelTopBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void FecharButton_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja realmente sair?", "Confirmação", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Application.Exit();
        }
        private void SobreButton_Click(object? sender, EventArgs e)
        {
            var sobre = new Sobre();
            sobre.ShowDialog();
        }
        private void PrincipalButton_Click(object? sender, EventArgs e)
        {
            var home = _scope.Resolve<Home>();
            home.ShowDialog();
        }
    }
}
