using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    public static class TagEngine
    {
        private static readonly Dictionary<string, string> GeneroMapeado = new()
        {
            { "ficção científica", "ficcaocientifica ficçãocientífica" },
            { "ficçãocientíficaefantasia", "ficcaocientificaefantasia ficçãocientíficaefantasia" },
            { "ficção científica e aventura", "ficcaocientificaeaventura ficçãocientíficaeaventura" },
            { "romântico", "romantico romântico" },
            { "romântica", "romantica romântica" },
            { "comédia", "comedia comédia" },
            { "mistério", "misterio mistério" },
            { "ação", "acao ação" },
            { "ação e fantasia", "acaoefantasia açãoefantasia" },
            { "ação e aventura", "acaoeaventura açãoeaventura" },
            { "animação", "animacao animação" },
            { "documentário", "documentario documentário" },
            { "comédia dramática", "comediadramatica comédiadramática" },
            { "comédia romântica", "comediaromantica comédiaromântica" },
            { "ficção científica e fantasia", "ficcaocientificaefantasia ficçãocientíficaefantasia" },
            { "ficção científica e ação", "ficcaocientificaeacao ficçãocientíficaeação" },
            { "ficção científica e comédia", "ficcaocientificaecomedia ficçãocientíficaecomédia" },
            { "ficção científica e drama", "ficcaocientificaedrama ficçãocientíficaedrama" },
            { "ficção científica e mistério", "ficcaocientificaemisterio ficçãocientíficaemistério" },
            { "ficção científica e romance", "ficcaocientificaeromance ficçãocientíficaeromance" },
            { "ficção científica e terror", "ficcaocientificaeterror ficçãocientíficaeterror" }
        };

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
            {
                return string.Empty;
            }

            var semEspacos = titulo.Replace(" ", "");
            semEspacos = Regex.Replace(semEspacos, @"[^\w\d]", "");

            if (string.IsNullOrWhiteSpace(semEspacos))
            {
                return string.Empty;
            }

            var comAcento = char.ToUpper(semEspacos[0]) + semEspacos[1..];
            var semAcento = RemoverAcentos(comAcento);

            return semAcento.Equals(comAcento, StringComparison.Ordinal)
                ? $"#{semAcento}"
                : $"#{semAcento} #{comAcento}";
        }
    }
}