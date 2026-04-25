using System;
using System.Collections.Generic;
using System.Text;
using Telinha.Services;

namespace Telinha.Helpers
{
    public class ColorHelper
    {

        // 🔥 Arreio de 3 cores. Troca os RGB aí
        private static readonly Color[] Cores =
        {
               new[] { Color.FromArgb(0, 30, 60), Color.FromArgb(0, 74, 127), Color.FromArgb(100, 181, 246) }, // Blue
        new[] { Color.FromArgb(0, 50, 28), Color.FromArgb(0, 105, 61), Color.FromArgb(129, 199, 132) }, // Green
        new[] { Color.FromArgb(49, 27, 63), Color.FromArgb(103, 58, 135), Color.FromArgb(186, 104, 200) }, // Purple
        new[] { Color.FromArgb(0, 45, 45), Color.FromArgb(0, 95, 95), Color.FromArgb(77, 182, 172) }, // Teal
        new[] { Color.FromArgb(62, 31, 0), Color.FromArgb(130, 65, 0), Color.FromArgb(255, 167, 38) }, // Orange
        new[] { Color.FromArgb(60, 0, 0), Color.FromArgb(127, 0, 0), Color.FromArgb(239, 83, 80) }, // Red
        new[] { Color.FromArgb(26, 35, 67), Color.FromArgb(57, 73, 138), Color.FromArgb(121, 134, 203) }, // Indigo
        new[] { Color.FromArgb(60, 15, 35), Color.FromArgb(128, 35, 75), Color.FromArgb(240, 98, 146) }, // Pink
        new[] { Color.FromArgb(50, 38, 0), Color.FromArgb(106, 80, 0), Color.FromArgb(255, 213, 79) }, // Amber
        new[] { Color.FromArgb(0, 47, 53), Color.FromArgb(0, 99, 112), Color.FromArgb(38, 198, 218) } // Cyan
        };

        private static int _indiceAtual = 0;

        public static void AplicarCores(params Panel[] panels)
        {
            if (panels == null || panels.Length == 0) return;

            // Pega 3 cores a partir do índice atual, com wrap
            for (int i = 0; i < panels.Length; i++)
            {
                var corIndex = (_indiceAtual + i) % Cores.Length;
                panels[i].BackColor = Cores[corIndex];
            }

            // Avança 1 posição pro próximo load
            _indiceAtual = (_indiceAtual + 1) % Cores.Length;

            LogServices.LogarDebug("Cores aplicadas. Próximo índice: {indice}", _indiceAtual);
        }
        public static void Resetar()
        {
            _indiceAtual = 0;
        }
    }
}
