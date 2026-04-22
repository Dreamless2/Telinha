using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    public static partial class TagEngine
    {
        // Regex compilado (melhor performance)
        private static readonly Regex RegexLimpeza = new(@"[^a-zA-Z0-9]", RegexOptions.Compiled);

        // Cache para remover acentos (evita recomputação)
        private static readonly Dictionary<string, string> CacheAcentos = new();

        // Dicionário já NORMALIZADO (sem acento e sem espaço)
        private static readonly Dictionary<string, string[]> GeneroMapeado = new()
    {
        { "ficcaocientifica", new[] { "ficcaocientifica", "ficçãocientífica" } },
        { "ficcaocientificaefantasia", new[] { "ficcaocientificaefantasia", "ficçãocientíficaefantasia" } },
        { "ficcaocientificaeaventura", new[] { "ficcaocientificaeaventura", "ficçãocientíficaeaventura" } },
        { "romantico", new[] { "romantico", "romântico" } },
        { "romantica", new[] { "romantica", "romântica" } },
        { "comedia", new[] { "comedia", "comédia" } },
        { "misterio", new[] { "misterio", "mistério" } },
        { "acao", new[] { "acao", "ação" } },
        { "acaoefantasia", new[] { "acaoefantasia", "açãoefantasia" } },
        { "acaoeaventura", new[] { "acaoeaventura", "açãoeaventura" } },
        { "animacao", new[] { "animacao", "animação" } },
        { "documentario", new[] { "documentario", "documentário" } },
        { "comediadramatica", new[] { "comediadramatica", "comédiadramática" } },
        { "comediaromantica", new[] { "comediaromantica", "comédiaromântica" } },
        { "ficcaocientificaeacao", new[] { "ficcaocientificaeacao", "ficçãocientíficaeação" } },
        { "ficcaocientificaecomedia", new[] { "ficcaocientificaecomedia", "ficçãocientíficaecomédia" } },
        { "ficcaocientificaedrama", new[] { "ficcaocientificaedrama", "ficçãocientíficaedrama" } },
        { "ficcaocientificaemisterio", new[] { "ficcaocientificaemisterio", "ficçãocientíficaemistério" } },
        { "ficcaocientificaeromance", new[] { "ficcaocientificaeromance", "ficçãocientíficaeromance" } },
        { "ficcaocientificaeterror", new[] { "ficcaocientificaeterror", "ficçãocientíficaeterror" } }
    };

        public static string NormalizarGeneros(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada))
                return string.Empty;

            var hashTags = new HashSet<string>();

            var generos = entrada.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(g => g.Trim());

            foreach (var generoOriginal in generos)
            {
                var chave = NormalizarChave(generoOriginal);

                if (GeneroMapeado.TryGetValue(chave, out var formas))
                {
                    foreach (var forma in formas)
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
                return string.Empty;

            var nomes = texto.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(n => RemoverAcentos(n.Trim()))
                             .Select(n => RegexLimpeza.Replace(n, ""))
                             .Where(n => !string.IsNullOrWhiteSpace(n))
                             .Select(n => $"#{n}");

            return string.Join(" ", nomes);
        }

        public static string FormatarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return string.Empty;

            var semEspacos = RegexLimpeza.Replace(titulo.Replace(" ", ""), "");

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

        private static string NormalizarChave(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            texto = RemoverAcentos(texto);
            texto = texto.ToLowerInvariant().Replace(" ", "");

            return texto;
        }

        public static string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            if (CacheAcentos.TryGetValue(texto, out var cached))
                return cached;

            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            var resultado = sb.ToString().Normalize(NormalizationForm.FormC);
            CacheAcentos[texto] = resultado;

            return resultado;
        }
    }
}