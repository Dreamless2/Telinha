using Autofac;
using Telinha.Container;
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

                var home = scope.Resolve<Home>();
                Application.Run(home);
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