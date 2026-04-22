using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class Cleanser
    {
        private static readonly Regex NonAlphaNumericRegex = new(@"[^\p{L}\p{Nd}]", RegexOptions.Compiled);
        private static readonly Regex MultiSpaceRegex = new(@"\s+", RegexOptions.Compiled);

        private static readonly HashSet<string> StopWords = new()
    {
        "e", "and", "de", "da", "do", "das", "dos"
    };

        // 🔥 PRINCIPAL
        public static string NormalizarGeneros(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada))
                return string.Empty;

            var tags = new HashSet<string>();

            foreach (var genero in entrada.Split(','))
            {
                var original = LimparTexto(genero, manterAcento: true);
                var semAcento = RemoverAcentos(original);

                var compactoOriginal = Compactar(original);
                var compactoSemAcento = Compactar(semAcento);

                // versão completa
                AddTag(tags, compactoOriginal);
                AddTag(tags, compactoSemAcento);

                // 🔥 versão por palavras (explode gênero composto)
                foreach (var palavra in original.Split(' '))
                {
                    var p = palavra.Trim().ToLowerInvariant();

                    if (StopWords.Contains(p) || p.Length < 3)
                        continue;

                    var pSemAcento = RemoverAcentos(p);

                    AddTag(tags, p);
                    AddTag(tags, pSemAcento);
                }
            }

            return string.Join(" ", tags);
        }

        public static string GerarTags(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var tags = new HashSet<string>();

            foreach (var item in texto.Split(','))
            {
                var limpo = LimparTexto(item, manterAcento: false);

                if (!string.IsNullOrWhiteSpace(limpo))
                    AddTag(tags, limpo);
            }

            return string.Join(" ", tags);
        }

        public static string FormatarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return string.Empty;

            var original = LimparTexto(titulo, manterAcento: true);
            var semAcento = RemoverAcentos(original);

            var compactoOriginal = Compactar(original);
            var compactoSemAcento = Compactar(semAcento);

            if (string.IsNullOrEmpty(compactoOriginal))
                return string.Empty;

            var tags = new HashSet<string>();

            AddTag(tags, Capitalizar(compactoOriginal));
            AddTag(tags, Capitalizar(compactoSemAcento));

            return string.Join(" ", tags);
        }

        // =========================
        // 🔹 Helpers inteligentes
        // =========================

        private static string LimparTexto(string texto, bool manterAcento)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var t = texto.Trim().ToLowerInvariant();

            t = MultiSpaceRegex.Replace(t, " ");

            if (!manterAcento)
                t = RemoverAcentos(t);

            t = NonAlphaNumericRegex.Replace(t, " ");
            t = MultiSpaceRegex.Replace(t, " ").Trim();

            return t;
        }

        private static string Compactar(string texto)
        {
            return texto.Replace(" ", "");
        }

        private static void AddTag(HashSet<string> tags, string valor)
        {
            if (!string.IsNullOrWhiteSpace(valor))
                tags.Add($"#{valor}");
        }

        private static string Capitalizar(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            return char.ToUpper(texto[0]) + texto[1..];
        }

        public static string RemoverAcentos(string texto)
        {
            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}