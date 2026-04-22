using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Telinha.Services
{
    public sealed class TokenEncryptionServices : IDisposable
    {
        private readonly byte[] _key;

        private const int NonceSize = 12; // 96 bits
        private const int TagSize = 16;   // 128 bits
        private const byte Version = 1;

        public TokenEncryptionServices(byte[] key)
        {
            if (key == null || key.Length != 32)
                throw new ArgumentException("A chave deve ter exatamente 32 bytes.", nameof(key));



            _key = (byte[])key.Clone();
        }

        public string Encrypt(string plainToken, string? aad = null)
        {
            if (string.IsNullOrWhiteSpace(plainToken))
                throw new ArgumentException("Token inválido.", nameof(plainToken));

            byte[] plaintext = Encoding.UTF8.GetBytes(plainToken);
            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[TagSize];

            byte[]? aadBytes = aad != null ? Encoding.UTF8.GetBytes(aad) : null;

            using (var aes = new AesGcm(_key, TagSize))
            {
                aes.Encrypt(nonce, plaintext, ciphertext, tag, aadBytes);
            }

            // Layout: [Version][Nonce][Ciphertext][Tag]
            byte[] result = new byte[1 + NonceSize + ciphertext.Length + TagSize];

            result[0] = Version;
            Buffer.BlockCopy(nonce, 0, result, 1, NonceSize);
            Buffer.BlockCopy(ciphertext, 0, result, 1 + NonceSize, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, result, 1 + NonceSize + ciphertext.Length, TagSize);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string encryptedTokenBase64, string? aad = null)
        {
            if (string.IsNullOrWhiteSpace(encryptedTokenBase64))
                throw new ArgumentException("Token inválido.");

            byte[] data;

            try
            {
                data = Convert.FromBase64String(encryptedTokenBase64);
            }
            catch
            {
                throw new SecurityException("Token não está em Base64 válido.");
            }

            if (data.Length < 1 + NonceSize + TagSize)
                throw new SecurityException("Token inválido.");

            byte version = data[0];

            if (version != Version)
                throw new SecurityException($"Versão do token não suportada: {version}");

            int cipherLength = data.Length - 1 - NonceSize - TagSize;

            byte[] nonce = new byte[NonceSize];
            byte[] ciphertext = new byte[cipherLength];
            byte[] tag = new byte[TagSize];

            Buffer.BlockCopy(data, 1, nonce, 0, NonceSize);
            Buffer.BlockCopy(data, 1 + NonceSize, ciphertext, 0, cipherLength);
            Buffer.BlockCopy(data, 1 + NonceSize + cipherLength, tag, 0, TagSize);

            byte[] plaintext = new byte[cipherLength];
            byte[]? aadBytes = aad != null ? Encoding.UTF8.GetBytes(aad) : null;

            try
            {
                using var aes = new AesGcm(_key, TagSize);
                aes.Decrypt(nonce, ciphertext, tag, plaintext, aadBytes);
            }
            catch (CryptographicException)
            {
                throw new SecurityException("Token inválido ou corrompido.");
            }

            return Encoding.UTF8.GetString(plaintext);
        }

        public void Dispose()
        {
            if (_key != null)
                CryptographicOperations.ZeroMemory(_key);
        }
    }
}