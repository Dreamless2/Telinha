using Newtonsoft.Json.Linq;
using Telinha.Core.Contracts;
using Telinha.Core.Enums;
using Telinha.Core.Helpers;
using Telinha.Core.Models;
using Telinha.Core.Utils;

namespace Telinha.Core.Factory
{
    public static class MidiaFactory
    {
        public static async Task<MidiaModel> ConstruirMidia(JObject json, JObject credits, JObject? alternative, MidiaTipo tipoBase, DEEPLContracts deepl)
        {
            MidiaTipo tipoDetectado = tipoBase;

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

            bool isTV = tipoDetectado != MidiaTipo.Filme;
            string titleField = isTV ? "name" : "title";
            string dateField = isTV ? "first_air_date" : "release_date";
            string originalTitleField = isTV ? "original_name" : "original_title";
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
            item.Id = json["id"]?.ToObject<int>() ?? 0;
            item.Nome = json[titleField]?.ToString() ?? "--";
            item.Sinopse = json["overview"]?.ToString() ?? "--";
            item.Original = json[originalTitleField]?.ToString() ?? "--";
            item.Genero = TagEngine.NormalizarGeneros(
                string.Join(", ", json["genres"]?.Select(g => g["name"]?.ToString()).Where(g => g != null) ?? []) ?? "--"
            );
            item.Produtora = TagEngine.GerarTags(
                string.Join(", ", json["production_companies"]?.Select(c => c["name"]?.ToString()).Where(c => c != null) ?? []) ?? "--"
            );

            var tituloFormatado = TagEngine.FormatarTitulo(item.Nome);

            if (tipoDetectado == MidiaTipo.Anime)
            {
                item.Serie = tituloFormatado;
                item.Referencia = item.Original ?? "--";
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

            if (credits["cast"] is JArray castArray)
            {
                var top3 = castArray.Take(3)
                                    .Select(a => TagEngine.GerarTags(a["name"]?.ToString()!))
                                    .Where(s => !string.IsNullOrEmpty(s));

                item.Artistas = string.Join(" ", top3) ?? "--";
            }

            if (credits["crew"] is JArray crewArray)
            {
                var directors = crewArray
                    .Where(c => string.Equals(c["job"]?.ToString(), "Director", StringComparison.OrdinalIgnoreCase))
                    .Select(c => TagEngine.GerarTags(c["name"]?.ToString()!))
                    .Where(s => !string.IsNullOrEmpty(s));

                item.Diretor = string.Join(" ", directors) ?? "--";
            }

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

            var localFormatado = TagEngine.FormatarTitulo(taskPais.Result ?? "--");
            var localSemAcento = TagEngine.RemoverAcentos(localFormatado);
            item.Local = TagEngine.FormatarTitulo(taskPais.Result ?? "--");

            item.Idioma = TagEngine
                .FormatarTitulo(taskIdioma.Result ?? "--")
                .ToLower();

            return item;
        }
    }
}
