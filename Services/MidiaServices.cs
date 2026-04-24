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

        private async Task<MidiaModel?> ExecutarBuscaSeguro(int id, MidiaTipo tipo, CancellationToken ct)
        {
            const int maxTentativas = 2;

            for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
            {
                try
                {
                    ct.ThrowIfCancellationRequested();

                    var result = await ExecutarBusca(id, tipo, ct);

                    if (result != null)
                        return result;
                }
                catch (OperationCanceledException)
                {
                    return null;
                }
                catch (Exception ex)
                {
                    LogServices.LogarErroComException(ex, $"Erro tentativa {tentativa} - {tipo}");
                }

                await Task.Delay(300, ct); // pequeno retry
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