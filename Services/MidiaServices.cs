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
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));

            var filmeTask = ExecutarBuscaSeguro(id, MidiaTipo.Filme, cts.Token);
            var serieTask = ExecutarBuscaSeguro(id, MidiaTipo.Serie, cts.Token);

            await Task.WhenAll(filmeTask, serieTask);

            var filme = filmeTask.Result;
            var serie = serieTask.Result;

            var escolhido = DecidirMelhorResultado(filme, serie);

            LogServices.LogarInformacao(
                "Resultado final -> Filme: {f}, Série: {s}, Escolhido: {e}",
                filme != null,
                serie != null,
                escolhido?.Tipo
            );

            return escolhido;
        }

        private MidiaModel? DecidirMelhorResultado(MidiaModel? filme, MidiaModel? serie)
        {
            // 🔹 1. Só um existe
            if (filme != null && serie == null)
                return filme;

            if (serie != null && filme == null)
                return serie;

            // 🔹 2. Nenhum existe
            if (filme == null && serie == null)
                return null;

            // 🔹 3. Ambos existem → decidir com critério

            // Prioridade 1: Tipo explícito (se vier correto)
            if (Enum.TryParse(filme!.Tipo, true, out MidiaTipo tipoFilme) &&
                tipoFilme == MidiaTipo.Filme)
                return filme;

            if (Enum.TryParse(serie!.Tipo, true, out MidiaTipo tipoSerie) &&
                tipoSerie != MidiaTipo.Filme)
                return serie;

            // 🔹 4. Critério por conteúdo (TMDB padrão)

            bool filmeValido = !string.IsNullOrWhiteSpace(filme?.TituloOriginal);
            bool serieValida = !string.IsNullOrWhiteSpace(serie?.TituloOriginal);

            if (filmeValido && !serieValida)
                return filme;

            if (serieValida && !filmeValido)
                return serie;

            // 🔹 5. Critério por popularidade (se tiver)
            if (filme?.Popularidade > serie?.Popularidade)
                return filme;

            if (serie?.Popularidade > filme?.Popularidade)
                return serie;

            // 🔹 6. Fallback final (evita null)
            return filme ?? serie;
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

            // 🔴 validações fortes
            if (results == null || results.Length < 2 || results[0] == null)
                return null;

            if (results[0]?["success"]?.ToObject<bool>() == false)
                return null;

            if (results[0]?["status_code"]?.ToObject<int>() == 34)
                return null;

            // 🔴 validação essencial de conteúdo
            if (tipo == MidiaTipo.Filme &&
                string.IsNullOrWhiteSpace(results[0]?["title"]?.ToString()))
                return null;

            if (tipo == MidiaTipo.Serie &&
                string.IsNullOrWhiteSpace(results[0]?["name"]?.ToString()))
                return null;

            var apiFactory = new ApiClientFactory();
            var deepl = apiFactory.GetDeepL();

            var model = await MidiaFactory.ConstruirMidia(
                results[0],
                results[1],
                results.Length > 2 ? results[2] : null,
                tipo,
                deepl
            );

            return model;
        }
    }
}