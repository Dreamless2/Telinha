using Newtonsoft.Json.Linq;
using RestSharp;
using Telinha.Infrastructure.Logging;

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
            var request = new RestRequest(endpoint);
            request.AddQueryParameter("api_key", _token);

            if (query != null)
                foreach (var p in query)
                    request.AddQueryParameter(p.Key, p.Value);

            LogServices.Info($"Consultando API: {endpoint}", endpoint);

            var resp = await _client.ExecuteAsync(request);

            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new JObject { ["status_code"] = 34, ["success"] = false };

            if (!resp.IsSuccessful || string.IsNullOrWhiteSpace(resp.Content))
                LogServices.Warn("Falha na chamada: {Status} - {Content}", resp.StatusCode, resp.Content!);
            throw new Exception($"TMDB ERROR {resp.StatusCode}: {endpoint}\nContent: {resp.Content}");

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
;