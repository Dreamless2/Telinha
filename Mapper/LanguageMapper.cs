using System.Collections.Concurrent;

namespace Telinha.Mapper
{
    public static class LanguageMapper
    {
        private static readonly ConcurrentDictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["English"] = "Inglês",
            ["en"] = "Inglês",

            ["Portuguese"] = "Português",
            ["pt"] = "Português",

            ["Spanish"] = "Espanhol",
            ["es"] = "Espanhol",

            ["French"] = "Francês",
            ["fr"] = "Francês",

            ["German"] = "Alemão",
            ["de"] = "Alemão",

            ["Italian"] = "Italiano",
            ["it"] = "Italiano",

            ["Japanese"] = "Japonês",
            ["ja"] = "Japonês",

            ["Korean"] = "Coreano",
            ["ko"] = "Coreano",

            ["Chinese"] = "Chinês",
            ["zh"] = "Chinês",

            ["Russian"] = "Russo",
            ["ru"] = "Russo"
        };

        public static string? TryMap(string input)
        {
            return Map.TryGetValue(input, out var pt) ? pt : null;
        }
    }
}