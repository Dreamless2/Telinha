using System.Reflection;
using Telinha.Data;

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

        public static async Task<(bool inserted, bool updated)> SaveAsync<T>(T item) where T : class
        {
            var type = typeof(T);

            var propCodigo = GetCodigoProp(type);
            var propId = GetIdProp(type);

            var codigoValue = propCodigo?.GetValue(item)?.ToString();

            if (string.IsNullOrWhiteSpace(codigoValue))
                throw new ArgumentException("Código da mídia inválido.");

            var existente = await DB.Select<T>()
                .WhereDynamic(new { Codigo = codigoValue })
                .FirstAsync();

            if (existente == null)
            {
                await DB.Insert(item).ExecuteAffrowsAsync();
                return (true, false);
            }

            var idExistente = propId?.GetValue(existente);
            propId?.SetValue(item, idExistente);

            var rows = await DB.Update<T>()
                .SetSource(item)
                .WhereDynamic(new { Id = idExistente })
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

        public async static Task<T?> GetNext<T>(long id) where T : class
            => await DB.Select<T>()
                .Where("id > @id", new { id })
                .OrderBy("id")
                .FirstAsync();

        public async static Task<T?> GetPrevious<T>(long id) where T : class
            => await DB.Select<T>()
                .Where("id < @id", new { id })
                .OrderBy("id DESC")
                .FirstAsync();

        public static bool Any<T>() where T : class
            => DB.Select<T>().Any();

        // anyasync
        public static async Task<bool> AnyAsync<T>() where T : class
            => await DB.Select<T>().AnyAsync();


        public static async Task<bool> ExistsNext<T>(long id) where T : class
            => await DB.Select<T>()
                .Where("id > @id", new { id })
                .AnyAsync(); // SELECT 1 WHERE EXISTS... bem mais rápido que trazer o objeto

        public static async Task<bool> ExistsPrevious<T>(long id) where T : class
            => await DB.Select<T>()
                .Where("id < @id", new { id })
                .AnyAsync();

    }
}