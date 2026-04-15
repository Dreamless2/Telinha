using Newtonsoft.Json.Linq;
using Telinha.Contracts;
using Telinha.Enums;
using Telinha.Models;
using Telinha.Utils;

namespace Telinha.Factory
{
    public static class MidiaFactory
    {
        private static readonly DEEPLContracts deepl = new();
        public static async Task<MidiaModel> ConstruirMidia(MidiaTipo tipo, JObject json, JObject credits, JObject? alternative = null)
        {
            var item = new MidiaModel
            {
                Tipo = tipo.ToString()
            };

            // 1. MAPEAMENTO DE CAMPOS (TMDB diferencia Filmes de TV/Anime)
            bool isTV = tipo != MidiaTipo.Filme;
            string titleField = isTV ? "name" : "title";
            string dateField = isTV ? "first_air_date" : "release_date";
            string originalTitleField = isTV ? "original_name" : "original_title";

            // 2. TRATAMENTO DE DATA E TAGS
            var releaseDateStr = json[dateField]?.ToString();
            bool hasValidDate = DateTime.TryParse(releaseDateStr, out DateTime releaseDate);

            item.Lancamento = hasValidDate ? releaseDate.ToString("dd/MM/yyyy") : "--";

            // Gera tags como: #Filme #Filme2024 ou #Anime #Anime2024
            string tagBase = tipo.ToString();
            item.Tags = hasValidDate ? $"#{tagBase} #{tagBase}{releaseDate.Year}" : $"#{tagBase}";

            // 3. INFORMAÇÕES BÁSICAS
            item.Titulo = json[titleField]?.ToString() ?? "--";
            item.Sinopse = json["overview"]?.ToString() ?? "--";
            item.Original = json[originalTitleField]?.ToString() ?? "--";

            // 4. GÊNEROS E ESTÚDIOS (Uso dos seus Cleansers)
            item.Genero = Cleanser.NormalizarGeneros(
                string.Join(", ", json["genres"]?.Select(g => g["name"]?.ToString()).Where(g => g != null) ?? [])
            );

            item.Estudio = Cleanser.GerarTags(
                string.Join(", ", json["production_companies"]?.Select(c => c["name"]?.ToString()).Where(c => c != null) ?? [])
            );

            // 5. CAMPOS ESPECÍFICOS POR CATEGORIA
            string tituloFormatado = Cleanser.FormatarTitulo(item.Titulo);

            if (tipo == MidiaTipo.Anime)
            {
                item.Serie = tituloFormatado;
                item.Obra = item.Original;
            }
            else if (tipo == MidiaTipo.Serie)
            {
                item.Serie = tituloFormatado;
            }
            else if (tipo == MidiaTipo.Filme)
            {
                item.Franquia = tituloFormatado;

                // Título Alternativo (exclusivo para Filmes via parâmetro opcional)
                if (alternative?["titles"] is JArray titles)
                {
                    item.Alternativo = titles
                        .Where(t => t["iso_3166_1"]?.ToString() == "BR")
                        .Select(t => t["title"]?.ToString())
                        .FirstOrDefault() ?? "--";
                }
            }

            // 6. ELENCO (Top 3 Artistas)
            if (credits["cast"] is JArray castArray)
            {
                var top3 = castArray.Take(3)
                                    .Select(a => Cleanser.GerarTags(a["name"]?.ToString()!))
                                    .Where(s => !string.IsNullOrEmpty(s));

                item.Artistas = string.Join(" ", top3);
            }

            // 7. DIRETOR / EQUIPE
            if (credits["crew"] is JArray crewArray)
            {
                var directors = crewArray
                    .Where(c => string.Equals(c["job"]?.ToString(), "Director", StringComparison.OrdinalIgnoreCase))
                    .Select(c => Cleanser.GerarTags(c["name"]?.ToString()!))
                    .Where(s => !string.IsNullOrEmpty(s));

                item.Diretor = string.Join(" ", directors);
            }

            // 8. LOCALIZAÇÃO E TRADUÇÃO (DeepL)
            // Buscando o primeiro país e idioma oficial do JSON
            var paisRaw = json["production_countries"]?.FirstOrDefault()?["name"]?.ToString() ?? "--";
            var idiomaRaw = json["spoken_languages"]?.FirstOrDefault()?["english_name"]?.ToString() ?? "--";

            // 1. Criamos as variáveis para as Tasks
            Task<DeepL.Model.TextResult>? taskPais = null;
            Task<DeepL.Model.TextResult>? taskIdioma = null;

            // 2. Disparamos as Tasks apenas se tiver algo útil para traduzir
            if (paisRaw != "--")
            {
                taskPais = deepl.Translate(paisRaw);
            }

            if (idiomaRaw != "--")
            {
                taskIdioma = deepl.Translate(idiomaRaw);
            }

            // 3. Aguardamos as que foram disparadas (se houver alguma)
            if (taskPais != null || taskIdioma != null)
            {
                // O Task.WhenAll aceita uma lista de tasks. Se uma for null, ele ignora se filtrarmos.
                await Task.WhenAll(new List<Task> { taskPais!, taskIdioma! }.Where(t => t != null));
            }

            // 4. Atribuímos os resultados
            item.Pais = taskPais != null
                ? Cleanser.FormatarTitulo(taskPais.Result.Text)
                : "--";

            item.Idioma = taskIdioma != null
                ? Cleanser.FormatarTitulo(taskIdioma.Result.Text).ToLower()
                : "--";

            return item;
        }

    }
}