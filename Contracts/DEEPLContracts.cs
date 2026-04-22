using DeepL;
using DeepL.Model;

namespace Telinha.Contracts
{
    public class DEEPLContracts(DeepLClient client)
    {
        private readonly DeepLClient _client = client;

        public async Task<TextResult> Translate(string text)
        {
            return await _client.TranslateTextAsync(
                text,
                null,
                LanguageCode.PortugueseBrazilian
            );
        }
    }
}
