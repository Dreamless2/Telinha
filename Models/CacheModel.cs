using FreeSql.DataAnnotations;

namespace Telinha.Models
{
    [Table(Name = "cache")]
    [Index("idx_cache_unique", "type, midia_id", true)]
    public class CacheModel
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Name = "type")]
        public string? Type { get; set; }

        [Column(Name = "midia_id")]
        public int MidiaId { get; set; }

        [Column(Name = "json")]
        public string? Json { get; set; }

        [Column(Name = "updated_at")]
        public long UpdatedAt { get; set; }
    }
}