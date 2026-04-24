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
                escolhido != null ? escolhido.Titulo : null
            );

            return escolhido;
        }

        private MidiaModel? DecidirMelhorResultado(MidiaModel? filme, MidiaModel? serie)
        {
            if (filme == null && serie == null)
                return null;

            if (filme != null && serie == null)
                return filme;

            if (serie != null && filme == null)
                return serie;

            // 🔥 DETECÇÃO DE ANIME
            if (EhAnime(serie))
                return serie;

            // 🔹 Tipo explícito
            if (filme?.Tipo?.Equals("Filme", StringComparison.OrdinalIgnoreCase) == true)
                return filme;

            if (serie?.Tipo?.Equals("Serie", StringComparison.OrdinalIgnoreCase) == true)
                return serie;

            // 🔹 Score de qualidade
            double scoreFilme = CalcularScore(filme);
            double scoreSerie = CalcularScore(serie);

            LogServices.LogarInformacao("Score -> Filme: {f}, Série: {s}", scoreFilme, scoreSerie);

            if (scoreFilme > scoreSerie)
                return filme;

            if (scoreSerie > scoreFilme)
                return serie;

            // fallback
            return filme ?? serie;
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


        private double CalcularScore(MidiaModel m)
        {
            double score = 0;

            if (!string.IsNullOrWhiteSpace(m.Nome))
                score += 2;

            if (!string.IsNullOrWhiteSpace(m.Sinopse))
                score += 1;

            if (m.Popularidade > 0)
                score += m.Popularidade / 100;

            if (m.Votos > 0)
                score += m.Votos / 1000;

            return score;
        }


        private bool EhAnime(MidiaModel? m)
        {
            if (m == null) return false;

            // idioma japonês
            if (m.Idioma?.Equals("ja", StringComparison.OrdinalIgnoreCase) == true)
                return true;

            // palavras-chave comuns
            if (m.Genero?.Any(g =>
                g.Contains("anima", StringComparison.OrdinalIgnoreCase) ||
                g.Contains("anime", StringComparison.OrdinalIgnoreCase)) == true)
                return true;

            // produtoras comuns de anime (exemplo)
            if (m.Produtora?.Any(p =>
                p.Contains("Toei", StringComparison.OrdinalIgnoreCase) ||
                p.Contains("Madhouse", StringComparison.OrdinalIgnoreCase) ||
                p.Contains("Bones", StringComparison.OrdinalIgnoreCase)) == true)
                return true;

            return false;
        }



        private async Task<MidiaModel?> ExecutarBusca(int id, MidiaTipo tipo, CancellationToken ct)
        {
            var baseRoute = tipo == MidiaTipo.Filme ? "movie" : "tv";

            var calls = new List<(string, Dictionary<string, string>?)>
            {
                ($"/{baseRoute}/{id}", new() { ["language"] = "pt-BR" }),
                ($"/{baseRoute}/{id}/credits", new() { ["language"] = "pt-BR" })
            };

            if (tipo == MidiaTipo.Filme)
                calls.Add(($"/{baseRoute}/{id}/alternative_titles", null));

            var results = await _tmdb.Many([.. calls], ct);

            if (results == null || results.Length < 2 || results[0] == null)
                return null;

            if (results[0]?["success"]?.ToObject<bool>() == false)
                return null;

            if (results[0]?["status_code"]?.ToObject<int>() == 34)
                return null;

            // 🔥 validação REAL do TMDB
            if (tipo == MidiaTipo.Filme &&
                string.IsNullOrWhiteSpace(results[0]?["title"]?.ToString()))
                return null;

            if (tipo == MidiaTipo.Serie &&
                string.IsNullOrWhiteSpace(results[0]?["name"]?.ToString()))
                return null;

            var deepl = new ApiClientFactory().GetDeepL();

            return await MidiaFactory.ConstruirMidia(
                results[0],
                results[1],
                results.Length > 2 ? results[2] : null,
                tipo,
                deepl
            );
        }
    }
}


