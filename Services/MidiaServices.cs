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

        public async Task<MidiaModel?> GetMidia(int id, MidiaTipo tipo)
        {
            // 1. TENTAR CACHE LOCAL (Mantemos o tipo original da solicitação)
            // 1. TENTAR CACHE (Verificamos nas duas pastas de cache possíveis para garantir)
            string? cached = MidiaCache.Get(MidiaTipo.Filme, id) ?? MidiaCache.Get(MidiaTipo.Serie, id);
            if (cached != null) return JsonConvert.DeserializeObject<MidiaModel>(cached);

            // 2. TENTAR A ROTA INFORMADA PELO USUÁRIO
            try
            {
                return await ExecutarBusca(id, tipoInicial);
            }
            catch (Exception ex) when (ex.Message.Contains("404") || ex.Message.Contains("NotFound"))
            {
                // 3. SE NÃO ACHOU, TENTA A OUTRA ROTA IMEDIATAMENTE
                var tipoAlternativo = (tipoInicial == MidiaTipo.Filme) ? MidiaTipo.Serie : MidiaTipo.Filme;

                try
                {
                    return await ExecutarBusca(id, tipoAlternativo);
                }
                catch
                {
                    return null; // Não existe em nenhuma das duas
                }
            }
        }

        // Método auxiliar para evitar repetição de código
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

            if (results == null || results.Length < 2 || results[0] == null) return null;

            var model = await MidiaFactory.ConstruirMidia(results[0], results[1], results.Length > 2 ? results[2] : null);

            if (model != null)
            {
                // Salva no cache com o tipo REAL detectado pela factory
                Enum.TryParse(model.Tipo, out MidiaTipo tipoReal);
                MidiaCache.Save(tipoReal, id, JsonConvert.SerializeObject(model));
            }

            return model;
        }
    }
}