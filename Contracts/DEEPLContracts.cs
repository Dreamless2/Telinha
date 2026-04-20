using DeepL;
using DeepL.Model;
using Telinha.Services;

namespace Telinha.Contracts
{
    public class DEEPLContracts
    {
        private readonly DeepLClient _client;
        private readonly TokenServices _tokenService;

        public DEEPLContracts(TokenServices tokenService)
        {
            _tokenService = tokenService;
            var deeplKey = Task.Run(async () => await _tokenService.ObterTokenAsync("DEEPL")).GetAwaiter().GetResult();

            if (string.IsNullOrWhiteSpace(deeplKey))
            {
                throw new InvalidOperationException("Chave API do DEEPL não configurado.");
            }

            _client = new DeepLClient(deeplKey);
        }

        public async Task<TextResult> Translate(string text) =>
            await _client.TranslateTextAsync(text, null, LanguageCode.PortugueseBrazilian);
    }
}