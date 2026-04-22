using System.Collections.Frozen;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    public static partial class TagEngine
    {
        public static string NormalizarGeneros(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada))
                return string.Empty;

            var hashTags = new HashSet<string>();

            var generos = entrada.Split(',')
                                 .Select(g => g.Trim().ToLowerInvariant());

            foreach (var generoOriginal in generos)
            {
                var chave = generoOriginal.Replace(" ", "");

                var semAcento = RemoverAcentos(chave);

                hashTags.Add($"#{semAcento}");
                hashTags.Add($"#{chave}");
            }

            return string.Join(" ", hashTags);
        }

        public static string GerarTags(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var nomes = texto.Split(',')
                             .Select(n => RemoverAcentos(n.Trim()))
                             .Select(n => Regex.Replace(n, @"[^a-zA-Z0-9]", ""))
                             .Where(n => !string.IsNullOrWhiteSpace(n))
                             .Select(n => $"#{n}");

            return string.Join(" ", nomes);
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

        public static string FormatarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return string.Empty;

            var semEspacos = titulo.Replace(" ", "");
            semEspacos = Regex.Replace(semEspacos, @"[^\w\d]", "");

            if (string.IsNullOrWhiteSpace(semEspacos))
                return string.Empty;

            var comAcento = char.ToUpper(semEspacos[0]) + semEspacos[1..];
            var semAcento = RemoverAcentos(comAcento);

            return semAcento.Equals(comAcento, StringComparison.Ordinal)
                ? $"#{semAcento}"
                : $"#{semAcento} #{comAcento}";
        }
    }
}