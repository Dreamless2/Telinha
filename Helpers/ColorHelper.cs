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
        [
            Color.FromArgb(45, 45, 48), // Cinza escuro
            Color.FromArgb(0, 122, 204), // Azul VS
            Color.FromArgb(16, 124, 16) // Verde VS
        ];

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
