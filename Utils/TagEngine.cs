using System.Collections.Frozen;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    public class TagEngine
    {
        // Regex compilado 1x só
        private static readonly Regex RegexNaoAlfaNum = new(@"[^\p{L}\p{Nd}]", RegexOptions.Compiled);
        private static readonly Regex RegexNaoAlfaNumEspaco = new(@"[^\p{L}\p{Nd} ]", RegexOptions.Compiled);

        private static readonly FrozenDictionary<string, string> GeneroMapeado = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ficçãocientífica"] = P("ficçãocientífica"),
            ["ficçãocientíficaefantasia"] = P("ficçãocientíficaefantasia"),
            ["ficçãocientíficaeaventura"] = P("ficçãocientíficaeaventura"),
            ["romântico"] = P("romântico"),
            ["romântica"] = P("romântica"),
            ["comédia"] = P("comédia"),
            ["mistério"] = P("mistério"),
            ["ação"] = P("ação"),
            ["açãoefantasia"] = P("açãoefantasia"),
            ["açãoeaventura"] = P("açãoeaventura"),
            ["animação"] = P("animação"),
            ["documentário"] = P("documentário"),
            ["comédiadramática"] = P("comédiadramática"),
            ["comédiaromântica"] = P("comédiaromântica"),
        }.ToFrozenDictionary();

        private static string P(string s) => $"{RemoverAcentos(s)} {s}".Replace($"{s} {s}", s);

        public static string NormalizarGeneros(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada))
                return string.Empty;

            var hashTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var genero in entrada.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var chave = genero.Replace(" ", "").ToLowerInvariant();

                if (GeneroMapeado.TryGetValue(chave, out var formas))
                {
                    foreach (var forma in formas.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                        hashTags.Add($"#{forma}");
                }
                else
                {
                    var semEspaco = chave;
                    var semAcento = RemoverAcentos(semEspaco);

                    hashTags.Add($"#{semAcento}");
                    if (!semAcento.Equals(semEspaco, StringComparison.Ordinal))
                        hashTags.Add($"#{semEspaco}");
                }
            }

            return string.Join(" ", hashTags);
        }

        public static string GerarTags(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return string.Join(" ", texto
               .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
               .Select(n => RemoverAcentos(n))
               .Select(n => RegexNaoAlfaNum.Replace(n, ""))
               .Where(n => !string.IsNullOrWhiteSpace(n))
               .Select(n => $"#{n}"));
        }

        public static string FormatarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return string.Empty;

            var apenasTexto = RegexNaoAlfaNumEspaco.Replace(titulo, "");
            var semEspacos = apenasTexto.Replace(" ", "");

            if (string.IsNullOrWhiteSpace(semEspacos))
                return string.Empty;

            var comAcento = semEspacos.Length > 1
               ? char.ToUpper(semEspacos[0]) + semEspacos[1..]
                : semEspacos.ToUpper();

            var semAcento = RemoverAcentos(comAcento);

            return semAcento.Equals(comAcento, StringComparison.Ordinal)
               ? $"#{semAcento}"
                : $"#{semAcento} #{comAcento}";
        }

        public static string RemoverAcentos(string texto)
        {
            var normalized = texto.Normalize(NormalizationForm.FormD);
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