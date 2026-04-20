using Newtonsoft.Json.Linq;
using RestSharp;

namespace Telinha.Services
{
    public class TMDBServices(RestClient client)
    {
        private readonly RestClient _client = client;

        public async Task<JObject> GetAsync(string endpoint, Dictionary<string, string>? query = null)
        {
            var req = new RestRequest(endpoint);

            req.AddHeader("accept", "application/json");

            // 🔥 GARANTE o token aqui
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
        }

        public async Task<JObject[]> Many(params (string url, Dictionary<string, string>? q)[] calls)
        {
            return await Task.WhenAll(
                calls.Select(c => GetAsync(c.url, c.q))
            );
        }
    }
}