using Telinha.Factory;
using Telinha.Infrastructure.Logging;
using Telinha.Services;

namespace Telinha
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            LogServices.Configure();

            try
            {
                using var tokenService = new TokenServices();
                var apiFactory = new ApiClientFactory(tokenService);

                var tmdb = tokenService.ObterTokenAsync("TMDB").GetAwaiter().GetResult();
                var deepl = tokenService.ObterTokenAsync("DEEPL").GetAwaiter().GetResult();

                bool temTokensValidos = !string.IsNullOrWhiteSpace(tmdb) && !string.IsNullOrWhiteSpace(deepl);

                if (temTokensValidos)
                {
                    LogServices.Info("Chaves encontrados. Iniciando Home...");
                    Application.Run(new Home(apiFactory));
                }
                else
                {
                    LogServices.Info("Chaves ausentes. Abrindo tela de configuração...");
                    Application.Run(new Token(tokenService));
                }
            }
            catch (Exception ex)
            {
                LogServices.Error(ex, "Erro fatal na inicialização do sistema.");
                MessageBox.Show("Erro crítico ao iniciar: " + ex.Message);
            }
            finally
            {
                LogServices.Close();
            }
        }
    }
}
