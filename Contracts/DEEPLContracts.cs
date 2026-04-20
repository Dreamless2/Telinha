using DeepL;
using DeepL.Model;

namespace Telinha.Contracts
{
    public class DEEPLContracts
    {
        private readonly DeepLClient _client;

        public DEEPLContracts()
        {
            var deeplKey = "7feb3eb8-de95-4312-843c-1064aecdab8b:fx";

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