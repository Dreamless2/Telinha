using Autofac;
using System.Runtime.InteropServices;

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

        private Form? currentChildForm;
        public Principal(ILifetimeScope scope)
        {
            InitializeComponent();
            _scope = scope;
            PrincipalButton.Click += PrincipalButton_Click;
            FecharButton.Click += FecharButton_Click;
            PanelTopBar.MouseDown += PanelTopBar_MouseDown;
            Load += Principal_Load;
        }

        private void Principal_Load(object? sender, EventArgs e)
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
        private void OpenChildForm(Form childForm)
        {
            currentChildForm?.Close();
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            PanelDesktop.Controls.Add(childForm);
            PanelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }
        private void FecharButton_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja realmente sair?", "Confirmação", MessageBoxButtons.YesNo) == DialogResult.Yes) Application.Exit();
        }
        private void PrincipalButton_Click(object? sender, EventArgs e)
        {
            OpenChildForm(_scope.Resolve<Home>());
            PrincipalLabel.Text = "Home";
        }
    }
}
