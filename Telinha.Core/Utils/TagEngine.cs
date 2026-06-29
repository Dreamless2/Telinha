using System.Buffers;
using System.Collections.Frozen;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Core.Utils
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
        /*public static string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            var normalized = texto.Normalize(NormalizationForm.FormD);
            var span = normalized.AsSpan();

            if (span.IndexOfAny(NonSpacingMarks) == -1)
                return texto;

            return string.Create(normalized.Length, normalized, (dest, src) =>
            {
                var destIdx = 0;
                foreach (var c in src)
                {
                    if (!NonSpacingMarks.Contains(c))
                        dest[destIdx++] = c;
                }
            })[..^0].Normalize(NormalizationForm.FormC);
        }*/

        public static class FastAccentRemover
        {
            // Tabela simples para Latin-1 estendido (0–255)
            // Só cobre o que interessa na maioria dos sistemas PT-BR/ES
            private static readonly char[] Map = CreateMap();

            private static char[] CreateMap()
            {
                var map = new char[256];

                for (int i = 0; i < map.Length; i++)
                    map[i] = (char)i;

                // PT-BR / ES principais
                map['á'] = 'a'; map['à'] = 'a'; map['ã'] = 'a'; map['â'] = 'a';
                map['ä'] = 'a'; map['å'] = 'a';

                map['é'] = 'e'; map['è'] = 'e'; map['ê'] = 'e'; map['ë'] = 'e';

                map['í'] = 'i'; map['ì'] = 'i'; map['î'] = 'i'; map['ï'] = 'i';

                map['ó'] = 'o'; map['ò'] = 'o'; map['õ'] = 'o'; map['ô'] = 'o'; map['ö'] = 'o';

                map['ú'] = 'u'; map['ù'] = 'u'; map['û'] = 'u'; map['ü'] = 'u';

                map['ç'] = 'c';

                // maiúsculas
                map['Á'] = 'A'; map['À'] = 'A'; map['Ã'] = 'A'; map['Â'] = 'A'; map['Ä'] = 'A';
                map['É'] = 'E'; map['È'] = 'E'; map['Ê'] = 'E'; map['Ë'] = 'E';
                map['Í'] = 'I'; map['Ì'] = 'I'; map['Î'] = 'I'; map['Ï'] = 'I';
                map['Ó'] = 'O'; map['Ò'] = 'O'; map['Õ'] = 'O'; map['Ô'] = 'O'; map['Ö'] = 'O';
                map['Ú'] = 'U'; map['Ù'] = 'U'; map['Û'] = 'U'; map['Ü'] = 'U';
                map['Ç'] = 'C';

                return map;
            }

            public static string RemoverAcentos(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return input;

                return string.Create(input.Length, input, static (dest, src) =>
                {
                    ReadOnlySpan<char> span = src.AsSpan();

                    for (int i = 0; i < span.Length; i++)
                    {
                        char c = span[i];

                        dest[i] = c < 256 ? Map[c] : c;
                    }
                });
            }
        }

        public static string FormatarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return string.Empty;

            var apenasTexto = RegexNaoAlfaNumEspaco.Replace(titulo, "");
            var semEspacos = apenasTexto.Replace(" ", "");

            if (semEspacos.Length == 0)
                return string.Empty;

            var comAcento = semEspacos.Length > 1
               ? char.ToUpper(semEspacos[0]) + semEspacos[1..]
                : semEspacos.ToUpperInvariant();

            var semAcento = RemoverAcentos(comAcento);

            return semAcento.Equals(comAcento, StringComparison.Ordinal)
                ? $"#{semAcento}"
                : $"#{semAcento} #{comAcento}";
        }
    }
}