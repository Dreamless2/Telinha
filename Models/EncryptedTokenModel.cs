using FreeSql.DataAnnotations;

namespace Telinha.Models
{
    [Table(Name = "encrypted_tokens")]
    [Index("uk_encrypted_tokens_keyname", nameof(KeyName), true)]
    public class EncryptedToken
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        [Column(StringLength = 100, IsNullable = false, Name = "key_name")]
        public string KeyName { get; set; } = string.Empty;

        [Column(StringLength = -1, IsNullable = false, Name = "encrypted_data")]
        public string EncryptedData { get; set; } = string.Empty;

        [Column(Name = "created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(Name = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column(StringLength = 20, IsNullable = false, Name = "encryption_version")]
        public string EncryptionVersion { get; set; } = "1";

        public string? Description { get; set; }

        [Column(Name = "is_active")]
        public bool IsActive { get; set; } = true;
    }
}