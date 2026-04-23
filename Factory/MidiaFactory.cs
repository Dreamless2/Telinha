using Newtonsoft.Json.Linq;
using Telinha.Contracts;
using Telinha.Enums;
using Telinha.Helpers;
using Telinha.Models;
using Telinha.Utils;

namespace Telinha.Factory
{
    public static class MidiaFactory
    {
        public static async Task<MidiaModel> ConstruirMidia(JObject json, JObject credits, JObject? alternative, MidiaTipo tipoBase, DEEPLContracts deepl)
        {
            // 1. DETECÇÃO AUTOMÁTICA DE TIPO (Filme vs TV)
            MidiaTipo tipoDetectado = tipoBase;

            // 2. REFINAMENTO (Série vs Anime)
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

            item.Estreia = hasValidDate ? releaseDate.ToString("dd/MM/yyyy") : "--";

            string tagBase = tipoDetectado.ToString();

            var tags = new List<string>
            {
                $"#{tagBase}"
            };

            if (hasValidDate)
                tags.Add($"#{tagBase}{releaseDate.Year}");

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
            item.Genero = TagEngine.NormalizarGeneros(
                string.Join(", ", json["genres"]?.Select(g => g["name"]?.ToString()).Where(g => g != null) ?? [])
            );

            item.Produtora = TagEngine.GerarTags(
                string.Join(", ", json["production_companies"]?.Select(c => c["name"]?.ToString()).Where(c => c != null) ?? [])
            );

            // 7. CAMPOS ESPECÍFICOS POR CATEGORIA
            //var tituloFormatado = TagEngine.FormatarTitulo(item.Nome);
            var tituloFormatado = TagEngine.FormatarTitulo("Highlander 2 A Ressurreição");

            if (tipoDetectado == MidiaTipo.Anime)
            {
                item.Serie = tituloFormatado;
                item.Referencia = item.Original;
            }
            else if (tipoDetectado == MidiaTipo.Serie)
            {
                item.Serie = tituloFormatado;
            }
            else if (tipoDetectado == MidiaTipo.Filme)
            {
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
                                    .Select(a => TagEngine.GerarTags(a["name"]?.ToString()!))
                                    .Where(s => !string.IsNullOrEmpty(s));

                item.Artistas = string.Join(" ", top3);
            }

            // 9. DIRETOR / EQUIPE
            if (credits["crew"] is JArray crewArray)
            {
                var directors = crewArray
                    .Where(c => string.Equals(c["job"]?.ToString(), "Director", StringComparison.OrdinalIgnoreCase))
                    .Select(c => TagEngine.GerarTags(c["name"]?.ToString()!))
                    .Where(s => !string.IsNullOrEmpty(s));

                item.Diretor = string.Join(" ", directors);
            }

            // 10. LOCALIZAÇÃO E TRADUÇÃO (DeepL)            
            var countryRaw = json["production_countries"]?.FirstOrDefault()?["name"]?.ToString();

            var languageEnglish = json["spoken_languages"]?.FirstOrDefault()?["english_name"]?.ToString();
            var languageIso = json["spoken_languages"]?.FirstOrDefault()?["iso_639_1"]?.ToString();

            var taskPais = !string.IsNullOrWhiteSpace(countryRaw)
                ? deepl.Translate(countryRaw)
                : Task.FromResult<string?>("--");

            var taskIdioma = LanguageHelper.ResolveAsync(
                languageEnglish,
                languageIso,
                deepl.Translate
            );

            await Task.WhenAll(taskPais!, taskIdioma);

            item.Local = taskPais.Result ?? "--";

            item.Idioma = TagEngine
                .FormatarTitulo(taskIdioma.Result ?? "--")
                .ToLower();

            return item;
        }
    }
}