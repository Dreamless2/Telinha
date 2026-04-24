using Newtonsoft.Json;
using Telinha.Cache;
using Telinha.Enums;
using Telinha.Factory;
using Telinha.Models;

namespace Telinha.Services
{
    public class MidiaServices(TMDBServices tmdb)
    {
        private readonly TMDBServices _tmdb = tmdb;

        public async Task<MidiaModel?> GetMidia(int id)
        {
            var filme = await ExecutarBusca(id, MidiaTipo.Filme);
            var serie = await ExecutarBusca(id, MidiaTipo.Serie);

            return DecidirMelhorResultado(filme, serie);
        }
        private async Task<MidiaModel?> ExecutarBusca(int id, MidiaTipo tipo)
        {
            var baseRoute = tipo == MidiaTipo.Filme ? "movie" : "tv";
            var calls = new List<(string, Dictionary<string, string>?)>
        {
            ($"/{baseRoute}/{id}", new() { ["language"] = "pt-BR" }),
            ($"/{baseRoute}/{id}/credits", new() { ["language"] = "pt-BR" })
        };

            if (tipo == MidiaTipo.Filme)
                calls.Add(($"/{baseRoute}/{id}/alternative_titles", null));

            var results = await _tmdb.Many([.. calls]);

            // ❗ VALIDAÇÃO CRÍTICA
            if (results == null || results.Length < 2 || results[0] == null)
                return null;


            if (results[0]?["success"] != null && results[0]?["success"]?.ToObject<bool>() == false)
                return null;

            if (results[0]?["status_code"]?.ToObject<int>() == 34)
                return null;

            LogServices.LogarInformacao("RESULT[0]: {json}", results[0]?.ToString()!);

            // ❗ validação de conteúdo real
            if (tipo == MidiaTipo.Filme && string.IsNullOrWhiteSpace(results[0]?["title"]?.ToString()))
                return null;

            if (tipo != MidiaTipo.Filme && string.IsNullOrWhiteSpace(results[0]?["name"]?.ToString()))
                return null;

            var apiFactory = new ApiClientFactory();

            var deepl = apiFactory.GetDeepL();

            var model = await MidiaFactory.ConstruirMidia(results[0], results[1], results.Length > 2 ? results[2] : null, tipo, deepl);

            if (model != null)
                if (Enum.TryParse(model.Tipo, out MidiaTipo tipoReal))
                    MidiaCache.Save(tipo, id, JsonConvert.SerializeObject(model));

                else
                    MidiaCache.Save(tipo, id, JsonConvert.SerializeObject(model));

            return model;
        }
    }
}