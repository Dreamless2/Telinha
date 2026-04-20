using Telinha.Services;

namespace Telinha
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var tokenService = new TokenServices();
            var hasToken = Task.Run(async () =>
            {
                var tmdb = await tokenService.ObterTokenAsync("TMDB");
                var deepl = await tokenService.ObterTokenAsync("DEEPL");

                return !string.IsNullOrEmpty(tmdb) || !string.IsNullOrEmpty(deepl);
            }).GetAwaiter().GetResult();





            Application.Run(new Token());
        }
    }
}
