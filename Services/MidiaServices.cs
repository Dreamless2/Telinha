using Newtonsoft.Json.Linq;
using Telinha.Cache;
using Telinha.Enums;
using Telinha.Factory;
using Telinha.Models;

namespace Telinha.Services
{
    public class MidiaServices
    {
        private readonly TMDBServices _tmdb = tmdb;
        private readonly FileCacheServices? _cache;
        private readonly TimeSpan _cacheTtl = TimeSpan.FromHours(12);

        public MidiaServices(TMDBServices tmdb, FileCacheServices? cache = null)
        {
            _tmdb = tmdb;
            _cache = cache;
        }

        public async Task<MidiaModel?> GetMidia(int id)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var filmeTask = ExecutarBuscaSeguro(id, MidiaTipo.Filme, cts.Token);
            var serieTask = ExecutarBuscaSeguro(id, MidiaTipo.Serie, cts.Token);

            await Task.WhenAll(filmeTask, serieTask);

            var filme = filmeTask.Result;
            var serie = serieTask.Result;

            bool serieExiste = serie != null && !string.IsNullOrWhiteSpace(serie.Nome);
            bool filmeExiste = filme != null && !string.IsNullOrWhiteSpace(filme.Nome);

            LogServices.LogarInformacao("ID {id} - Filme: {filmeExiste}, Série: {serieExiste}",
                id, filmeExiste, serieExiste);

            if (!serieExiste && !filmeExiste)
                return null;

            if (serieExiste && !filmeExiste)
                return serie;

            if (filmeExiste && !serieExiste)
                return filme;

            filme!.Classificacao = ClassificarAnimacaoAvancado(filme);
            serie!.Classificacao = ClassificarAnimacaoAvancado(serie);

            return DecidirMelhorResultado(filme, serie);
        }

        private string ClassificarAnimacaoAvancado(MidiaModel m)
        {
            if (m == null) return "LiveAction";

            bool isAnimation = m.GenerosLista?.Any(g =>
                g.Contains("Animation", StringComparison.OrdinalIgnoreCase) ||
                g.Contains("Animação", StringComparison.OrdinalIgnoreCase)) == true;

            if (!isAnimation) return "LiveAction";

            int scoreAnime = 0;
            if (m.IdiomaOriginal == "ja") scoreAnime += 4;
            if (m.PaisesOrigem?.Any(p => p.Equals("JP", StringComparison.OrdinalIgnoreCase)) == true) scoreAnime += 3;

            string texto = $"{m.Nome} {m.Sinopse}".ToLower();
            string[] palavrasAnime = ["anime", "mangá", "manga", "shonen", "shoujo", "isekai", "mecha", "otaku", "samurai"];
            if (palavrasAnime.Any(p => texto.Contains(p))) scoreAnime += 2;
            if (m.ProdutorasLista?.Any(p => p.Contains("animation", StringComparison.OrdinalIgnoreCase)) == true) scoreAnime += 1;
            if (m.Episodios > 0 && m.Episodios <= 30 && m.DuracaoMedia > 0 && m.DuracaoMedia <= 30) scoreAnime += 1;

            if (scoreAnime >= 6 && m.IdiomaOriginal == "ja") return "Anime";
            if (scoreAnime >= 5) return "AnimeLike";
            if (m.IdiomaOriginal == "zh") return "Donghua";
            if (m.IdiomaOriginal == "ko") return "AnimacaoCoreana";
            return "AnimacaoOcidental";
        }

        private async Task<MidiaModel?> ExecutarBuscaSeguro(int id, MidiaTipo tipo, CancellationToken ct)
        {
            const int maxTentativas = 2;
            for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
            {
                try
                {
                    var result = await ExecutarBusca(id, tipo, ct);
                    if (result != null) return result;
                }
                catch (OperationCanceledException)
                {
                    LogServices.LogarInformacao("Timeout {tipo} ID {id}", tipo, id);
                    return null;
                }
                catch (Exception ex)
                {
                    LogServices.LogarErroComException(ex, $"Erro tentativa {tentativa} - {tipo} ID {id}");
                }

                if (tentativa < maxTentativas) await Task.Delay(500);
            }
            return null;
        }

        private async Task<MidiaModel?> ExecutarBusca(int id, MidiaTipo tipo, CancellationToken ct)
        {
            var cacheKey = $"tmdb_{tipo.ToString().ToLower()}_{id}";

            var cached = await _cache.GetAsync(cacheKey);

            if (cached != null) return cached;




            var baseRoute = tipo == MidiaTipo.Filme ? "movie" : "tv";
            var calls = new List<(string, Dictionary<string, string>?)>
            {
                ($"/{baseRoute}/{id}", new() { ["language"] = "pt-BR" }),
                ($"/{baseRoute}/{id}/credits", new() { ["language"] = "pt-BR" })
            };

            if (tipo == MidiaTipo.Filme)
                calls.Add(($"/{baseRoute}/{id}/alternative_titles", null));

            var results = await _tmdb.Many(ct, [.. calls]);

            if (results == null || results.Length < 2 || results[0] == null)
            {
                LogServices.LogarInformacao("TMDB {tipo} {id} - Resultado nulo", tipo, id);
                return null;
            }

            var details = results[0];
            var credits = results[1];
            var alternative = results.Length > 2 ? results[2] : null;

            LogServices.LogarInformacao("TMDB {tipo} {id} - Raw: {json}",
                tipo, id, details.ToString());

            if (details is not JObject validDetails) return null;
            if (validDetails["success"]?.ToObject<bool>() == false) return null;
            if (validDetails["status_code"]?.ToObject<int>() == 34) return null;
            if (!IsValidMedia(validDetails, tipo))
            {
                LogServices.LogarInformacao("IsValidMedia FALSE - {tipo} {id}", tipo, id);
                return null;
            }

            var deepl = new ApiClientFactory().GetDeepL();
            var model = await MidiaFactory.ConstruirMidia(details, credits, alternative, tipo, deepl);

            if (model != null) NormalizarModel(model, details);

            return model;
        }

        private bool IsValidMedia(JObject data, MidiaTipo tipo)
        {
            if (data?["success"]?.ToObject<bool>() == false) return false;
            if (data?["status_code"]?.ToObject<int>() == 34) return false;

            // 🔥 FIX: TMDB pode retornar array vazio em vez de string
            if (tipo == MidiaTipo.Filme)
            {
                var titleToken = data?["title"];
                if (titleToken == null || titleToken.Type == JTokenType.Array) return false;
                var title = titleToken.ToString();
                return !string.IsNullOrWhiteSpace(title);
            }

            if (tipo == MidiaTipo.Serie)
            {
                var nameToken = data?["name"];
                if (nameToken == null || nameToken.Type == JTokenType.Array) return false;
                var name = nameToken.ToString();
                return !string.IsNullOrWhiteSpace(name);
            }

            return false;
        }

        private MidiaModel? DecidirMelhorResultado(MidiaModel? filme, MidiaModel? serie)
        {
            if (filme == null) return serie;
            if (serie == null) return filme;

            if (serie.Classificacao is "Anime" or "AnimeLike") return serie;

            bool serieMaisForte = serie.Episodios > 0;
            bool filmeMaisForte = filme.Episodios == 0 && filme.DuracaoMedia > 60;

            if (serieMaisForte && !filmeMaisForte) return serie;
            if (filmeMaisForte && !serieMaisForte) return filme;

            double scoreFilme = CalcularScore(filme);
            double scoreSerie = CalcularScore(serie);

            LogServices.LogarInformacao("Score Filme: {filme}, Série: {serie}", scoreFilme, scoreSerie);
            return scoreSerie >= scoreFilme ? serie : filme;
        }

        private double CalcularScore(MidiaModel m)
        {
            double score = 0;
            if (!string.IsNullOrWhiteSpace(m.Nome)) score += 3;
            if (!string.IsNullOrWhiteSpace(m.Sinopse) && m.Sinopse.Length > 30) score += 2;
            if (m.Episodios > 0) score += 3;
            if (m.DuracaoMedia > 0) score += 2;

            if (m.Classificacao == "Anime") score += 4;
            else if (m.Classificacao == "AnimeLike") score += 2;
            else if (m.Classificacao == "LiveAction") score += 1;

            if (m.Popularidade > 0) score += Math.Min(m.Popularidade / 50.0, 2);
            if (m.Votos > 0) score += Math.Min(m.Votos / 1000.0, 2);

            bool pareceSerie = m.Episodios > 0 && m.DuracaoMedia <= 45;
            bool pareceFilme = m.Episodios == 0 && m.DuracaoMedia > 60;

            if (pareceSerie) score += 3;
            if (pareceFilme) score += 3;
            return score;
        }

        private void NormalizarModel(MidiaModel model, JObject data)
        {
            model.IdiomaOriginal = data?["original_language"]?.ToString();
            model.Popularidade = data?["popularity"]?.ToObject<double>() ?? 0;
            model.Votos = data?["vote_count"]?.ToObject<int>() ?? 0;
            model.Episodios = data?["number_of_episodes"]?.ToObject<int>() ?? 0;

            model.DuracaoMedia = data?["runtime"]?.ToObject<int?>()
                  ?? (data?["episode_run_time"] is JArray arr && arr.Count > 0
                      ? arr[0].ToObject<int?>()
                       : 0) ?? 0;

            model.PaisesOrigem = data?["origin_country"]?.ToObject<List<string>>() ?? [];
            model.GenerosLista = data?["genres"]?.Select(g => g["name"]?.ToString()).OfType<string>().ToList() ?? [];
            model.ProdutorasLista = data?["production_companies"]?.Select(p => p["name"]?.ToString()).OfType<string>().ToList() ?? [];

            // 🔥 Garante que não pega array vazio
            var titleToken = data?["title"] ?? data?["name"];
            model.Nome = titleToken?.Type != JTokenType.Array ? titleToken?.ToString() : "--";

            var originalToken = data?["original_title"] ?? data?["original_name"];
            model.Original = originalToken?.Type != JTokenType.Array ? originalToken?.ToString() : "--";

            model.Sinopse = data?["overview"]?.ToString();
        }
    }
}
