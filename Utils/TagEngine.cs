using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    public static partial class TagEngine
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "e", "and", "de", "da", "do", "das", "dos"
        };

        // 🔥 GeneratedRegex (resolve SYSLIB1045)
        [GeneratedRegex(@"[^\p{L}\p{Nd}]")]
        private static partial Regex NonAlphaNumericRegex();

        [GeneratedRegex(@"\s+")]
        private static partial Regex MultiSpaceRegex();

        // =========================
        // 🚀 PRINCIPAL
        // =========================
        public static string NormalizarGeneros(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada))
                return string.Empty;

            var tags = new HashSet<string>(StringComparer.Ordinal);

            foreach (var genero in entrada.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var original = LimparTexto(genero, manterAcento: true);
                if (original.Length == 0) continue;

                var semAcento = RemoverAcentos(original);

                var compactoOriginal = Compactar(original);
                var compactoSemAcento = Compactar(semAcento);

                AddTag(tags, compactoOriginal);
                AddTag(tags, compactoSemAcento);

                // 🔥 explode palavras
                foreach (var palavra in original.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (palavra.Length < 3 || StopWords.Contains(palavra))
                        continue;

                    AddTag(tags, palavra);

                    var pSemAcento = RemoverAcentos(palavra);
                    AddTag(tags, pSemAcento);
                }
            }

            return string.Join(' ', tags);
        }
        public static string GerarTags(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var tags = new HashSet<string>(StringComparer.Ordinal);

            foreach (var item in texto.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var tag in GerarTagsNomeComposto(item))
                {
                    tags.Add(tag);
                }
            }

            return string.Join(' ', tags);
        }

        public static string FormatarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return string.Empty;

            var original = LimparTexto(titulo, manterAcento: true);
            if (original.Length == 0)
                return string.Empty;

            var palavras = original.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();

            foreach (var palavra in palavras)
            {
                sb.Append(Capitalizar(palavra));
            }

            var comAcento = sb.ToString();
            var semAcento = RemoverAcentos(comAcento);

            return semAcento.Equals(comAcento, StringComparison.Ordinal)
                ? $"#{comAcento}"
                : $"#{semAcento.ToLowerInvariant()} #{comAcento}";
        }

        // =========================
        // 🔹 Helpers
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

        private static void AddTag(HashSet<string> tags, string valor)
        {
            if (!string.IsNullOrEmpty(valor))
                tags.Add($"#{valor}");
        }

        private static string Capitalizar(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            return char.ToUpperInvariant(texto[0]) + texto[1..];
        }

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
        public static IEnumerable<string> GerarTagsNomeComposto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                yield break;

            var original = LimparTexto(texto, manterAcento: true);
            if (original.Length == 0)
                yield break;

            var palavras = original.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 🔥 mantém todas palavras (inclusive stopwords)
            var pascal = new StringBuilder();
            var lower = new StringBuilder();

            foreach (var palavra in palavras)
            {
                if (palavra.Length == 0)
                    continue;

                pascal.Append(Capitalizar(palavra));
                lower.Append(palavra);
            }

            var comAcento = pascal.ToString().ToUpper();
            var semAcento = RemoverAcentos(comAcento);

            yield return $"#{comAcento}";
            yield return $"#{semAcento}";
        }
    }
}
