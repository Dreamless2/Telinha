using FreeSql;

namespace Telinha.Data
{
    public static class Database
    {
        private static IFreeSql? _db;
        private static readonly string DbPath = "telinha.db";

        public static IFreeSql DB
        {
            get
            {
                if (_db == null)
                {
                    Initialize(); // Auto-inicializa se alguém chamar o DB antes
                }

                return _db!;
            }
        }
        public static void Initialize()
        {
            if (_db != null)
            {
                return;
            }

            // 1. Garante que o arquivo existe (mesma lógica que você já tinha)
            if (!File.Exists(DbPath))
            {
                File.Create(DbPath).Close();
            }

            // 2. Configura o FreeSql para SQLite
            _db = new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, $"Data Source={DbPath}")
                // Como você já tem os comandos CREATE TABLE e TRIGGERS manuais, 
                // deixamos o AutoSync desligado para o FreeSql respeitar seu SQL.
                .UseAutoSyncStructure(false)
                .Build();

            // 3. Executa a criação das tabelas se necessário (via SQL puro através do FreeSql)
            CreateTablesIfNotExist();
            CreateTriggers();
        }

        private static void CreateTablesIfNotExist()
        {
            // O FreeSql permite executar SQL puro via Ado.ExecuteNonQuery
            const string sql = @"
    CREATE TABLE IF NOT EXISTS midia (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        codigo TEXT,
        titulo TEXT,
        audio TEXT CHECK(audio IN ('Dublado', 'Legendado', 'Nacional', 'Desconhecido')),
        tipo TEXT CHECK(tipo IN ('filmes', 'series', 'animes')),
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

            _db!.Ado.ExecuteNonQuery(sql);

            // Nova Tabela de Cache (Unificada)
            const string sqlCache = @"
    CREATE TABLE IF NOT EXISTS cache (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        type TEXT,
        midia_id INTEGER,
        json TEXT,
        updated_at INTEGER,
        UNIQUE(type, midia_id) -- Isso garante que o Upsert funcione sem erros
    );";
            _db!.Ado.ExecuteNonQuery(sqlCache);

        }

        private static void CreateTriggers()
        {
            const string updated = @"
            CREATE TRIGGER IF NOT EXISTS trg_midia_updated
            AFTER UPDATE ON midia
            FOR EACH ROW
            BEGIN
                UPDATE midia SET updated_at = CURRENT_TIMESTAMP WHERE id = OLD.id;
            END;
            ";
            _db!.Ado.ExecuteNonQuery(updated);

            const string created = @"
            CREATE TRIGGER IF NOT EXISTS trg_midia_created
            BEFORE INSERT ON midia
            FOR EACH ROW
            BEGIN
                SELECT CASE WHEN NEW.created_at IS NULL THEN
                    NEW.created_at = CURRENT_TIMESTAMP
                END;
            END;
        ";
            _db!.Ado.ExecuteNonQuery(created);
        }
    }
}
