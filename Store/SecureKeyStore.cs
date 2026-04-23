using System;
using System.Collections.Generic;
using System.Text;
using Telinha.Services;

namespace Telinha.Store
{
    public sealed class SecureKeyStore
    {
        private readonly TokenEncryptionServices _crypto;
        private readonly string _filePath;

        public SecureKeyStore(TokenEncryptionServices crypto, string filePath)
        {
            _crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        /// <summary>
        /// Salva um valor criptografado no arquivo
        /// </summary>
        public void Save(string value, string? aad = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Valor inválido.");

            string encrypted = _crypto.Encrypt(value, aad);

            File.WriteAllText(_filePath, encrypted);
        }

        /// <summary>
        /// Lê e descriptografa o valor do arquivo
        /// </summary>
        public string Load(string? aad = null)
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException("Arquivo de chave não encontrado.", _filePath);

            string encrypted = File.ReadAllText(_filePath);

            return _crypto.Decrypt(encrypted, aad);
        }

        /// <summary>
        /// Verifica se o arquivo existe
        /// </summary>
        public bool Exists()
        {
            return File.Exists(_filePath);
        }

        /// <summary>
        /// Remove o arquivo com segurança básica
        /// </summary>
        public void Delete()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}