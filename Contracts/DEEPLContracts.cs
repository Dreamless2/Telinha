using DeepL;

namespace Telinha.Contracts
{
    public class DEEPLContracts(DeepLClient client)
    {
        private readonly DeepLClient _client = client;

        public async Task<string?> Translate(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text == "--")
                return null;

            try
            {
                var result = await _client.TranslateTextAsync(text, null, LanguageCode.PortugueseBrazilian);
                return result.Text;
            }
            catch (DeepLException ex) when (ex.Message.Contains("Not found"))
            {
                return text;
            }
        }
    }
}
