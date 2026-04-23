using System.Collections.Concurrent;
using Telinha.Data;
using Telinha.Helpers;
using Telinha.Models;

namespace Telinha.Services
{
    public class TokenServices : IDisposable
    {
        private readonly IFreeSql _fsql;
        private readonly byte[] _masterKey;

        private readonly ConcurrentDictionary<string, string> _cache = new();

        public TokenServices()
        {
            _fsql = Database.DB;
            _masterKey = KeyHelper.GetOrCreateMasterKey();
        }
        public async Task SalvarTokenAsync(string keyName, string plainToken, string? description = null, string? aad = null)
        {
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

            _cache[keyName] = plainToken;
        }

        public async Task<string?> ObterTokenAsync(string keyName, string? aad = null)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                return null;

            if (_cache.TryGetValue(keyName, out var cached))
                return cached;

            var entity = await _fsql.Select<EncryptedToken>()
                                    .Where(x => x.KeyName == keyName && x.IsActive)
                                    .FirstAsync();

            if (entity == null)
                return null;

            using var encryptor = new TokenEncryptionServices(_masterKey);

            var decrypted = encryptor.Decrypt(entity.EncryptedData, aad);

            _cache[keyName] = decrypted;

            return decrypted;
        }

        public async Task RemoverTokenAsync(string keyName)
        {
            await _fsql.Update<EncryptedToken>()
                       .Set(x => x.IsActive, false)
                       .Where(x => x.KeyName == keyName)
                       .ExecuteAffrowsAsync();

            _cache.TryRemove(keyName, out _);
        }

        public void LimparCache()
        {
            _cache.Clear();
        }
        public void Dispose()
        {
            KeyHelper.ZeroMemory(_masterKey);
            GC.SuppressFinalize(this);
        }
    }
}