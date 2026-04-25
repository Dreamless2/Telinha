using System;
using System.Collections.Generic;
using System.Text;

namespace Telinha.Utils
{
    public static class ButtonExtensions
    {
        private static readonly Color EnabledBack = Color.FromArgb(4, 52, 72);
        private static readonly Color EnabledFore = Color.White;
        private static readonly Color DisabledBack = Color.FromArgb(45, 45, 45);
        private static readonly Color DisabledFore = Color.FromArgb(130, 130, 130);

        public static void SetMaterialState(this Button btn, bool enabled)
        {
            btn.BackColor = enabled ? EnabledBack : DisabledBack;
            btn.ForeColor = enabled ? EnabledFore : DisabledFore;
            btn.Enabled = enabled;
        }
    }
}