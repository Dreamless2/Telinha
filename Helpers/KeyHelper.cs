using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Telinha.Models;
using Telinha.Services;

namespace Telinha.Helpers
{

    public class KeyHelper
    {
        private static readonly string KeyFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Assembly.GetExecutingAssembly().GetName().Name,
                "master.key");

        private const string Entropy = "telinha-app-v1"; // entropy extra (opcional, mas recomendado)

        /// <summary>
        /// Gera ou carrega a chave mestra de 32 bytes protegida por DPAPI
        /// </summary>
        public static byte[] GetOrCreateMasterKey()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(KeyFilePath)!);

            if (File.Exists(KeyFilePath))
            {
                try
                {
                    byte[] protectedData = File.ReadAllBytes(KeyFilePath);
                    return ProtectedData.Unprotect(protectedData, Encoding.UTF8.GetBytes(Entropy), DataProtectionScope.CurrentUser);
                }
                catch
                {
                    // Se falhar (arquivo corrompido), cria uma nova chave
                    File.Delete(KeyFilePath);
                }
            }

            // Gera nova chave de 32 bytes (256 bits)
            byte[] masterKey = RandomNumberGenerator.GetBytes(32);

            // Protege com DPAPI
            byte[] protectedKey = ProtectedData.Protect(
                masterKey,
                Encoding.UTF8.GetBytes(Entropy),
                DataProtectionScope.CurrentUser);

            File.WriteAllBytes(KeyFilePath, protectedKey);

            // Protege o arquivo contra outros usuários
            File.SetAttributes(KeyFilePath, FileAttributes.Hidden);

            return masterKey;
        }

        /// <summary>
        /// Limpa a chave da memória (boa prática)
        /// </summary>
        public static void ZeroMemory(byte[] key)
        {
            CryptographicOperations.ZeroMemory(key);
        }

        public async Task<string?> ObterTokenAsync(string keyName, string? aad = null)
        {
            if (_cache.TryGetValue(keyName, out var cached))
                return cached;

            var entity = await _fsql.Select<EncryptedToken>()
                                    .Where(x => x.KeyName == keyName && x.IsActive)
                                    .FirstAsync();

            if (entity == null)
                return null;

            using var encryptor = new TokenEncryptionService(_masterKey);

            var decrypted = encryptor.Decrypt(entity.EncryptedData, aad);

            _cache[keyName] = decrypted;

            return decrypted;
        }
    }
}