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
            // O tipo recebido aqui define se buscamos na rota de 'movie' ou 'tv'
            var baseRoute = tipo == MidiaTipo.Filme ? "movie" : "tv";

            var calls = new List<(string, Dictionary<string, string>?)>
            {
                ($"/{baseRoute}/{id}", new() { ["language"] = "pt-BR" }),
                ($"/{baseRoute}/{id}/credits", new() { ["language"] = "pt-BR" })
            };

            // Mantemos a busca de títulos alternativos apenas para filmes
            if (tipo == MidiaTipo.Filme)
            {
                calls.Add(($"/{baseRoute}/{id}/alternative_titles", null));
            }

            // 3. REQUISIÇÕES PARALELAS
            var results = await _tmdb.Many([.. calls]);

            // Verificação de segurança
            if (results == null || results.Length < 2 || results[0] == null)
            {
                return null;
            }

            // 4. CHAMAR A FACTORY UNIFICADA
            // Note que removemos o parâmetro 'tipo', a factory decide baseada nos resultados
            var model = await MidiaFactory.ConstruirMidia(
                results[0],
                results[1],
                results.Length > 2 ? results[2] : null
            );

            // 5. GRAVAR NO CACHE
            if (model != null)
            {
                // Salvaguarda: usamos o tipo detectado pela Factory para salvar no cache correto
                // ou mantemos o 'tipo' original da chamada, conforme sua preferência de organização.
                MidiaCache.Save(tipo, id, JsonConvert.SerializeObject(model));
            }

            return model;
        }
    }
}