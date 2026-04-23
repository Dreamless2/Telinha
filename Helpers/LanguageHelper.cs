using Telinha.Cache;
using Telinha.Mapper;

namespace Telinha.Helpers
{
    public static class LanguageHelper
    {
        public static async Task<string> ResolveAsync(
       string? englishName,
       string? isoCode,
       Func<string, Task<string?>> translateFunc)
        {
            var raw = englishName ?? isoCode;

            if (string.IsNullOrWhiteSpace(raw))
                return "--";

            if (TranslationCache.TryGet(raw, out var cached))
                return cached;

            var mapped = LanguageMapper.TryMap(raw);
            if (mapped != null)
            {
                TranslationCache.Set(raw, mapped);
                return mapped;
            }

            if (!string.IsNullOrWhiteSpace(englishName))
            {
                mapped = LanguageMapper.TryMap(englishName);
                if (mapped != null)
                {
                    TranslationCache.Set(raw, mapped);
                    return mapped;
                }
            }

            try
            {
                var translated = await translateFunc(raw);

                if (!string.IsNullOrWhiteSpace(translated))
                {
                    TranslationCache.Set(raw, translated);
                    return translated;
                }
            }
            catch
            {
            }

            return raw;
        }
    }
}