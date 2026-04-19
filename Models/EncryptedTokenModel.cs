using FreeSql.DataAnnotations;

namespace Telinha.Models
{
    [Table(Name = "encrypted_tokens")]
    public class EncryptedToken
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Nome identificador do token 
        /// </summary>
        [Column(StringLength = 100)]
        public string KeyName { get; set; } = string.Empty;

        /// <summary>
        /// Token já criptografado em Base64 (resultado do Encrypt())
        /// </summary>
        [Column(StringLength = -1)]   // TEXT sem limite no SQLite
        public string EncryptedData { get; set; } = string.Empty;

        /// <summary>
        /// Quando o token foi salvo
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Versão da criptografia
        /// </summary>
        [Column(StringLength = 20)]
        public string EncryptionVersion { get; set; } = "1";

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}