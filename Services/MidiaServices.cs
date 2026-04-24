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

            // 🔥 Classificação automática aplicada
            if (filme != null)
                filme.Classificacao = ClassificarAnimacaoAvancado(filme);

            if (serie != null)
                serie.Classificacao = ClassificarAnimacaoAvancado(serie);

            var escolhido = DecidirMelhorResultado(filme, serie);

            if (filme != null && serie != null && escolhido != null)
            {
                LogServices.LogarInformacao(
                    "Final -> Filme: {f} ({cf}), Série: {s} ({cs}), Escolhido: {e}",
                    filme.Classificacao ?? "N/A",
                    serie.Classificacao ?? "N/A",
                    escolhido.Classificacao ?? "N/A"
                );
            }

            return escolhido;
        }
        private string ClassificarAnimacaoAvancado(MidiaModel m)
        {
            bool isAnimation = m.GenerosLista?.Any(g =>
                g.Contains("Animation", StringComparison.OrdinalIgnoreCase) ||
                g.Contains("Animação", StringComparison.OrdinalIgnoreCase)) == true;

            if (!isAnimation)
                return "LiveAction";

            int scoreAnime = 0;

            // 🔹 1. idioma japonês (peso forte)
            if (m.IdiomaOriginal == "ja")
                scoreAnime += 4;

            // 🔹 2. país de origem
            if (m.PaisesOrigem?.Any(p => p.Equals("JP", StringComparison.OrdinalIgnoreCase)) == true)
                scoreAnime += 3;

            // 🔹 3. palavras-chave
            string texto = $"{m.Nome} {m.Sinopse}".ToLower();

            string[] palavrasAnime =
            [
                "anime", "mangá", "manga", "shonen", "shoujo", "isekai", "mecha", "otaku", "samurai"
            ];

            if (palavrasAnime.Any(p => texto.Contains(p)))
                scoreAnime += 2;

            // 🔹 4. estúdio (peso leve)
            if (m.ProdutorasLista?.Any(p =>
                p.Contains("animation", StringComparison.OrdinalIgnoreCase)) == true)
                scoreAnime += 1;

            // 🔹 5. formato típico
            if (m.Episodios > 0 && m.Episodios <= 30 && m.DuracaoMedia > 0 && m.DuracaoMedia <= 30)
                scoreAnime += 1;

            // 🔥 DECISÃO FINAL (melhorada)

            if (scoreAnime >= 6 && m.IdiomaOriginal == "ja")
                return "Anime";

            if (scoreAnime >= 5)
                return "AnimeLike";

            if (m.IdiomaOriginal == "zh")
                return "Donghua";

            if (m.IdiomaOriginal == "ko")
                return "AnimacaoCoreana";

            return "AnimacaoOcidental";
        }

        private MidiaModel? DecidirMelhorResultado(MidiaModel? filme, MidiaModel? serie)
        {
            if (filme == null && serie == null)
                return null;

            if (filme != null && serie == null)
                return filme;

            if (serie != null && filme == null)
                return serie;

            // 🔥 PRIORIDADE: Anime e AnimeLike
            if (serie?.Classificacao == "Anime")
                return serie;

            if (serie?.Classificacao == "AnimeLike")
                return serie;

            // 🔹 Tipo explícito
            if (filme?.Tipo?.Equals("Filme", StringComparison.OrdinalIgnoreCase) == true)
                return filme;

            if (serie?.Tipo?.Equals("Serie", StringComparison.OrdinalIgnoreCase) == true)
                return serie;

            // 🔹 Score
            double scoreFilme = CalcularScore(filme!);
            double scoreSerie = CalcularScore(serie!);

            LogServices.LogarInformacao("Score -> Filme: {f}, Série: {s}", scoreFilme, scoreSerie);

            if (scoreFilme > scoreSerie)
                return filme;

            if (scoreSerie > scoreFilme)
                return serie;

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

                await Task.Delay(300, ct);
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

        private async Task<MidiaModel?> ExecutarBusca(int id, MidiaTipo tipo, CancellationToken ct)
        {
            var baseRoute = tipo == MidiaTipo.Filme ? "movie" : "tv";

            var calls = new List<(string, Dictionary<string, string>?)>
            {
                ($"/{baseRoute}/{id}", new()
                {
                    ["language"] = "pt-BR"
                }),
                ($"/{baseRoute}/{id}/credits", new()
                {
                    ["language"] = "pt-BR"
                })
            };

            if (tipo == MidiaTipo.Filme)
                calls.Add(($"/{baseRoute}/{id}/alternative_titles", null));

            var results = await _tmdb.Many([.. calls], ct);

            // 🔴 validação base
            if (results == null || results.Length < 2 || results[0] == null)
                return null;

            if (results[0]?["success"]?.ToObject<bool>() == false)
                return null;

            if (results[0]?["status_code"]?.ToObject<int>() == 34)
                return null;

            var data = results[0];

            // 🔴 validação de existência real
            if (tipo == MidiaTipo.Filme &&
                string.IsNullOrWhiteSpace(data?["title"]?.ToString()))
                return null;

            if (tipo == MidiaTipo.Serie &&
                string.IsNullOrWhiteSpace(data?["name"]?.ToString()))
                return null;

            var deepl = new ApiClientFactory().GetDeepL();

            var model = await MidiaFactory.ConstruirMidia(
                data,
                results[1],
                results.Length > 2 ? results[2] : null,
                tipo,
                deepl
            );

            if (model == null)
                return null;

            // 🔥 NORMALIZAÇÃO (ESSENCIAL pro teu sistema novo)

            NormalizarModel(model, data);

            return model;
        }

        private void NormalizarModel(MidiaModel model, dynamic data)
        {
            // 🔹 idioma original (CRÍTICO pro anime detection)
            model.IdiomaOriginal = data?["original_language"]?.ToString();

            // 🔹 popularidade
            model.Popularidade = data?["popularity"]?.ToObject<double>() ?? 0;

            // 🔹 votos
            model.Votos = data?["vote_count"]?.ToObject<int>() ?? 0;

            // 🔹 episódios (séries)
            model.Episodios = data?["number_of_episodes"]?.ToObject<int>() ?? 0;

            // 🔹 duração média (filmes ou episódios)
            model.DuracaoMedia = data?["runtime"]?.ToObject<int>() ??
                                  data?["episode_run_time"]?[0]?.ToObject<int>() ?? 0;

            // 🔹 país de origem
            model.PaisesOrigem = ((IEnumerable<dynamic>?)data?["origin_country"])
                ?.Select(x => (string)x.ToString())
                .ToList();

            // 🔹 gêneros estruturados
            model.GenerosLista = ((IEnumerable<dynamic>?)data?["genres"])
                ?.Select(g => (string?)g?["name"]?.ToString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            // 🔹 produtoras estruturadas
            model.ProdutorasLista = ((IEnumerable<dynamic>?)data?["production_companies"])
                ?.Select(p => (string?)p?["name"]?.ToString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            // 🔹 título normalizado
            model.Nome = data?["title"]?.ToString()
                      ?? data?["name"]?.ToString();

            model.Original = data?["original_title"]?.ToString()
                         ?? data?["original_name"]?.ToString();
        }
    }
}