using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Telinha.Helpers
{
    public class KeyHelper
    {
        private static readonly string KeyFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Assembly.GetExecutingAssembly().GetName().Name!,
                $"{Assembly.GetExecutingAssembly().GetName().Name!}.key");

        private const string Entropy = "telinha-app-v1";

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
                    File.Delete(KeyFilePath);
                }
            }

            byte[] masterKey = RandomNumberGenerator.GetBytes(32);

            byte[] protectedKey = ProtectedData.Protect(
                masterKey,
                Encoding.UTF8.GetBytes(Entropy),
                DataProtectionScope.CurrentUser);

            File.WriteAllBytes(KeyFilePath, protectedKey);

            File.SetAttributes(KeyFilePath, FileAttributes.Hidden);

            return masterKey;
        }
        public static void ZeroMemory(byte[] key)
        {
            CryptographicOperations.ZeroMemory(key);
        }
    }
}