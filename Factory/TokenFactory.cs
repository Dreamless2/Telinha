using Telinha.Data;
using Telinha.Helpers;
using Telinha.Infrastructure.Logging;
using Telinha.Models;
using Telinha.Services;

namespace Telinha.Factory
{
    public class TokenFactory
    {
        private readonly byte[] _masterKey;
        private readonly IFreeSql _fsql;

        public TokenFactory()
        {
            _masterKey = KeyHelper.GetOrCreateMasterKey();
            _fsql = Database.DB;
        }

        public async Task SalvarTokenAsync(string keyName, string plainToken, string? description = null, string? aad = null)
        {
            LogServices.Info("Salvando token: {KeyName}", keyName);

            if (string.IsNullOrWhiteSpace(keyName) || string.IsNullOrWhiteSpace(plainToken))
                throw new ArgumentException("KeyName e Token são obrigatórios.");

            using var encryptor = new TokenEncryptionServices(_masterKey);

            string encryptedBase64 = encryptor.Encrypt(plainToken, aad);

            var entity = new EncryptedToken
            {
                KeyName = keyName.Trim(),
                EncryptedData = encryptedBase64,
                Description = description?.Trim(),
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _fsql.InsertOrUpdate<EncryptedToken>()
                       .SetSource(entity)
                       .ExecuteAffrowsAsync();
        }

        public async Task<string?> ObterTokenAsync(string keyName, string? aad = null)
        {
            LogServices.Info("Obtendo token: {KeyName}", keyName);

            var entity = await _fsql.Select<EncryptedToken>()
                                   .Where(x => x.KeyName == keyName && x.IsActive)
                                   .FirstAsync();

            if (entity?.EncryptedData == null)
                return null;

            using var encryptor = new TokenEncryptionServices(_masterKey);

            try
            {
                var decrypted = encryptor.Decrypt(entity.EncryptedData, aad);
                return decrypted?.Trim().Replace("\0", "");
            }
            catch
            {
                LogServices.Error("Erro ao obter token: {KeyName}", keyName);
                return null;
            }
        }

        public void Dispose()
        {
            KeyHelper.ZeroMemory(_masterKey);
        }
    }
}
