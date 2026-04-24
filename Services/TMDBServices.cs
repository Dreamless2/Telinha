using Newtonsoft.Json.Linq;
using RestSharp;

namespace Telinha.Services
{
    public class TMDBServices(RestClient client, string token)
    {
        private readonly string _token = token;
        private readonly RestClient _client = client;

        private bool IsBearer => _token.StartsWith("eyJ");

        public async Task<JObject> GetAsync(string endpoint, Dictionary<string, string>? query = null)
        {
            var request = new RestRequest(endpoint);

            // 🔥 Detecta automaticamente
            if (IsBearer)
            {
                request.AddHeader("Authorization", $"Bearer {_token}");
            }
            else
            {
                request.AddQueryParameter("api_key", _token);
            }

            request.AddHeader("Accept", "application/json");

            if (query != null)
                foreach (var p in query)
                    request.AddQueryParameter(p.Key, p.Value);

            var resp = await _client.ExecuteAsync(request);

            // 🔎 Logs úteis
            LogServices.LogarInformacao("TMDB TipoAuth: {tipo}", IsBearer ? "Bearer" : "ApiKey");
            LogServices.LogarInformacao("TMDB Status: {status}", resp.StatusCode);
            LogServices.LogarInformacao("TMDB Response: {resp}", resp.Content!);

            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new JObject { ["status_code"] = 34, ["success"] = false };

            if (!resp.IsSuccessful || string.IsNullOrWhiteSpace(resp.Content))
                throw new Exception($"TMDB ERROR {resp.StatusCode}: {endpoint}\nContent: {resp.Content}");

            return JObject.Parse(resp.Content!);
        }

        public async Task<JObject[]> Many(
      params (string url, Dictionary<string, string>? q)[] calls,
      CancellationToken ct = default)
        {
            return await Task.WhenAll(
                calls.Select(c => GetAsync(c.url, c.q, ct))
            );
        }
    }
}