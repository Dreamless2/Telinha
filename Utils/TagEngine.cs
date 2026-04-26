using System.Buffers;
using System.Collections.Frozen;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    public class TagEngine
    {
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

        // Gera "semAcento comAcento" automático. Se for igual, retorna só 1
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
            {
                return string.Empty;
            }

            var hashTags = new HashSet<string>();

            var generos = entrada.Split(',')
                                 .Select(g => g.Trim().ToLowerInvariant());

            foreach (var generoOriginal in generos)
            {
                var chave = generoOriginal.Replace(" ", "");

                if (GeneroMapeado.TryGetValue(chave, out var formas))
                {
                    foreach (var forma in formas.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        hashTags.Add($"#{forma}");
                    }
                }
                else
                {
                    var semEspaco = chave;
                    var semAcento = RemoverAcentos(semEspaco);

                    hashTags.Add($"#{semAcento}");
                    hashTags.Add($"#{semEspaco}");
                }
            }

            return string.Join(" ", hashTags);
        }

        public static string GerarTags(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

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