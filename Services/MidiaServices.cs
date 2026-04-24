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

            LogServices.LogarInformacao(
                "Final -> Filme: {f} ({cf}), Série: {s} ({cs}), Escolhido: {e}",
                filme != null,
                filme?.Classificacao,
                serie != null,
                serie?.Classificacao,
                escolhido?.Classificacao
            );

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
                "anime", "mangá", "manga", "shonen", "shoujo",
        "isekai", "mecha", "otaku", "samurai"
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

    }
}
