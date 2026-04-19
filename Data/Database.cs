using FreeSql;

namespace Telinha.Data
{
    public static class Database
    {
        private static IFreeSql? _db;
        private static readonly object _lock = new();
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

            // 🔹 Garante que o arquivo existe (seguro)
            if (!File.Exists(DbPath))
            {
                using (File.Create(DbPath)) { }
            }

            // 🔹 Configura FreeSql
            _db = new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, $"Data Source={DbPath}")
                .UseAutoSyncStructure(false)
                .Build();

            // 🔹 Criação de estrutura
            CreateTablesIfNotExist();
            CreateTriggers();
        }

        private static void CreateTablesIfNotExist()
        {
            const string sqlMidia = @"
CREATE TABLE IF NOT EXISTS midia (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    codigo TEXT,
    nome TEXT,
    audio TEXT CHECK(audio IN ('Dublado', 'Legendado', 'Nacional', 'Desconhecido')),
    tipo TEXT CHECK(tipo IN ('Filme', 'Serie', 'Anime')),    
    sinopse TEXT,
    original TEXT,
    lancamento TEXT,
    alternativo TEXT,
    serie TEXT,
    pais TEXT,
    idioma TEXT,
    franquia TEXT,
    autores TEXT,
    criadores TEXT,
    obra TEXT,
    genero TEXT,
    tags TEXT,
    diretor TEXT,
    artistas TEXT,
    produtora TEXT,
    mcu TEXT,
    created_at TEXT DEFAULT CURRENT_TIMESTAMP,
    updated_at TEXT DEFAULT CURRENT_TIMESTAMP
);";

            _db!.Ado.ExecuteNonQuery(sqlMidia);

            const string sqlCache = @"
CREATE TABLE IF NOT EXISTS cache (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    type TEXT,
    midia_id INTEGER,
    json TEXT,
    updated_at INTEGER,
    UNIQUE(type, midia_id)
);";

            _db.Ado.ExecuteNonQuery(sqlCache);

            // 🔹 Índice para performance/cache cleanup
            const string idxCache = @"
CREATE INDEX IF NOT EXISTS idx_cache_updated 
ON cache(updated_at);";

            _db.Ado.ExecuteNonQuery(idxCache);
        }

        private static void CreateTriggers()
        {
            // 🔹 Atualiza updated_at automaticamente (sem loop)
            const string updated = @"
CREATE TRIGGER IF NOT EXISTS trg_midia_updated
AFTER UPDATE ON midia
FOR EACH ROW
WHEN NEW.updated_at = OLD.updated_at
BEGIN
    UPDATE midia 
    SET updated_at = CURRENT_TIMESTAMP 
    WHERE id = OLD.id;
END;
";
            _db!.Ado.ExecuteNonQuery(updated);

            // ❌ Removido trigger de created_at (já é DEFAULT)
        }
    }
}