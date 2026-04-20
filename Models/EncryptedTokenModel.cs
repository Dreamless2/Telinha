using FreeSql.DataAnnotations;

namespace Telinha.Models
{
    [Table(Name = "encrypted_tokens")]
    [Index("uk_encrypted_tokens_keyname", nameof(KeyName), true)] // UNIQUE
    public class EncryptedToken
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Nome identificador do token (UNIQUE)
        /// </summary>
        [Column(StringLength = 100, IsNullable = false, Name = "key_name")]
        public string KeyName { get; set; } = string.Empty;

        /// <summary>
        /// Token criptografado em Base64
        /// </summary>
        [Column(StringLength = -1, IsNullable = false, Name = "encrypted_data")]

        public string EncryptedData { get; set; } = string.Empty;

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da última atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Versão da criptografia (para rotação de chave)
        /// </summary>
        [Column(StringLength = 20, IsNullable = false, Name = "encryption_version")]
        public string EncryptionVersion { get; set; } = "1";

        /// <summary>
        /// Descrição opcional
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Flag de ativação
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}