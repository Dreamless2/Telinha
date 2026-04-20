using DeepL;
using DeepL.Model;

namespace Telinha.Contracts
{
    public class DEEPLContracts
    {
        private readonly DeepLClient _client;

        public DEEPLContracts(DeepLClient client)
        {
            _client = client;
        }

        public async Task<TextResult> Translate(string text) =>
            await _client.TranslateTextAsync(text, null, LanguageCode.PortugueseBrazilian);
    }
}