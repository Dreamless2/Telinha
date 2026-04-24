using Telinha.Data;
using Telinha.Enums;
using Telinha.Models;
using Telinha.Services;

namespace Telinha.Cache
{
    public static class MidiaCache
    {
        public static string? Get(MidiaTipo tipo, int id, int maxAgeMinutes)
        {
            try
            {
                // Alterado para FirstOrDefault para evitar exceções se não houver cache
                var item = Database.DB.Select<CacheModel>()
                    .Where(x => x.Type == tipo.ToString() && x.MidiaId == id)
                    .FirstOrDefault<CacheModel>();

                if (item == null) return null;

                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Verifica expiração
                if ((now - item.UpdatedAt) > (maxAgeMinutes * 60))
                {
                    // Opcional: Deletar registro expirado para não lotar o banco
                    return null;
                }

                return item.Json;
            }
            catch (Exception ex)
            {
                LogServices.LogarErroComException(ex, "Erro ao ler cache MariaDB para {Tipo} ID {Id}", tipo, id);
                return null;
            }
        }

        public static void Save(MidiaTipo tipo, int id, string json)
        {
            try
            {
                var cache = new CacheModel
                {
                    Type = tipo.ToString(),
                    MidiaId = id,
                    Json = json,
                    UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                Database.DB.InsertOrUpdate<CacheModel>()
                    .SetSource(cache)
                    .ExecuteAffrows();
            }
            catch (Exception ex)
            {
                LogServices.LogarErroComException(ex, "Erro ao salvar cache no MariaDB para {Tipo} ID {Id}", tipo, id);
            }
        }
    }
}