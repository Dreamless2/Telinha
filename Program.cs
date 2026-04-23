using Telinha.Data;
using Telinha.Factory;
using Telinha.Services;
using Telinha.Store;

namespace Telinha
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                var configStore = new SecureConfigStore();
                var config = configStore.Load();

                // 🔴 Se não tem config → abre tela direto
                if (config == null)
                {
                    Application.Run(new Token());
                    return;
                }

                // 🔧 testa conexão antes de tudo
                try
                {
                    var db = Database.DB;
                    db.Ado.ExecuteConnectTest();
                }
                catch
                {
                    Application.Run(new Token());
                    return;
                }

                // ✅ agora sim pode usar serviços
                using var tokenService = new TokenServices();
                var apiFactory = new ApiClientFactory(tokenService);

                var tmdb = tokenService.ObterTokenAsync("TMDB").GetAwaiter().GetResult();
                var deepl = tokenService.ObterTokenAsync("DEEPL").GetAwaiter().GetResult();

                bool temTokensValidos =
                    !string.IsNullOrWhiteSpace(tmdb) &&
                    !string.IsNullOrWhiteSpace(deepl);

                if (temTokensValidos)
                    Application.Run(new Home(apiFactory));
                else
                    Application.Run(new Token(tokenService));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro crítico ao iniciar: " + ex.Message);
            }
        }
    }
}