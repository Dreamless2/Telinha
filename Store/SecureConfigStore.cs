using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Telinha.Store
{
    public class SecureConfigStore
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppDomain.CurrentDomain.FriendlyName,
            $"{AppDomain.CurrentDomain.FriendlyName}.key");

        private const string Entropy = "telinha-config-v1";

        public class ConfigData
        {
            public string? Host { get; set; }
            public string? Porta { get; set; }
            public string? Usuario { get; set; }
            public string? Senha { get; set; }
        }

        public static void Save(ConfigData data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

            string json = JsonConvert.SerializeObject(data);

            byte[] protectedData = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(json),
                Encoding.UTF8.GetBytes(Entropy),
                DataProtectionScope.CurrentUser);

            File.WriteAllBytes(FilePath, protectedData);
            File.SetAttributes(FilePath, FileAttributes.Hidden);
        }

        public static ConfigData? Load()
        {
            if (!File.Exists(FilePath))
                return null;

            try
            {
                byte[] protectedData = File.ReadAllBytes(FilePath);

                byte[] raw = ProtectedData.Unprotect(
                    protectedData,
                    Encoding.UTF8.GetBytes(Entropy),
                    DataProtectionScope.CurrentUser);

                string json = Encoding.UTF8.GetString(raw);

                return JsonConvert.DeserializeObject<ConfigData>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}