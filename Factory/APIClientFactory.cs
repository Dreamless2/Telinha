using DeepL;
using RestSharp;
using Telinha.Contracts;
using Telinha.Services;

namespace Telinha.Factory
{
    public class ApiClientFactory(TokenServices tokenService)
    {
        private readonly TokenServices _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));

        private DeepLClient? _deepLClient;
        private RestClient? _tmdbClient;

        // 🔹 DeepL
        public async Task<DEEPLContracts> GetDeepLAsync()
        {
            if (_deepLClient == null)
            {
                var key = await _tokenService.ObterTokenAsync("DEEPL");

                if (string.IsNullOrWhiteSpace(key))
                    throw new InvalidOperationException("Token DeepL não configurado.");

                _deepLClient = new DeepLClient(key);
            }

            return new DEEPLContracts(_deepLClient);
        }

        // 🔹 TMDB
        public async Task<TMDBServices> GetTMDBAsync()
        {
            if (_tmdbClient == null)
            {
                var token = await _tokenService.ObterTokenAsync("TMDB");
                MessageBox.Show(token ?? "TOKEN NULL");

                if (string.IsNullOrWhiteSpace(token))
                    throw new InvalidOperationException("Token TMDB não configurado.");

                _tmdbClient = new RestClient("https://api.themoviedb.org/3");

                _tmdbClient.AddDefaultHeader("Authorization", $"Bearer {token}");
                _tmdbClient.AddDefaultHeader("accept", "application/json");
            }

            var tokenFinal = (await _tokenService.ObterTokenAsync("TMDB"))?.Trim();

            return new TMDBServices(_tmdbClient, tokenFinal!);
        }
    }
}