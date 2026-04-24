using DeepL;
using RestSharp;
using Telinha.Contracts;
using Telinha.Services;

namespace Telinha.Factory
{
    public class ApiClientFactory
    {
        private readonly AppConfigServices.AppConfig _config;

        private DeepLClient? _deepLClient;
        private RestClient? _tmdbClient;

        public ApiClientFactory()
        {
            var config = new AppConfigServices().Load();

            if (config == null)
                throw new InvalidOperationException("Configuração não encontrada.");

            _config = config;
        }

        public async Task<DEEPLContracts> GetDeepLAsync() { if (_deepLClient == null) { var key = await _tokenService.ObterTokenAsync("DEEPL"); if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("Chave API do DeepL não configurada."); _deepLClient = new DeepLClient(key); } return new DEEPLContracts(_deepLClient); }
        vices GetTMDB()
        {
            if (_tmdbClient == null)
            {
                if (string.IsNullOrWhiteSpace(_config.TMDB))
                    throw new InvalidOperationException("Chave API do TMDB não configurada.");

                _tmdbClient = new RestClient("https://api.themoviedb.org/3/");
                _tmdbClient.AddDefaultParameter("api_key", _config.TMDB);
            }

            return new TMDBServices(_tmdbClient, _config.TMDB!);
        }
    }
}