using System.Collections.Frozen;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    public static partial class TagEngine
    {
        // FrozenDictionary é otimizado para buscas rápidas em coleções que não mudam
        private static readonly FrozenDictionary<string, string[]> GeneroMapeado = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["ficção científica"] = ["ficcaocientifica", "ficçãocientífica"],
            ["romântico"] = ["romantico", "romântico"],
            ["ação"] = ["acao", "ação"],
            ["comédia"] = ["comedia", "comédia"],
            // Use a sintaxe de coleção [ ] do C# 12+
        }.ToFrozenDictionary();

        // Source Generator: O Regex é gerado em tempo de compilação (Performance máxima)
        [GeneratedRegex(@"[^a-zA-Z0-9]")]
        private static partial Regex LimparTextoRegex();

        public static string NormalizarGeneros(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada)) return string.Empty;

            var hashTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Split direto com Span-based parsing por baixo dos panos
            foreach (var parte in entrada.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                var chaveSemEspaco = parte.Replace(" ", "");

                if (GeneroMapeado.TryGetValue(parte, out var formas) ||
                    GeneroMapeado.TryGetValue(chaveSemEspaco, out formas))
                {
                    foreach (var forma in formas) hashTags.Add($"#{forma}");
                }
                else
                {
                    hashTags.Add($"#{RemoverAcentos(chaveSemEspaco)}");
                    hashTags.Add($"#{chaveSemEspaco}");
                }
            }

            return string.Join(" ", hashTags);
        }

        public static string GerarTags(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            return string.Join(" ", texto
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(n => $"#{LimparTextoRegex().Replace(RemoverAcentos(n), "")}")
                .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        public static string FormatarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo)) return string.Empty;

            var limpo = LimparTextoRegex().Replace(titulo, "");
            if (string.IsNullOrEmpty(limpo)) return string.Empty;

            // Capitalização moderna
            string comAcento = string.Concat(char.ToUpperInvariant(limpo[0]), limpo[1..]);
            string semAcento = RemoverAcentos(comAcento);

            return semAcento == comAcento ? $"#{semAcento}" : $"#{semAcento} #{comAcento}";
        }

        public static string RemoverAcentos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return texto;

            // Normalização eficiente
            ReadOnlySpan<char> normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}