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

            try
            {
                using var tokenService = new TokenServices();
                var apiFactory = new ApiClientFactory(tokenService);

                var tmdb = tokenService.ObterTokenAsync("TMDB").GetAwaiter().GetResult();
                var deepl = tokenService.ObterTokenAsync("DEEPL").GetAwaiter().GetResult();

                bool temTokensValidos = !string.IsNullOrWhiteSpace(tmdb) && !string.IsNullOrWhiteSpace(deepl);

                if (temTokensValidos)
                    Application.Run(new Home(apiFactory));
                else
                {
                    Application.Run(new Token(tokenService));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro crítico ao iniciar: " + ex.Message);
            }
        }
    }
}
