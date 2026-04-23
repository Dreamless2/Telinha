using FreeSql;

namespace Telinha.Data
{
    public static class Database
    {
        private static IFreeSql? _db;
        private static readonly Lock _lock = new();
        private static readonly string DbPath = "telinha.db";
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
        public static void Initialize()
        {
            if (_db != null)
                return;

            if (!File.Exists(DbPath))
            {
                using (File.Create(DbPath)) { }
            }

            _db = new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, $"Data Source={DbPath}")
                .UseAutoSyncStructure(false)
                .Build();

            CreateTablesIfNotExist();
            CreateTriggers();
        }

       