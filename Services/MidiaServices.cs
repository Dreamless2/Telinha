using Newtonsoft.Json;
using Telinha.Data;
using Telinha.Enums;
using Telinha.Factory;
using Telinha.Models;

namespace Telinha.Services
{
    public class MidiaServices(TMDBServices tmdb)
    {
        private readonly TMDBServices _tmdb = tmdb;
        private readonly TokenServices? _tokenService;

        public async Task<MidiaModel?> GetMidia(int id, MidiaTipo tipoSolicitado)
        {
            var tipoAlternativo = (tipoSolicitado == MidiaTipo.Filme)
                ? MidiaTipo.Serie
                : MidiaTipo.Filme;

            // 🔹 1. CACHE (tenta ambos)
            string? cacheSolicitado = MidiaCache.Get(tipoSolicitado, id, 720);
            if (cacheSolicitado != null)
                return JsonConvert.DeserializeObject<MidiaModel>(cacheSolicitado);

            string? cacheAlternativo = MidiaCache.Get(tipoAlternativo, id, 720);
            if (cacheAlternativo != null)
                return JsonConvert.DeserializeObject<MidiaModel>(cacheAlternativo);

            // 🔹 2. BUSCA PARALELA
            var filmeTask = Task.Run(async () =>
            {
                try
                {
                    return await ExecutarBusca(id, MidiaTipo.Filme);
                }
                catch
                {
                    //return null;
                    MessageBox.Show($"Erro interno: {ex.Message}\n{ex.StackTrace}");
                    return null;
                }
            });

            var serieTask = Task.Run(async () =>
            {
                try
                {
                    return await ExecutarBusca(id, MidiaTipo.Serie);
                }
                catch
                {
                    return null;
                }
            });

            await Task.WhenAll(filmeTask, serieTask);

            var filme = filmeTask.Result;
            var serie = serieTask.Result;

            await Task.WhenAll(filmeTask, serieTask);

            // 🔹 3. DECISÃO INTELIGENTE
            if (tipoSolicitado == MidiaTipo.Filme)
            {
                if (filme != null) return filme;
                if (serie != null) return serie;
            }
            else // Série OU Anime
            {
                // 🔥 prioriza anime se detectado
                if (serie != null && serie.Tipo!.Equals("Anime", StringComparison.OrdinalIgnoreCase))
                    return serie;

                if (serie != null) return serie;

                if (filme != null) return filme;
            }

            return null;
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

            if (results[0]?["success"]?.ToObject<bool>() == false)
                return null;

            if (results[0]?["status_code"]?.ToObject<int>() == 34)
                return null;

            // ❗ validação de conteúdo real
            if (tipo == MidiaTipo.Filme && string.IsNullOrWhiteSpace(results[0]?["title"]?.ToString()))
                return null;

            if (tipo != MidiaTipo.Filme && string.IsNullOrWhiteSpace(results[0]?["name"]?.ToString()))
                return null;


            var apiFactory = new ApiClientFactory(_tokenService!);

            var deepl = await apiFactory.GetDeepLAsync();

            var model = await MidiaFactory.ConstruirMidia(results[0], results[1], results.Length > 2 ? results[2] : null, tipo, deepl);

            if (model != null)
            {
                if (Enum.TryParse(model.Tipo, out MidiaTipo tipoReal))
                    MidiaCache.Save(tipoReal, id, JsonConvert.SerializeObject(model));
                else
                    MidiaCache.Save(tipo, id, JsonConvert.SerializeObject(model));
            }

            bool invalido = results[0]?["id"] == null || (tipo == MidiaTipo.Filme && string.IsNullOrWhiteSpace(results[0]?["title"]?.ToString())) || (tipo != MidiaTipo.Filme && string.IsNullOrWhiteSpace(results[0]?["name"]?.ToString()));

            if (invalido)
                return null;

            return model;
        }
    }
}