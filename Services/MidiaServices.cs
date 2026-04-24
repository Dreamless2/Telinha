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

        public async Task<MidiaModel?> GetMidia(int id, MidiaTipo tipoSolicitado)
        {
            // 1. Tenta cache primeiro (prioridade máxima)
            var cache = await TryGetFromCacheAsync(id, tipoSolicitado);
            if (cache != null)
                return cache;

            // 2. Busca paralela no TMDB
            var (filme, serie) = await BuscarFilmeESerieEmParaleloAsync(id);

            // 3. Decisão inteligente de qual retornar
            var midiaEscolhida = EscolherMelhorMidia(filme, serie, tipoSolicitado);

            // 4. Salva em cache (com o tipo real encontrado)
            if (midiaEscolhida != null)
            {
                await SalvarEmCacheAsync(midiaEscolhida, id);
            }

            return midiaEscolhida;
        }

        private static async Task<MidiaModel?> TryGetFromCacheAsync(int id, MidiaTipo tipo)
        {
            // Tenta o tipo solicitado primeiro
            string? json = MidiaCache.Get(tipo, id, 720);
            if (json != null)
                return JsonConvert.DeserializeObject<MidiaModel>(json);

            // Tenta o tipo alternativo (útil quando o ID é compartilhado entre filme e série)
            var tipoAlternativo = tipo == MidiaTipo.Filme ? MidiaTipo.Serie : MidiaTipo.Filme;
            json = MidiaCache.Get(tipoAlternativo, id, 720);
            if (json != null)
                return JsonConvert.DeserializeObject<MidiaModel>(json);

            return null;
        }

        private async Task<(MidiaModel? filme, MidiaModel? serie)> BuscarFilmeESerieEmParaleloAsync(int id)
        {
            var filmeTask = ExecutarBuscaAsync(id, MidiaTipo.Filme);
            var serieTask = ExecutarBuscaAsync(id, MidiaTipo.Serie);

            await Task.WhenAll(filmeTask, serieTask);

            return (await filmeTask, await serieTask);
        }

        private static MidiaModel? EscolherMelhorMidia(MidiaModel? filme, MidiaModel? serie, MidiaTipo tipoSolicitado)
        {
            if (filme != null && serie != null)
            {
                return tipoSolicitado == MidiaTipo.Filme ? filme : serie;
            }


            // Para Série ou Anime
            if (serie != null)
            {
                // Prioriza Anime se detectado
                if (serie.Tipo?.Equals("Anime", StringComparison.OrdinalIgnoreCase) == true)
                    return serie;

                return serie;
            }

            return filme ?? serie;
        }

        private static async Task SalvarEmCacheAsync(MidiaModel model, int id)
        {
            if (string.IsNullOrWhiteSpace(model.Tipo) ||
                !Enum.TryParse<MidiaTipo>(model.Tipo, true, out var tipoReal))
            {
                tipoReal = model.Tipo?.Contains("Anime", StringComparison.OrdinalIgnoreCase) == true
                           ? MidiaTipo.Serie
                           : MidiaTipo.Serie; // ajuste conforme sua lógica
            }

            MidiaCache.Save(tipoReal, id, JsonConvert.SerializeObject(model));
        }

        private async Task<MidiaModel?> ExecutarBuscaAsync(int id, MidiaTipo tipo)
        {
            var baseRoute = tipo == MidiaTipo.Filme ? "movie" : "tv";

            var calls = new List<(string endpoint, Dictionary<string, string>? query)>
        {
            ($"/{baseRoute}/{id}", new() { ["language"] = "pt-BR" }),
            ($"/{baseRoute}/{id}/credits", new() { ["language"] = "pt-BR" })
        };

            if (tipo == MidiaTipo.Filme)
            {
                calls.Add(("/movie/" + id + "/alternative_titles", null));
            }

            var results = await _tmdb.Many([.. calls]);

            // Validações críticas
            if (results == null || results.Length < 2 || results[0] == null)
                return null;

            if (results[0]?["success"]?.ToObject<bool>() == false)
                return null;

            if (results[0]?["status_code"]?.ToObject<int>() == 34) // Not Found
                return null;

            // Validação de conteúdo real
            var titulo = tipo == MidiaTipo.Filme
                ? results[0]?["title"]?.ToString()
                : results[0]?["name"]?.ToString();

            if (string.IsNullOrWhiteSpace(titulo))
                return null;

            // Construção do modelo
            var apiFactory = new ApiClientFactory();
            var deepl = apiFactory.GetDeepL();

            var model = await MidiaFactory.ConstruirMidia(
                results[0],
                results[1],
                results.Length > 2 ? results[2] : null,
                tipo,
                deepl);

            return model;
        }
    }
}