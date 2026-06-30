using System.Runtime.InteropServices;

namespace Telinha.Views
{
    public partial class Sobre : Form
    {

        #region Low Level
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        #endregion
        public Sobre()
        {
            InitializeComponent();
            PanelTopBar.MouseDown +=
        }

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
