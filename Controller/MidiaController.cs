using System.Reflection;
using Telinha.Data;
using Telinha.Entity;

namespace Telinha.Controller
{
    public static class MidiaController
    {
        private static IFreeSql DB => Database.DB;

        private static readonly Dictionary<Type, PropertyInfo?> _codigoCache = [];
        private static readonly Dictionary<Type, PropertyInfo?> _idCache = [];

        private static PropertyInfo? GetCodigoProp(Type t)
        {
            if (!_codigoCache.TryGetValue(t, out var prop))
            {
                prop = t.GetProperty("Codigo");
                _codigoCache[t] = prop;
            }
            return prop;
        }
        private static PropertyInfo? GetIdProp(Type t)
        {
            if (!_idCache.TryGetValue(t, out var prop))
            {
                prop = t.GetProperty("Id");
                _idCache[t] = prop;
            }
            return prop;
        }

        public static async Task<(bool inserted, bool updated)> SaveAsync<T>(T item)
    where T : class, IEntity
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (string.IsNullOrWhiteSpace(item.Codigo))
                throw new ArgumentException("Código da mídia inválido.");

            // tenta localizar por código
            var existente = await DB.Select<T>()
                .Where(x => x.Codigo == item.Codigo)
                .FirstAsync();

            // INSERT
            if (existente == null)
            {
                // garante que o ID será retornado
                var id = await DB.Insert(item).ExecuteIdentityAsync();

                item.Id = Convert.ToInt64(id);

                return (true, false);
            }

            // UPDATE
            item.Id = existente.Id;

            var rows = await DB.Update<T>()
                .SetSource(item)
                .Where(x => x.Id == existente.Id)
                .ExecuteAffrowsAsync();

            return (false, rows > 0);
        }

        // --- CONSULTAS ---
        public static async Task<T?> GetByIdAsync<T>(long id) where T : class
            => await DB.Select<T>()
                .WhereDynamic(new { Id = id })
                .FirstAsync();

        public static async Task<T?> GetFirstAsync<T>() where T : class
            => await DB.Select<T>()
                .OrderBy("id")
                .FirstAsync();

        public static T? GetNext<T>(long id) where T : class
            => DB.Select<T>()
                .Where("id > @id", new { id })
                .OrderBy("id")
                .First();

        public static T? GetPrevious<T>(long id) where T : class
            => DB.Select<T>()
                .Where("id < @id", new { id })
                .OrderBy("id DESC")
                .First();

        public static bool Any<T>() where T : class
            => DB.Select<T>().Any();

        // anyasync
        public static async Task<bool> AnyAsync<T>() where T : class
            => await DB.Select<T>().AnyAsync();
    }

}