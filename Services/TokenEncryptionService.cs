using System.Security.Cryptography;
using System.Text;

namespace Telinha.Services
{
    public class TokenEncryptionService : IDisposable
    {
        private readonly byte[] _key;                    // Chave AES-256 (32 bytes)
        private readonly AesGcm _aesGcm;

        private const int NonceSize = 12;   // 96 bits (recomendado para GCM)
        private const int TagSize = 16;     // 128 bits (padrão)

        /// <summary>
        /// Construtor - recebe a chave mestra (32 bytes para AES-256)
        /// </summary>
        public TokenEncryptionService(byte[] key)
        {
            if (key == null || key.Length != 32)
                throw new ArgumentException("A chave deve ter exatamente 32 bytes.", nameof(key));

            _key = (byte[])key.Clone();           // cópia para segurança
            _aesGcm = new AesGcm(_key, TagSize);
        }

        /// <summary>
        /// Criptografa um token (string) e retorna em Base64
        /// </summary>
        public string Encrypt(string plainToken)
        {
            if (string.IsNullOrEmpty(plainToken))
                throw new ArgumentException("Não pode ser nulo ou vazio.", nameof(plainToken));

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainToken);

            byte[] nonce = new byte[NonceSize];
            byte[] ciphertext = new byte[plainBytes.Length];
            byte[] tag = new byte[TagSize];

            RandomNumberGenerator.Fill(nonce);                    // Nonce único por token!

            _aesGcm.Encrypt(nonce, plainBytes, ciphertext, tag);

            // Formato final: Nonce + Ciphertext + Tag  (tudo concatenado)
            byte[] result = new byte[NonceSize + ciphertext.Length + TagSize];

            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(ciphertext, 0, result, NonceSize, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, result, NonceSize + ciphertext.Length, TagSize);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Descriptografa o token a partir do Base64
        /// </summary>
        public string Decrypt(string encryptedTokenBase64)
        {
            if (string.IsNullOrEmpty(encryptedTokenBase64))
                throw new ArgumentException("Não pode ser nulo ou vazio.");

            byte[] encryptedData = Convert.FromBase64String(encryptedTokenBase64);

            if (encryptedData.Length < NonceSize + TagSize)
                throw new CryptographicException("Dados inválidos.");

            int cipherLength = encryptedData.Length - NonceSize - TagSize;

            byte[] nonce = new byte[NonceSize];
            byte[] ciphertext = new byte[cipherLength];
            byte[] tag = new byte[TagSize];

            // Extrai as partes
            Buffer.BlockCopy(encryptedData, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(encryptedData, NonceSize, ciphertext, 0, cipherLength);
            Buffer.BlockCopy(encryptedData, NonceSize + cipherLength, tag, 0, TagSize);

            byte[] plaintext = new byte[cipherLength];

            _aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

            return Encoding.UTF8.GetString(plaintext);
        }

        public void Dispose()
        {
            _aesGcm?.Dispose();
        }
    }
}
