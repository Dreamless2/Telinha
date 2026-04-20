using DeepL;
using System;
using System.Collections.Generic;
using System.Text;
using Telinha.Services;

namespace Telinha.Factory
{
    public class APIClientFactory
    {
        private readonly TokenService _tokenService;

        // 🔥 cache dos clients (evita recriar toda hora)
        private DeepLClient? _deepLClient;
        private HttpClient? _tmdbClient;

        public ApiClientFactory(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // =========================
        // 🌍 DEEPL
        // =========================
        public async Task<DeepLClient> GetDeepLClientAsync()
        {
            if (_deepLClient != null)
                return _deepLClient;

            var key = await _tokenService.ObterTokenAsync("DEEPL");

            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Token do DeepL não configurado.");

            _deepLClient = new DeepLClient(key);

            return _deepLClient;
        }

        // =========================
        // 🎬 TMDB
        // =========================
        public async Task<HttpClient> GetTmdbClientAsync()
        {
            if (_tmdbClient != null)
                return _tmdbClient;

            var token = await _tokenService.ObterTokenAsync("TMDB");

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Token do TMDB não configurado.");

            _tmdbClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.themoviedb.org/3/")
            };

            _tmdbClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return _tmdbClient;
        }
    }
}