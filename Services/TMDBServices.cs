using Newtonsoft.Json.Linq;
using RestSharp;

namespace Telinha.Services
{
    public class TMDBServices
    {
        private readonly string _token;
        private readonly RestClient _client;

        public TMDBServices(RestClient client, string token)
        {
            _client = client;
            _token = token;
        }

        public async Task<JObject> GetAsync(string endpoint, Dictionary<string, string>? query = null)
        {
            // 1. Limpeza radical do token
            // Remove espaços, quebras de linha e o caractere nulo (\0) que a descriptografia pode deixar
            string cleanToken = _token?.Trim()
                                .Replace("\0", "")
                                .Replace("\r", "")
                                .Replace("\n", "") ?? "";

            var request = new RestRequest(endpoint);
            request.AddQueryParameter("api_key", "ae0d6c15bfcd81b3c140023da8da64c9");

            if (query != null)
            {
                foreach (var p in query)
                    request.AddQueryParameter(p.Key, p.Value);
            }

            var resp = await _client.ExecuteAsync(request);

            if (!resp.IsSuccessful || string.IsNullOrWhiteSpace(resp.Content))
            {
                // Se der erro, vamos capturar exatamente o que o servidor respondeu
                throw new Exception($"TMDB ERROR {resp.StatusCode}: {endpoint}\nContent: {resp.Content}");
            }

            return JObject.Parse(resp.Content!);
        }

        public async Task<JObject[]> Many(params (string url, Dictionary<string, string>? q)[] calls)
        {
            return await Task.WhenAll(
                calls.Select(c => GetAsync(c.url, c.q))
            );
        }
    }
}
