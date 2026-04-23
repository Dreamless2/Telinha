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
        private readonly TokenEncryptionServices _encryptor;
        private readonly IMemoryCache _cache;

        public TokenServices()
        {
            _fsql = Database.DB;
            _masterKey = KeyHelper.GetOrCreateMasterKey();
            _encryptor = new TokenEncryptionServices(_masterKey);

            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task SalvarTokenAsync(string keyName, string plainToken, string? description = null, string? aad = null)
        {
            string encryptedBase64 = _encryptor.Encrypt(plainToken, aad);

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

            // cache com expiração
            _cache.Set(keyName, plainToken, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });
        }

        public async Task<string?> ObterTokenAsync(string keyName, string? aad = null)
        {
            if (_cache.TryGetValue(keyName, out string cached))
                return cached;

            var entity = await _fsql.Select<EncryptedToken>()
                                    .Where(x => x.KeyName == keyName && x.IsActive)
                                    .FirstAsync();

            if (entity == null)
                return null;

            var decrypted = _encryptor.Decrypt(entity.EncryptedData, aad);

            _cache.Set(keyName, decrypted, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });

            return decrypted;
        }

        public async Task RemoverTokenAsync(string keyName)
        {
            await _fsql.Update<EncryptedToken>()
                       .Set(x => x.IsActive, false)
                       .Where(x => x.KeyName == keyName)
                       .ExecuteAffrowsAsync();

            _cache.Remove(keyName);
        }
        public void Dispose()
        {
            _encryptor.Dispose();
            KeyHelper.ZeroMemory(_masterKey);

            if (_cache is IDisposable d)
                d.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}