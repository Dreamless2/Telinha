using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    public static partial class TagEngine
    {
        [GeneratedRegex(@"[^\p{L}\p{Nd}]", RegexOptions.NonBacktracking)]
        private static partial Regex NonAlphaNumericRegex();

        [GeneratedRegex(@"\s+", RegexOptions.NonBacktracking)]
        private static partial Regex MultiSpaceRegex();

        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "e", "and", "de", "da", "do", "das", "dos"
    };

        // =========================
        // 🎬 GÊNEROS
        // =========================
        public static IEnumerable<string> NormalizarGeneros(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada))
                yield break;

            foreach (var genero in entrada.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var original = LimparTexto(genero, manterAcento: true);
                if (original.Length == 0)
                    continue;

                var semAcento = RemoverAcentos(original);

                yield return $"#{Compactar(original)}";
                yield return $"#{Compactar(semAcento)}";

                foreach (var palavra in original.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (palavra.Length < 3 || StopWords.Contains(palavra))
                        continue;

                    yield return $"#{palavra}";
                    yield return $"#{RemoverAcentos(palavra)}";
                }
            }
        }

        // =========================
        // 🏷️ NOMES / TAGS
        // =========================
        public static IEnumerable<string> GerarTags(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                yield break;

            foreach (var item in texto.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var tag in FormatarTitulo(item))
                    yield return tag;
            }
        }

        // =========================
        // 🎯 TÍTULO (SEU ORIGINAL)
        // =========================
        public static IEnumerable<string> FormatarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                yield break;

            var semEspacos = titulo.Replace(" ", "");
            semEspacos = NonAlphaNumericRegex().Replace(semEspacos, "");

            if (semEspacos.Length == 0)
                yield break;

            var comAcento = char.ToUpperInvariant(semEspacos[0]) + semEspacos[1..];
            var semAcento = RemoverAcentos(comAcento);

            yield return $"#{semAcento}";

            if (!semAcento.Equals(comAcento, StringComparison.Ordinal))
                yield return $"#{comAcento}";
        }

        // =========================
        // 🔹 HELPERS
        // =========================
        private static string LimparTexto(string texto, bool manterAcento)
        {
            var t = texto.Trim().ToLowerInvariant();

            t = MultiSpaceRegex().Replace(t, " ");

            if (!manterAcento)
                t = RemoverAcentos(t);

            t = NonAlphaNumericRegex().Replace(t, " ");
            t = MultiSpaceRegex().Replace(t, " ");

            return t.Trim();
        }

        private static string Compactar(string texto)
            => texto.Replace(" ", "");

        public static string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(texto.Length);

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}