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
        /*public async Task<JObject> GetAsync(string endpoint, Dictionary<string, string>? query = null)
        {
            var req = new RestRequest(endpoint);


            req.AddHeader("accept", "application/json");
            req.AddHeader("Authorization", $"Bearer {_token}");

            if (query != null)
            {
                foreach (var p in query)
                    req.AddQueryParameter(p.Key, p.Value);
            }

            var resp = await _client.ExecuteAsync(req);

            if (!resp.IsSuccessful || string.IsNullOrWhiteSpace(resp.Content))
                throw new Exception($"TMDB ERROR: {endpoint} {resp.StatusCode}\n{resp.Content}");

            return JObject.Parse(resp.Content!);
        }*/

        public async Task<JObject> GetAsync(string endpoint, Dictionary<string, string>? query = null)
        {
            var req = new RestRequest(endpoint);
            req.AddHeader("accept", "application/json");

            // 🔥 Limpeza extrema antes de enviar
            string cleanToken = _token.Trim().Replace("\0", "").Replace("\r", "").Replace("\n", "");

            req.AddHeader("Authorization", $"Bearer {cleanToken}");

            var resp = await _client.ExecuteAsync(req);

            if (!resp.IsSuccessful)
            {
                // Se falhar, vamos ver exatamente o que foi enviado (CUIDADO EM PROD, use apenas para debug)
                throw new Exception($"TMDB ERROR {resp.StatusCode}: {resp.Content}");
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
