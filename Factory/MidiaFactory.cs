using Newtonsoft.Json.Linq;
using Telinha.Contracts;
using Telinha.Enums;
using Telinha.Models;
using Telinha.Services;
using Telinha.Utils;

namespace Telinha.Factory
{
    public class MidiaFactory
    {
        private readonly APIClientFactory _apiFactory;
        private readonly TokenServices _tokenService;

        public static async Task<MidiaModel> ConstruirMidia(JObject json, JObject credits, JObject? alternative, MidiaTipo tipoBase)
        {
            // 1. DETECÇÃO AUTOMÁTICA DE TIPO (Filme vs TV)
            // Se o JSON tem 'title' é filme, se tem 'name' é série/anime.
            MidiaTipo tipoDetectado = tipoBase;

            // 2. REFINAMENTO (Série vs Anime)
            // Se for TV, checamos se é animação asiática para classificar como Anime
            if (tipoDetectado == MidiaTipo.Serie)
            {
                var generosIds = json["genres"]?.Select(g => (int)g["id"]!).ToList() ?? [];
                string lingua = json["original_language"]?.ToString() ?? "";

                if (generosIds.Contains(16) && (lingua == "ja" || lingua == "zh" || lingua == "ko"))
                {
                    tipoDetectado = MidiaTipo.Anime;
                }
            }

            var item = new MidiaModel
            {
                Tipo = tipoDetectado.ToString()
            };

            // 3. MAPEAMENTO DE CAMPOS DINÂMICO
            bool isTV = tipoDetectado != MidiaTipo.Filme;
            string titleField = isTV ? "name" : "title";
            string dateField = isTV ? "first_air_date" : "release_date";
            string originalTitleField = isTV ? "original_name" : "original_title";

            // 4. TRATAMENTO DE DATA E TAGS
            var releaseDateStr = json[dateField]?.ToString();
            bool hasValidDate = DateTime.TryParse(releaseDateStr, out DateTime releaseDate);

            item.Lancamento = hasValidDate ? releaseDate.ToString("dd/MM/yyyy") : "--";

            string tagBase = tipoDetectado.ToString();

            //item.Tags = hasValidDate ? $"#{tagBase} #{tagBase}{releaseDate.Year}" : $"#{tagBase}";

            var tags = new List<string>
            {
                // sempre adiciona o padrão
                $"#{tagBase}"
            };

            if (hasValidDate)
                tags.Add($"#{tagBase}{releaseDate.Year}");

            // 🔥 regra especial para Série
            if (tipoDetectado == MidiaTipo.Serie)
            {
                string tagAcento = "Série";

                tags.Add($"#{tagAcento}");

                if (hasValidDate)
                    tags.Add($"#{tagAcento}{releaseDate.Year}");
            }

            item.Tags = string.Join(" ", tags);

            // 5. INFORMAÇÕES BÁSICAS
            item.Nome = json[titleField]?.ToString() ?? "--";
            item.Sinopse = json["overview"]?.ToString() ?? "--";
            item.Original = json[originalTitleField]?.ToString() ?? "--";

            // 6. GÊNEROS E ESTÚDIOS
            item.Genero = Cleanser.NormalizarGeneros(
                string.Join(", ", json["genres"]?.Select(g => g["name"]?.ToString()).Where(g => g != null) ?? [])
            );

            item.Produtora = Cleanser.GerarTags(
                string.Join(", ", json["production_companies"]?.Select(c => c["name"]?.ToString()).Where(c => c != null) ?? [])
            );

            // 7. CAMPOS ESPECÍFICOS POR CATEGORIA
            string tituloFormatado = Cleanser.FormatarTitulo(item.Nome);

            if (tipoDetectado == MidiaTipo.Anime)
            {
                item.Serie = tituloFormatado;
                item.Obra = item.Original;
            }
            else if (tipoDetectado == MidiaTipo.Serie)
            {
                item.Serie = tituloFormatado;
            }
            else if (tipoDetectado == MidiaTipo.Filme)
            {
                item.Franquia = tituloFormatado;

                if (alternative?["titles"] is JArray titles)
                {
                    item.Alternativo = titles
                        .Where(t => t["iso_3166_1"]?.ToString() == "BR")
                        .Select(t => t["title"]?.ToString())
                        .FirstOrDefault() ?? "--";
                }
            }

            // 8. ELENCO (Top 3)
            if (credits["cast"] is JArray castArray)
            {
                var top3 = castArray.Take(3)
                                    .Select(a => Cleanser.GerarTags(a["name"]?.ToString()!))
                                    .Where(s => !string.IsNullOrEmpty(s));

                item.Artistas = string.Join(" ", top3);
            }

            // 9. DIRETOR / EQUIPE
            if (credits["crew"] is JArray crewArray)
            {
                var directors = crewArray
                    .Where(c => string.Equals(c["job"]?.ToString(), "Director", StringComparison.OrdinalIgnoreCase))
                    .Select(c => Cleanser.GerarTags(c["name"]?.ToString()!))
                    .Where(s => !string.IsNullOrEmpty(s));

                item.Diretor = string.Join(" ", directors);
            }

            // 10. LOCALIZAÇÃO E TRADUÇÃO (DeepL)
            var paisRaw = json["production_countries"]?.FirstOrDefault()?["name"]?.ToString() ?? "--";
            var idiomaRaw = json["spoken_languages"]?.FirstOrDefault()?["english_name"]?.ToString() ?? "--";
            _apiFactory = new APIClientFactory(tokenService);

            Task<DeepL.Model.TextResult>? taskPais = (paisRaw != "--") ? deepl.Translate(paisRaw) : null;
            Task<DeepL.Model.TextResult>? taskIdioma = (idiomaRaw != "--") ? deepl.Translate(idiomaRaw) : null;

            if (taskPais != null || taskIdioma != null)
            {
                await Task.WhenAll(new List<Task?> { taskPais, taskIdioma }.Where(t => t != null)!);
            }

            item.Pais = taskPais != null ? Cleanser.FormatarTitulo(taskPais.Result.Text) : "--";
            item.Idioma = taskIdioma != null ? Cleanser.FormatarTitulo(taskIdioma.Result.Text).ToLower() : "--";

            return item;
        }
    }
}