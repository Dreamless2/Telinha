using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Telinha.Utils
{
    public class TagEngine
    {
        // Armazenamos os nomes limpos para evitar reprocessamento
        private static readonly HashSet<string> GenerosBase = new()
    {
        "ficção científica", "comédia romântica", "ação e aventura" 
        // Adicione apenas os nomes "bonitos" aqui se precisar de uma lista fixa, 
        // ou deixe vazio para aceitar qualquer entrada.
    };

        public static string NormalizarGeneros(string? entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada)) return string.Empty;

            // Usamos um StringBuilder com tamanho inicial estimado para evitar realocações
            var resultado = new StringBuilder(entrada.Length * 2);
            var hashTags = new HashSet<string>();

            // Span-based splitting para evitar alocar arrays de strings
            foreach (var segmento in entrada.AsSpan().EnumerateSplits(','))
            {
                var genero = entrada.AsSpan(segmento).Trim();
                if (genero.IsEmpty) continue;

                // 1. Gerar versão com acento e sem espaços
                string comAcento = genero.ToString().Replace(" ", "").ToLowerInvariant();

                // 2. Gerar versão sem acento e sem espaços
                string semAcento = RemoverAcentos(comAcento);

                hashTags.Add($"#{comAcento}");
                hashTags.Add($"#{semAcento}");
            }

            return string.Join(" ", hashTags);
        }

        private static string RemoverAcentos(string texto)
        {
            // Técnica eficiente usando Normalização de forma D (Decomposition)
            var normalizada = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalizada.Length);

            foreach (var c in normalizada)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                // Mantém apenas o que não é marca de acentuação (NonSpacingMark)
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
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