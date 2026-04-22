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

                var tokenService = new TokenServices();
                var apiFactory = new ApiClientFactory(tokenService);
                var hasToken = Task.Run(async () =>
                {
                    var tmdb = await tokenService.ObterTokenAsync("TMDB");
                    var deepl = await tokenService.ObterTokenAsync("DEEPL");

                    return !string.IsNullOrEmpty(tmdb) || !string.IsNullOrEmpty(deepl);
                }).GetAwaiter().GetResult();

                if (hasToken)
                    Application.Run(new Home(apiFactory));
                else
                    Application.Run(new Token(tokenService));
            }
            LogServices.Close();
        }
    }
}
