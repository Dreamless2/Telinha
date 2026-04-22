using DeepL;
using DeepL.Model;

namespace Telinha.Contracts
{
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
            // Se não achou tradução, retorna o original
            return text;
        }
    }
}
