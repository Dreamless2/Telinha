using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Telinha.Factory;
using Telinha.Services;

namespace Telinha
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();


            LogServices.ConfigurarLog();
            try
            {
                var configService = new AppConfigServices();
                var config = configService.Load();

                // 🔴 valida primeiro
                if (config == null ||
                    string.IsNullOrWhiteSpace(config.Host) ||
                    string.IsNullOrWhiteSpace(config.Porta) ||
                    string.IsNullOrWhiteSpace(config.Usuario) ||
                    string.IsNullOrWhiteSpace(config.Senha) ||
                    string.IsNullOrWhiteSpace(config.TMDB) ||
                    string.IsNullOrWhiteSpace(config.DEEPL))
                {
                    Application.Run(new Token());
                    return;
                }

                // ✅ só agora cria
                var apiFactory = new ApiClientFactory();
                var cacheServices = container.Resolve<FileCacheServices>();

                using var scope = container.BeginLifetimeScope();
                var home = scope.Resolve<Home>();
                Application.Run(new Home(apiFactory, cacheServices));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro crítico ao iniciar:\n" + ex.Message);
            }
            finally
            {
                LogServices.EncerrarLog();
            }
        }
    }
}