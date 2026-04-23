using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Telinha.Services
{
    public class AppConfigServices
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SeuApp",
            "appconfig.key");

        private const string Entropy = "telinha-app-v1";

        public class AppConfig
        {
            public string? Host { get; set; }
            public string? Porta { get; set; }
            public string? Usuario { get; set; }
            public string? Senha { get; set; }

            public string? TMDB { get; set; }
            public string? DEEPL { get; set; }
        }

        public void Save(AppConfig config)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

            string json = JsonConvert.SerializeObject(config);

            byte[] protectedData = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(json),
                Encoding.UTF8.GetBytes(Entropy),
                DataProtectionScope.CurrentUser);

            File.WriteAllBytes(FilePath, protectedData);
            File.SetAttributes(FilePath, FileAttributes.Hidden);
        }

        public AppConfig? Load()
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

                return JsonConvert.DeserializeObject<AppConfig>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}