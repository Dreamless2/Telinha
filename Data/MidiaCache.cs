using Telinha.Enums;
using Telinha.Models;

namespace Telinha.Data
{
    public static class MidiaCache
    {
        public static string? Get(MidiaTipo tipo, int id, int maxAgeMinutes)
        {
            var item = Database.DB.Select<CacheModel>()
                .Where(x => x.Type == tipo.ToString() && x.MidiaId == id)
                .First();

            if (item == null)
            {
                return null;
            }

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Se expirou, retorna null (o MidiaServices vai buscar no TMDB e dar Save novo)
            if ((now - item.UpdatedAt) > maxAgeMinutes * 60)
            {
                return null;
            }

            return item.Json;
        }

        public static void Save(MidiaTipo tipo, int id, string json)
        {
            var cache = new CacheModel
            {
                Type = tipo.ToString(),
                MidiaId = id,
                Json = json,
                UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            // O segredo está aqui: SetSource(item, x => new { x.Type, x.MidiaId })
            // Isso diz ao FreeSql: "Considere esses dois campos como a chave para decidir se Insere ou Atualiza"
            Database.DB.InsertOrUpdate<CacheModel>()
                .SetSource(cache, x => new { x.Type, x.MidiaId })
                .ExecuteAffrows();
        }
    }
}