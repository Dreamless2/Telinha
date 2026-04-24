using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            var filme = await ExecutarBuscaSeguro(id, MidiaTipo.Filme, cts.Token);
            var serie = await ExecutarBuscaSeguro(id, MidiaTipo.Serie, cts.Token);

            filme?.Classificacao = ClassificarAnimacaoAvancado(filme);
            serie?.Classificacao = ClassificarAnimacaoAvancado(serie);

            // 🔥 REGRA SIMPLES E BLINDADA
            if (serie != null)
                return serie;

            return filme;
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

            var results = await _tmdb.Many(ct, [.. calls]);

            if (results == null || results.Length < 2 || results[0] == null)
                return null;

            var details = results[0];
            var credits = results[1];
            var alternative = results.Length > 2 ? results[2] : null;

            if (details?["success"]?.ToObject<bool>() == false)
                return null;

            if (details?["status_code"]?.ToObject<int>() == 34)
                return null;

            // 🔥 validação estrutural REAL (sem inferência fraca)
            if (details?["status_code"]?.ToObject<int>() == 7)
                return null;

            if (!IsValidMedia(details, tipo))
                return null;

            var deepl = new ApiClientFactory().GetDeepL();

            var model = await MidiaFactory.ConstruirMidia(
                details,
                credits,
                alternative,
                tipo,
                deepl
            );

            if (model == null)
                return null;

            NormalizarModel(model, details);

            return model;
        }

        private bool IsValidMedia(JObject data, MidiaTipo tipo)
        {
            if (tipo == MidiaTipo.Filme)
            {
                var title = data?["title"]?.ToString();
                var runtime = data?["runtime"]?.ToObject<int?>();

                return !string.IsNullOrWhiteSpace(title)
                       && runtime.HasValue;
            }

            if (tipo == MidiaTipo.Serie)
            {
                var name = data?["name"]?.ToString();
                var episodes = data?["number_of_episodes"]?.ToObject<int?>();

                return !string.IsNullOrWhiteSpace(name)
                       && episodes.HasValue;
            }

            return false;
        }

        private MidiaModel? DecidirMelhorResultado(MidiaModel? filme, MidiaModel? serie)
        {
            if (filme == null) return serie;
            if (serie == null) return filme;

            // 🔥 Anime sempre vence série comum
            if (serie.Classificacao is "Anime" or "AnimeLike")
                return serie;

            // 🔥 consistência estrutural
            bool serieMaisForte =
                serie.Episodios > 0 ||
                !string.IsNullOrWhiteSpace(serie.Serie);

            bool filmeMaisForte =
                serie.Episodios == 0 &&
                filme.DuracaoMedia > 0;

            if (serieMaisForte && !filmeMaisForte)
                return serie;

            if (filmeMaisForte && !serieMaisForte)
                return filme;

            // fallback seguro
            double scoreFilme = CalcularScore(filme);
            double scoreSerie = CalcularScore(serie);

            return scoreSerie >= scoreFilme ? serie : filme;
        }

        private double CalcularScore(MidiaModel m)
        {
            double score = 0;

            // 🔥 identidade forte (o que mais importa)
            if (!string.IsNullOrWhiteSpace(m.Nome))
                score += 3;

            if (!string.IsNullOrWhiteSpace(m.Sinopse) && m.Sinopse.Length > 30)
                score += 2;

            // 🔥 estrutura válida de série vs filme
            if (m.Episodios > 0)
                score += 3;

            if (m.DuracaoMedia > 0)
                score += 2;

            // 🔥 consistência de classificação
            if (!string.IsNullOrWhiteSpace(m.Classificacao))
            {
                if (m.Classificacao == "Anime")
                    score += 4;

                if (m.Classificacao == "AnimeLike")
                    score += 2;

                if (m.Classificacao == "LiveAction")
                    score += 1;
            }

            // 🔥 sinais fracos (não decisivos sozinhos)
            if (m.Popularidade > 0)
                score += Math.Min(m.Popularidade / 50.0, 2);

            if (m.Votos > 0)
                score += Math.Min(m.Votos / 1000.0, 2);

            // 🔥 bônus de consistência interna
            bool pareceSerie = m.Episodios > 0 && m.DuracaoMedia <= 45;
            bool pareceFilme = m.Episodios == 0 && m.DuracaoMedia > 60;

            if (pareceSerie)
                score += 3;

            if (pareceFilme)
                score += 3;

            return score;
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
            model.DuracaoMedia = data?["runtime"]?.ToObject<int?>()
                    ?? (data?["episode_run_time"] is JArray arr && arr.Count > 0
                    ? arr[0].ToObject<int?>()
                    : 0);

            // 🔹 país de origem
            model.PaisesOrigem = ((IEnumerable<dynamic>?)data?["origin_country"])
                ?.Select(x => (string)x.ToString())
                .ToList();

            // 🔹 gêneros estruturados
            model.GenerosLista = ((IEnumerable<dynamic>?)data?["genres"])
                ?.Select(g => (string?)g?["name"]?.ToString())
                .OfType<string>()
                .ToList();

            // 🔹 produtoras estruturadas
            model.ProdutorasLista = ((IEnumerable<dynamic>?)data?["production_companies"])
                ?.Select(p => (string?)p?["name"]?.ToString())
                .OfType<string>()
                .ToList();

            // 🔹 título normalizado
            model.Nome = data?["title"]?.ToString()
                      ?? data?["name"]?.ToString();

            model.Original = data?["original_title"]?.ToString()
                         ?? data?["original_name"]?.ToString();
        }
    }
}