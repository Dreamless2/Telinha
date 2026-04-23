using FreeSql;
using Telinha.Store;

namespace Telinha.Data
{
    public static class Database
    {
        private static IFreeSql? _db;
        private static readonly Lock _lock = new();

        public static IFreeSql DB
        {
            get
            {
                if (_db == null)
                {
                    lock (_lock)
                    {
                        if (_db == null)
                            Initialize();
                    }
                }

                return _db!;
            }
        }

        private static string BuildConnectionString()
        {
            var configStore = new SecureConfigStore();
            var config = configStore.Load();

            if (config == null)
                throw new Exception("Configuração não encontrada.");

            return $"Data Source={config.Host};" +
                   $"Port={config.Porta};" +
                   $"User ID={config.Usuario};" +
                   $"Password={config.Senha};" +
                   $"Initial Catalog=telinha;";
        }

        public static void Initialize()
        {
            if (_db != null)
                return;

            _db = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, ConnStr)
                .UseAutoSyncStructure(false)
                .Build();

        }
    }
}