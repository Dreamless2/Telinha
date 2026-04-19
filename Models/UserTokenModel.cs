using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Telinha.Models
{
    [Table("UserTokens")]
    public class UserTokenModel
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        [Column(StringLength = 100)]
        public string TokenName { get; set; } = string.Empty;        // Ex: "AccessToken", "RefreshToken", "ApiKey"

        [Column(StringLength = -1)]           // -1 = TEXT sem limite no SQLite
        public string EncryptedValue { get; set; } = string.Empty;   // ← Aqui vai o Base64 do Encrypt()

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }                     // opcional, mas muito recomendado

        [Column(StringLength = 50)]
        public string? UserId { get; set; }                          // se for por usuário

        public bool IsRevoked { get; set; } = false;

        [Column(StringLength = 50)]
        public string? Version { get; set; } = "1";                  // caso você mude o esquema de criptografia no futuro
    }
}
