using FreeSql;
using Telinha.Store;

namespace Telinha.Data
{
    public static class Database
    {
        private static IFreeSql? _db;
        private static readonly Lock _lock = new();

        private static string? _connStr;
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
            if (_connStr != null)
                return _connStr;

            var config = new SecureConfigStore().Load();

            if (config == null)
                throw new InvalidOperationException("Configuração não encontrada.");

            if (string.IsNullOrWhiteSpace(config.Host) ||
                string.IsNullOrWhiteSpace(config.Porta) ||
                string.IsNullOrWhiteSpace(config.Usuario) ||
                string.IsNullOrWhiteSpace(config.Senha))
            {
                throw new InvalidOperationException("Configuração inválida.");
            }

            _connStr =
                $"Data Source={config.Host};" +
                $"Port={config.Porta};" +
                $"User ID={config.Usuario};" +
                $"Password={config.Senha};" +
                $"Initial Catalog=telinha;";

            return _connStr;
        }

        private static void Initialize()
        {
            var connStr = BuildConnectionString();

            _db = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, connStr)
                .UseAutoSyncStructure(false)
                .Build();
        }
    }
}