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
        public static void SetField(Label label, dynamic box, bool enabled, string placeholderWhenEnabled)
        {
            label.Enabled = enabled;
            box.Enabled = enabled;
            box.PlaceholderText = enabled ? placeholderWhenEnabled : "";
        }

        public static void OnlyNumbers(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        public static void ConfigurarCampo(TextBoxBase txt, bool podeEditar)
        {
            txt.Enabled = podeEditar;

            if (podeEditar)
            {
                txt.BackColor = Color.White; // Volta a cor normal
            }
            else
            {
                txt.Text = string.Empty;
            }
        }
    }
}