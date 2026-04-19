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
    sinopse TEXT,
    original TEXT,
    lancamento TEXT,
    alternativo TEXT,
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

        // ====================== TABELA TOKENS (NOVA) ======================
        const string sqlTokens = @"
CREATE TABLE IF NOT EXISTS encrypted_tokens (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    key_name TEXT NOT NULL UNIQUE,                    -- identificador único do token
    encrypted_data TEXT NOT NULL,                     -- token criptografado em Base64
    description TEXT,                                 -- descrição opcional
    is_active BOOLEAN DEFAULT 1,                      -- para desativar sem apagar
    created_at TEXT DEFAULT CURRENT_TIMESTAMP,
    updated_at TEXT DEFAULT CURRENT_TIMESTAMP
);";

        _db.Ado.ExecuteNonQuery(sqlTokens);

        // Índice para busca rápida por key_name
        const string idxTokens = @"
CREATE INDEX IF NOT EXISTS idx_encrypted_tokens_key 
ON encrypted_tokens(key_name);";

        _db.Ado.ExecuteNonQuery(idxTokens);
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
        }
    }
}