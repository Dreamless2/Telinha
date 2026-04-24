using DeepL;
using RestSharp;
using Telinha.Contracts;
using Telinha.Services;

namespace Telinha.Factory
{
    public class ApiClientFactory()
    {
        private DeepLClient? _deepLClient;
        private RestClient? _tmdbClient;
        private readonly AppConfigServices.AppConfig _config;

        public ApiClientFactory()
        {
            var config = new AppConfigServices().Load();

            if (config == null)
                throw new InvalidOperationException("Configuração não encontrada.");

            _config = config;
        }

        public DEEPLContracts GetDeepL()
        {
            if (_deepLClient == null)
            {
                if (string.IsNullOrWhiteSpace(_config.DEEPL))
                    throw new InvalidOperationException("Chave API do DeepL não configurada.");

                _deepLClient = new DeepLClient(_config.DEEPL);
            }

            return new DEEPLContracts(_deepLClient);
        }

        public TMDBServices GetTMDB()
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