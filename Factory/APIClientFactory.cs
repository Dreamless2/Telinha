using DeepL;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Telinha.Contracts;
using Telinha.Services;

namespace Telinha.Factory
{
    public class ApiClientFactory
    {
        private readonly TokenServices _tokenService;

        private DeepLClient? _deepLClient;
        private RestClient? _tmdbClient;

        public ApiClientFactory(TokenServices tokenService)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

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

                if (string.IsNullOrWhiteSpace(token))
                    throw new InvalidOperationException("Token TMDB não configurado.");

                _tmdbClient = new RestClient("https://api.themoviedb.org/3/");

                _tmdbClient.AddDefaultHeader("Authorization", $"Bearer {token}");
                _tmdbClient.AddDefaultHeader("accept", "application/json");
            }

            return new TMDBServices(_tmdbClient);
        }
    }
}