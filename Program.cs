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

            // 1. Inicia o Log primeiro de tudo
            LogServices.Configure();

            try
            {
                // 2. Instancia os serviços base
                using var tokenService = new TokenServices();
                var apiFactory = new ApiClientFactory(tokenService);

                // 3. Checagem de tokens (Sincronizada de forma limpa)
                // Usar .GetAwaiter().GetResult() na Main é aceitável, 
                // mas certifique-se de que o ObterTokenAsync não use o contexto de UI.
                var tmdb = tokenService.ObterTokenAsync("TMDB").GetAwaiter().GetResult();
                var deepl = tokenService.ObterTokenAsync("DEEPL").GetAwaiter().GetResult();

                bool temTokensValidos = !string.IsNullOrWhiteSpace(tmdb) && !string.IsNullOrWhiteSpace(deepl);

                if (temTokensValidos)
                {
                    LogServices.Info("Tokens encontrados. Iniciando Home...");
                    Application.Run(new Home(apiFactory));
                }
                else
                {
                    LogServices.Info("Tokens ausentes. Abrindo tela de configuração...");
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
                // 4. Garante que o log escreva tudo no disco antes de fechar o processo
                LogServices.Close();
            }
        }
    }
}
