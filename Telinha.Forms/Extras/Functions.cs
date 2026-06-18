using System.Windows.Forms;

namespace Telinha.Forms.Extras
{
    public class Functions
    {
        public static void OnlyNumbers(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}