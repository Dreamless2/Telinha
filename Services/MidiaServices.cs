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
            // 1. TENTAR CACHE LOCAL
            string? cached = MidiaCache.Get(tipo, id, maxAgeMinutes: 720); // 12h
            if (cached != null)
            {
                return JsonConvert.DeserializeObject<MidiaModel>(cached);
            }

            // 2. DEFINIR ROTAS
            // Mapeia o enum MidiaTipo para a rota do TMDB
            var baseRoute = tipo == MidiaTipo.Filme ? "movie" : "tv";

            var calls = new List<(string, Dictionary<string, string>?)>
        {
            ($"/{baseRoute}/{id}", new() { ["language"] = "pt-BR" }),
            ($"/{baseRoute}/{id}/credits", new() { ["language"] = "pt-BR" })
        };

            // Somente filmes precisam dos títulos alternativos para buscar o título no BR
            if (tipo == MidiaTipo.Filme)
            {
                calls.Add(($"/{baseRoute}/{id}/alternative_titles", new() { ["country"] = "BR" }));
            }

            // 3. REQUISIÇÕES PARALELAS
            var results = await _tmdb.Many([.. calls]);

            // Verificação de segurança
            if (results == null || results.Length < 2 || results[0] == null)
            {
                return null;
            }

            // 4. CHAMAR A FACTORY UNIFICADA
            var model = await MidiaFactory.ConstruirMidia(
                tipo,
                results[0],
                results[1],
                results.Length > 2 ? results[2] : null
            );

            // 5. GRAVAR NO CACHE (JSON compactado)
            if (model != null)
            {
                MidiaCache.Save(tipo, id, JsonConvert.SerializeObject(model));
            }

            return model;
        }
    }
}