using Newtonsoft.Json.Linq;
using RestSharp;

namespace Telinha.Services
{
    public class TMDBServices()
    {
        private readonly string? _token;
        private readonly RestClient? _client;


        public TMDBServices(RestClient client, string token)
        {
            _client = client;
            _token = token;
        }
    }

        