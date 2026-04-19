using System;
using System.Collections.Generic;
using System.Text;
using Telinha.Data;
using Telinha.Helpers;
using Telinha.Models;
using Telinha.Services;

namespace Telinha.Factory
{
    public class TokenFactory
    {
        private readonly byte[] _masterKey;   // chave AES protegida por DPAPI
        private readonly IFreeSql _fsql;

        public TokenFactory()
        {
            _masterKey = KeyHelper.GetOrCreateMasterKey();
            _fsql = Database.DB;
        }

        public async Task SalvarTokenAsync(string keyName, string plainToken, string? description = null, string? aad = null)
        {
            if (string.IsNullOrWhiteSpace(keyName) || string.IsNullOrWhiteSpace(plainToken))
                throw new ArgumentException("KeyName e Token são obrigatórios.");

            using var encryptor = new TokenEncryptionService(_masterKey);

            string encryptedBase64 = encryptor.Encrypt(plainToken, aad);

            var entity = new EncryptedToken
            {
                KeyName = keyName.Trim(),
                EncryptedData = encryptedBase64,
                Description = description?.Trim(),
                IsActive = true
            };

            await _fsql.InsertOrUpdate<EncryptedToken>()
           .SetSource(entity)
           .Where<EncryptedToken>(x => x.KeyName == keyName)   // ← Adicione <EncryptedToken>
           .ExecuteAffrowsAsync();
        }

        public async Task<string?> ObterTokenAsync(string keyName, string? aad = null)
        {
            var entity = await _fsql.Select<EncryptedToken>()
                                   .Where(x => x.KeyName == keyName && x.IsActive)
                                   .FirstAsync();

            if (entity?.EncryptedData == null)
                return null;

            using var encryptor = new TokenEncryptionService(_masterKey);

            try
            {
                return encryptor.Decrypt(entity.EncryptedData, aad);
            }
            catch
            {
                return null; // token inválido ou chave mudou
            }
        }

        // Método para limpar memória quando a aplicação fechar (opcional)
        public void Dispose()
        {
            KeyHelper.ZeroMemory(_masterKey);
        }
    }
}
