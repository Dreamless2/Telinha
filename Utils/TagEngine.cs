using System.Buffers;
using System.Collections.Frozen;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    public class TagEngine
    {
        private static readonly Regex RegexNaoAlfaNum = new(@"[^\p{L}\p{Nd}]", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex RegexNaoAlfaNumEspaco = new(@"[^\p{L}\p{Nd} ]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        // .NET 8+ SearchValues - remove acento sem alocação de categoria
        private static readonly SearchValues<char> NonSpacingMarks = SearchValues.Create(
            "\u0300\u0301\u0302\u0303\u0304\u0305\u0306\u0307\u0308\u0309\u030A\u030B\u030C\u030D\u030E\u030F" +
            "\u0310\u0311\u0312\u0313\u0314\u0315\u0316\u0317\u0318\u0319\u031A\u031B\u031C\u031D\u031E\u031F" +
            "\u0320\u0321\u0322\u0323\u0324\u0325\u0326\u0327\u0328\u0329\u032A\u032B\u032C\u032D\u032E\u032F");

        private static readonly FrozenDictionary<string, string> GeneroMapeado = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ficçãocientífica"] = P("ficçãocientífica"),
            ["ficçãocientíficaefantasia"] = P("ficçãocientíficaefantasia"),
            ["ficçãocientíficaeaventura"] = P("ficçãocientíficaeaventura"),
            ["ficçãocientíficaeação"] = P("ficçãocientíficaeação"),
            ["ficçãocientíficaecomédia"] = P("ficçãocientíficaecomédia"),
            ["ficçãocientíficaedrama"] = P("ficçãocientíficaedrama"),
            ["ficçãocientíficaemistério"] = P("ficçãocientíficaemistério"),
            ["ficçãocientíficaeromance"] = P("ficçãocientíficaeromance"),
            ["ficçãocientíficaeterror"] = P("ficçãocientíficaeterror"),
            ["romântico"] = P("romântico"),
            ["romântica"] = P("romântica"),
            ["comédia"] = P("comédia"),
            ["comédiadramática"] = P("comédiadramática"),
            ["comédiaromântica"] = P("comédiaromântica"),
            ["mistério"] = P("mistério"),
            ["ação"] = P("ação"),
            ["açãoefantasia"] = P("açãoefantasia"),
            ["açãoeaventura"] = P("açãoeaventura"),
            ["animação"] = P("animação"),
            ["documentário"] = P("documentário")
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        private static string P(string comAcento)
        {
            var semAcento = RemoverAcentos(comAcento);
            return semAcento.Equals(comAcento, StringComparison.Ordinal)
                ? semAcento
                : $"{semAcento} {comAcento}";
        }

        public static string NormalizarGeneros(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada))
                return string.Empty;

            var hashTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var genero in entrada.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var chave = genero.Replace(" ", "");

                if (GeneroMapeado.TryGetValue(chave, out var formas))
                {
                    foreach (var forma in formas.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                        hashTags.Add($"#{forma}");
                }
                else
                {
                    var semAcento = RemoverAcentos(chave);
                    hashTags.Add($"#{semAcento}");
                    if (!semAcento.Equals(chave, StringComparison.OrdinalIgnoreCase))
                        hashTags.Add($"#{chave}");
                }
            }

            return string.Join(' ', hashTags);
        }

        public static string GerarTags(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return string.Join(' ', texto
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(RemoverAcentos)
                .Select(n => RegexNaoAlfaNum.Replace(n, ""))
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => $"#{n}"));
        }


        public static string RemoverAcentos(string texto)
        {
            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string FormatarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return string.Empty;

            // 🔹 Limpa mantendo acento
            var apenasTexto = Regex.Replace(titulo, @"[^\p{L}\p{Nd} ]", "");
            var semEspacos = apenasTexto.Replace(" ", "");

            if (string.IsNullOrWhiteSpace(semEspacos))
                return string.Empty;

            // 🔹 Versão com acento (preservada)
            var comAcento = semEspacos.Length > 1
                ? char.ToUpper(semEspacos[0]) + semEspacos[1..]
                : semEspacos.ToUpper();

            // 🔹 Versão sem acento (derivada)
            var semAcento = RemoverAcentos(comAcento);

            // 🔥 GARANTE separação REAL
            if (semAcento.Equals(comAcento, StringComparison.Ordinal))
                return $"#{semAcento}";

            return string.Concat("#", semAcento, " ", "#", comAcento);
        }
    }
}