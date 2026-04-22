namespace Telinha.Utils
{
    public class Functions
    {
        public static void SetCamposEnabled(Control parent, bool enabled, params Type[] tipos)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (tipos.Length == 0 || tipos.Any(t => t.IsAssignableFrom(ctrl.GetType())))
                {
                    ctrl.Enabled = enabled;
                }

                if (ctrl.HasChildren)
                {
                    SetCamposEnabled(ctrl, enabled, tipos);
                }
            }
        }

        public static void OnlyNumbers(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}