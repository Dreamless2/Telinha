using Telinha.Data;

namespace Telinha.Controller
{
    public static class MidiaController
    {
        private static IFreeSql DB => Database.DB;

        /// <summary>
        /// Salva qualquer mídia (Filme, Série, Anime) usando o Upsert do FreeSql.
        /// </summary>
        /// <returns>inserted: bool, updated: bool</returns>
        public static async Task<(bool inserted, bool updated)> SaveAsync<T>(T item) where T : class
        {
            // Usando Reflection bem de leve para pegar o 'Codigo' e o 'Id'
            var propCodigo = typeof(T).GetProperty("Codigo");
            var propId = typeof(T).GetProperty("Id");

            if (propCodigo == null || string.IsNullOrWhiteSpace(propCodigo.GetValue(item)?.ToString()))
            {
                throw new ArgumentException("Código da mídia inválido.");
            }

            // 1. Tenta buscar o existente para saber se é Insert ou Update
            var codigoValue = propCodigo.GetValue(item)?.ToString();
            var existente = await DB.Select<T>()
                .WhereDynamic(new { Codigo = codigoValue }) // Busca dinâmica pelo código
                .FirstAsync();

            if (existente == null)
            {
                // INSERT
                await DB.Insert(item).ExecuteAffrowsAsync();
                return (true, false);
            }

            // 2. Se existe, sincroniza o ID para garantir que o Update saiba onde bater
            var idExistente = propId?.GetValue(existente);
            propId?.SetValue(item, idExistente);

            // 3. UPDATE INTELIGENTE (Substitui o seu ChangeTracker manual)
            // O método 'Update(item).NoneParameter().ExecuteAffrows()' do FreeSql 
            // pode ser configurado, mas o jeito mais seguro sem o tracker é:
            var rows = await DB.Update<T>()
                .SetSource(item)
                .ExecuteAffrowsAsync();

            return (false, rows > 0);
        }

        // --- CONSULTAS GENÉRICAS ---

        public static async Task<T?> GetByIdAsync<T>(long id) where T : class
            => await DB.Select<T>()
                .WhereDynamic(new { Id = id })
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
    }
}