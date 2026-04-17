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

        public async Task<MidiaModel?> GetMidia(int id, MidiaTipo tipoSolicitado)
        {
            string? cached = MidiaCache.Get(tipoSolicitado, id, 720);
            if (cached != null)
                return JsonConvert.DeserializeObject<MidiaModel>(cached);

            var tipoAlternativo = (tipoSolicitado == MidiaTipo.Filme) ? MidiaTipo.Serie : MidiaTipo.Filme;

            cached = MidiaCache.Get(tipoAlternativo, id, 720);
            if (cached != null)
                return JsonConvert.DeserializeObject<MidiaModel>(cached);

            MidiaModel? model = null;

            // 🔹 Tenta tipo solicitado
            try
            {
                model = await ExecutarBusca(id, tipoSolicitado);
                if (model != null)
                    return model;
            }
            catch
            {
                // ❗ NÃO retorna aqui — deixa seguir
            }

            // 🔹 Tenta tipo alternativo
            try
            {
                model = await ExecutarBusca(id, tipoAlternativo);
                if (model != null)
                    return model;
            }
            catch
            {
                // ignora também
            }

            return null;
        }

        private static bool NotFoundException(Exception ex)
        {
            if (ex is HttpRequestException httpEx &&
                httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                return true;

            string msg = ex.Message.ToLowerInvariant();
            return msg.Contains("404") || msg.Contains("not found");
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

            if (results == null || results.Length < 2 || results[0] == null)
                return null;

            var model = await MidiaFactory.ConstruirMidia(results[0], results[1],
    results.Length > 2 ? results[2] : null,
    tipo
);

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