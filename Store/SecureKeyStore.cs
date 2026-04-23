using Telinha.Services;

namespace Telinha.Store
{
    public class SecureKeyStore(TokenEncryptionServices crypto, string filePath)
    {
        private readonly TokenEncryptionServices _crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
        private readonly string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

        public void Save(string value, string? aad = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Valor inválido.");

            string encrypted = _crypto.Encrypt(value, aad);

            File.WriteAllText(_filePath, encrypted);
        }

        public string Load(string? aad = null)
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException("Arquivo de chave não encontrado.", _filePath);

            string encrypted = File.ReadAllText(_filePath);

            return _crypto.Decrypt(encrypted, aad);
        }

        public bool Exists()
        {
            return File.Exists(_filePath);
        }

        public void Delete()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}