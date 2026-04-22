using System;
using System.Collections.Generic;
using System.Text;
using Telinha.Cache;
using Telinha.Mapper;

namespace Telinha.Helpers
{
    public class LanguageHelper
    {
        public static async Task<string> ResolveAsync(
       string? englishName,
       string? isoCode,
       Func<string, Task<string?>> translateFunc)
        {
            var raw = englishName ?? isoCode;

            if (string.IsNullOrWhiteSpace(raw))
                return "--";

            // 🔹 1. Cache
            if (TranslationCache.TryGet(raw, out var cached))
                return cached;

            // 🔹 2. Mapper direto
            var mapped = LanguageMapper.TryMap(raw);
            if (mapped != null)
            {
                TranslationCache.Set(raw, mapped);
                return mapped;
            }

            // 🔹 3. Se tiver englishName, tenta mapear ele também
            if (!string.IsNullOrWhiteSpace(englishName))
            {
                mapped = LanguageMapper.TryMap(englishName);
                if (mapped != null)
                {
                    TranslationCache.Set(raw, mapped);
                    return mapped;
                }
            }

            // 🔹 4. DeepL fallback
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
                // ignora erro de API
            }

            // 🔹 5. fallback final
            return raw;
        }
    }
}