using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Telinha.Models
{
    [Table(Name = "encryptedtokens")]
    public class EncryptedToken
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Nome identificador do token (ex: "SpotifyAccess", "OpenAIKey", "ApiTokenPrincipal")
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
        /// Versão da criptografia (útil se você mudar o algoritmo no futuro)
        /// </summary>
        [Column(StringLength = 20)]
        public string EncryptionVersion { get; set; } = "1";

        // Campos opcionais úteis
        public string? Description { get; set; }          // Ex: "Token principal da API XYZ"
        public bool IsActive { get; set; } = true;
    }
}