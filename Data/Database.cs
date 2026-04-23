using FreeSql;

namespace Telinha.Data
{
    public static class Database
    {
        private static IFreeSql? _db;
        private static readonly Lock _lock = new();

        private static readonly string ConnStr = "Data Source=localhost;Port=3306;User ID=root;Password=qdgdTJYiuYbzp8%n;Initial Catalog=telinha;";

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

            _db = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, ConnStr)
                .UseAutoSyncStructure(false)
                .Build();

        }
    }
}