using System.Runtime.InteropServices;

namespace Telinha.Views
{
    #region Form
    public partial class Sobre : Form
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ReleaseCapture();

        [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
        private static partial int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        #region Constructor
        public Sobre()
        {
            InitializeComponent();
            PanelTopBar.MouseDown += PanelTopBar_MouseDown;
        }
        #endregion

        #region Mover Form
        private void PanelTopBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        #region Link
        private void LinkTMDB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                LinkTMDB.LinkVisited = true;
                string url = "https://www.themoviedb.org/";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível abrir o link: " + ex.Message);
            }
        }

        private void SobreButton_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}