using Autofac;
using Telinha.Core.Services;
using Telinha.Forms.Container;
using Telinha.Forms.Views;

namespace Telinha.Forms
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
                var container = ContainerConfig.Configure();
                using var scope = container.BeginLifetimeScope();

                if (config == null ||
                    string.IsNullOrWhiteSpace(config.Host) ||
                    string.IsNullOrWhiteSpace(config.Porta) ||
                    string.IsNullOrWhiteSpace(config.Usuario) ||
                    string.IsNullOrWhiteSpace(config.Senha) ||
                    string.IsNullOrWhiteSpace(config.TMDB) ||
                    string.IsNullOrWhiteSpace(config.DEEPL))
                {
                    var token = scope.Resolve<Token>();
                    Application.Run(token);
                    return;
                }

                var principal = scope.Resolve<Principal>();
                Application.Run(principal);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro crítico ao iniciar:\n" + ex.Message);
                LogServices.LogarErroComException(ex, "Erro crítico ao iniciar a aplicação.");
            }
            finally
            {
                LogServices.EncerrarLog();
            }
        }
    }
}