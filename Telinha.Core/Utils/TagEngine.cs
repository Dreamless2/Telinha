using System.Buffers;
using System.Collections.Frozen;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Core.Utils
{
    public class TagEngine
    {
        private static readonly FrozenDictionary<string, string> GeneroMapeado =
            BuildGeneroMap();

        private static FrozenDictionary<string, string> BuildGeneroMap()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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
            };

            return dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        }

        // ---------------- FAST CORE ----------------

        public static string NormalizarGeneros(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada))
                return string.Empty;

            var tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            ReadOnlySpan<char> span = entrada.AsSpan();

            int start = 0;

            for (int i = 0; i <= span.Length; i++)
            {
                if (i == span.Length || span[i] == ',')
                {
                    var slice = span[start..i].Trim();

                    if (!slice.IsEmpty)
                    {
                        ProcessGenero(slice, tags);
                    }

                    start = i + 1;
                }
            }

            return string.Join(' ', tags);
        }

        private static void ProcessGenero(ReadOnlySpan<char> genero, HashSet<string> tags)
        {
            Span<char> buffer = stackalloc char[128];
            int len = NormalizeFast(genero, buffer);

            var chave = buffer[..len];

            // remove espaços
            Span<char> compact = stackalloc char[len];
            int j = 0;

            for (int i = 0; i < len; i++)
                if (chave[i] != ' ')
                    compact[j++] = chave[i];

            var final = compact[..j].ToString();

            if (GeneroMapeado.TryGetValue(final, out var formas))
            {
                foreach (var f in formas.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    tags.Add("#" + f);
            }
            else
            {
                tags.Add("#" + final);
            }
        }

        // ---------------- FAST TAG GENERATION ----------------

        public static string GerarTags(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            ReadOnlySpan<char> span = texto.AsSpan();

            int start = 0;

            for (int i = 0; i <= span.Length; i++)
            {
                if (i == span.Length || span[i] == ',')
                {
                    var slice = span[start..i].Trim();

                    if (!slice.IsEmpty)
                    {
                        ProcessTag(slice, tags);
                    }

                    start = i + 1;
                }
            }

            return string.Join(' ', tags);
        }

        private static void ProcessTag(ReadOnlySpan<char> text, HashSet<string> tags)
        {
            Span<char> buffer = stackalloc char[128];

            int len = NormalizeFast(text, buffer);

            Span<char> clean = buffer[..len];

            int j = 0;
            for (int i = 0; i < len; i++)
            {
                char c = clean[i];

                if (IsAlphaNum(c))
                    clean[j++] = c;
            }

            if (j == 0)
                return;

            tags.Add("#" + clean[..j].ToString());
        }

        // ---------------- FORMAT TITLE ----------------

        public static string FormatarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return string.Empty;

            Span<char> buffer = stackalloc char[128];

            int len = NormalizeFast(titulo.AsSpan(), buffer);

            Span<char> clean = buffer[..len];

            int j = 0;

            for (int i = 0; i < len; i++)
            {
                char c = clean[i];

                if (IsAlphaNumOrSpace(c))
                    clean[j++] = c;
            }

            if (j == 0)
                return string.Empty;

            var result = clean[..j].ToString();

            if (result.Length == 0)
                return string.Empty;

            var first = char.ToUpper(result[0]) + result[1..];

            var semAcento = FastAccentRemover.RemoverAcentos(first);

            return semAcento.Equals(first, StringComparison.Ordinal)
                ? $"#{semAcento}"
                : $"#{semAcento} #{first}";
        }

        // ---------------- FAST NORMALIZER ----------------

        private static int NormalizeFast(ReadOnlySpan<char> input, Span<char> output)
        {
            int len = input.Length;

            for (int i = 0; i < len; i++)
            {
                char c = input[i];
                output[i] = FastAccentRemover.MapChar(c);
            }

            return len;
        }

        private static bool IsAlphaNum(char c)
            => (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               (c >= '0' && c <= '9') ||
               c >= 128;

        private static bool IsAlphaNumOrSpace(char c)
            => IsAlphaNum(c) || c == ' ';

        private static string P(string comAcento)
        {
            var sem = FastAccentRemover.RemoverAcentos(comAcento);

            return sem == comAcento
                ? sem
                : $"{sem} {comAcento}";
        }
    }

    // 🔥 upgrade do remover acento para inline mapping
    public static class FastAccentRemover
    {
        private static readonly char[] Map = CreateMap();

        private static char[] CreateMap()
        {
            var map = new char[256];

            for (int i = 0; i < map.Length; i++)
                map[i] = (char)i;

            map['á'] = map['à'] = map['ã'] = map['â'] = map['ä'] = 'a';
            map['é'] = map['è'] = map['ê'] = map['ë'] = 'e';
            map['í'] = map['ì'] = map['î'] = map['ï'] = 'i';
            map['ó'] = map['ò'] = map['õ'] = map['ô'] = map['ö'] = 'o';
            map['ú'] = map['ù'] = map['û'] = map['ü'] = 'u';
            map['ç'] = 'c';

            map['Á'] = map['À'] = map['Ã'] = map['Â'] = map['Ä'] = 'A';
            map['É'] = map['È'] = map['Ê'] = map['Ë'] = 'E';
            map['Í'] = map['Ì'] = map['Î'] = map['Ï'] = 'I';
            map['Ó'] = map['Ò'] = map['Õ'] = map['Ô'] = map['Ö'] = 'O';
            map['Ú'] = map['Ù'] = map['Û'] = map['Ü'] = 'U';
            map['Ç'] = 'C';

            return map;
        }

        public static char MapChar(char c)
            => c < 256 ? Map[c] : c;

        public static string RemoverAcentos(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return string.Create(input.Length, input, static (dest, src) =>
            {
                var span = src.AsSpan();

                for (int i = 0; i < span.Length; i++)
                    dest[i] = MapChar(span[i]);
            });
        }
    }
}