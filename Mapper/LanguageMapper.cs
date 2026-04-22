using System;
using System.Collections.Generic;
using System.Text;

namespace Telinha.Mapper
{
    public static class LanguageMapper
    {
        private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["English"] = "Inglês",
            ["Portuguese"] = "Português",
            ["Spanish"] = "Espanhol",
            ["French"] = "Francês",
            ["German"] = "Alemão",
            ["Italian"] = "Italiano",
            ["Japanese"] = "Japonês",
            ["Korean"] = "Coreano",
            ["Chinese"] = "Chinês",
            ["Russian"] = "Russo"
        };

        public static string ToPtBr(string englishName)
        {
            return Map.TryGetValue(englishName, out var pt) ? pt : englishName;
        }
    }
}