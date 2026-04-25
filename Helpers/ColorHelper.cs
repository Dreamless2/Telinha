using System;
using System.Collections.Generic;
using System.Text;
using Telinha.Services;

namespace Telinha.Helpers
{
    public class ColorHelper
    {

        // 🔥 Arreio de 3 cores. Troca os RGB aí
        private static readonly Color[][] Paletas =
        {
            // 1. Blue
            new[] { Color.FromArgb(0, 30, 60), Color.FromArgb(0, 74, 127), Color.FromArgb(100, 181, 246) },
            // 2. Green
            new[] { Color.FromArgb(0, 50, 28), Color.FromArgb(0, 105, 61), Color.FromArgb(129, 199, 132) },
            // 3. Purple
            new[] { Color.FromArgb(49, 27, 63), Color.FromArgb(103, 58, 135), Color.FromArgb(186, 104, 200) },
            // 4. Teal
            new[] { Color.FromArgb(0, 45, 45), Color.FromArgb(0, 95, 95), Color.FromArgb(77, 182, 172) },
            // 5. Orange
            new[] { Color.FromArgb(62, 31, 0), Color.FromArgb(130, 65, 0), Color.FromArgb(255, 167, 38) },
            // 6. Red
            new[] { Color.FromArgb(60, 0, 0), Color.FromArgb(127, 0, 0), Color.FromArgb(239, 83, 80) },
            // 7. Indigo
            new[] { Color.FromArgb(26, 35, 67), Color.FromArgb(57, 73, 138), Color.FromArgb(121, 134, 203) },
            // 8. Pink
            new[] { Color.FromArgb(60, 15, 35), Color.FromArgb(128, 35, 75), Color.FromArgb(240, 98, 146) },
            // 9. Amber
            new[] { Color.FromArgb(50, 38, 0), Color.FromArgb(106, 80, 0), Color.FromArgb(255, 213, 79) },
            // 10. Cyan
            new[] { Color.FromArgb(0, 47, 53), Color.FromArgb(0, 99, 112), Color.FromArgb(38, 198, 218) }
        };

        private static int _indiceAtual = 0;

        public static void AplicarCores(params Panel[] panels)
        {
            if (panels == null || panels.Length == 0) return;

            // 🔥 Pega a paleta atual - que é Color[]
            var paletaAtual = Paletas[_indiceAtual];

            // Aplica até 3 cores, se tiver menos panel usa só o que tem
            var limite = Math.Min(panels.Length, paletaAtual.Length);

            for (int i = 0; i < limite; i++)
            {
                panels[i].BackColor = paletaAtual[i]; // 🔥 Agora é Color, não Color[]
            }

            _indiceAtual = (_indiceAtual + 1) % Paletas.Length;
        }
        public static void Resetar()
        {
            _indiceAtual = 0;
        }

        private static readonly string[] NomesPaletas =
      { "Blue", "Green", "Purple", "Teal", "Orange", "Red", "Indigo", "Pink", "Amber", "Cyan" };

        public static string NomePaletaAtual => NomesPaletas[_indiceAtual];
    }
}
