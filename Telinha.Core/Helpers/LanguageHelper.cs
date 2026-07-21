using Telinha.Core.Cache;
using Telinha.Core.Mapper;

namespace Telinha.Core.Helpers
{
    public static class LanguageHelper
    {
        public static async Task<string> ResolveAsync(
       string? englishName,
       string? isoCode,
       Func<string, Task<string?>> translateFunc)
        {
            var raw = englishName ?? isoCode;

            if (string.IsNullOrWhiteSpace(raw)) return "--";

            var mapped = LanguageMapper.TryMap(raw);
            if (mapped != null)
                return mapped;

            if (!string.IsNullOrWhiteSpace(englishName))
            {
                mapped = LanguageMapper.TryMap(englishName);
                if (mapped != null)
                    return mapped;

            }

            try
            {
                var translated = await translateFunc(raw);

                if (!string.IsNullOrWhiteSpace(translated))
                    return translated;
            }
            catch
            {
            }

            return raw;
        }
    }
}