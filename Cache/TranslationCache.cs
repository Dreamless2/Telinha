namespace Telinha.Cache
{
    public static class TranslationCache
    {
        private static readonly Dictionary<string, string> Cache = new(StringComparer.OrdinalIgnoreCase);
        public static bool TryGet(string key, out string value)
            => Cache.TryGetValue(key, out value!);

        public static void Set(string key, string value)
            => Cache[key] = value;
    }
}