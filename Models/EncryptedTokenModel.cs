using FreeSql.DataAnnotations;

namespace Telinha.Models
{
    [Table(Name = "encrypted_tokens")]
    public class EncryptedToken
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        [Column(StringLength = 100)]
        public string KeyName { get; set; } = string.Empty;

        [Column(StringLength = -1)]   // TEXT sem limite no SQLite
        public string EncryptedData { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(StringLength = 20)]
        public string EncryptionVersion { get; set; } = "1";

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}