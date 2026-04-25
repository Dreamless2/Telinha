using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using Telinha.Models;

namespace Telinha.Services
{
    public class FileCacheServices
    {
        private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmdb_cache.json");
        private readonly IMemoryCache _memory;
        private readonly ConcurrentDictionary<string, CacheItem> _diskCache = new();
        private readonly SemaphoreSlim _fileLock = new(1, 1);
        private readonly TimeSpan _memoryTtl = TimeSpan.FromMinutes(30);
        private long _hits = 0;
        private long _misses = 0;

        public FileCacheServices(IMemoryCache memory)
        {
            _memory = memory;
            CarregarDoDisco();
        }

        public async Task<MidiaModel?> GetAsync(string key)
        {
            // 1. Tenta memória primeiro - 1ms
            if (_memory.TryGetValue(key, out MidiaModel? mem))
            {
                LogServices.LogarInformacao("CACHE HIT MEM - {key}", key);
                return mem;
            }

            // 2. Tenta arquivo - 5ms
            if (_diskCache.TryGetValue(key, out var item))
            {
                if (item.ExpiraEm > DateTime.UtcNow)
                {
                    var model = JsonConvert.DeserializeObject<MidiaModel>(item.Json);
                    _memory.Set(key, model, _memoryTtl); // esquenta memória
                    LogServices.LogarInformacao("CACHE HIT FILE - {key}", key);
                    return model;
                }
                else
                {
                    // Expirou, remove
                    _diskCache.TryRemove(key, out _);
                    _ = SalvarNoDisco(); // fire and forget
                }
            }

            LogServices.LogarInformacao("CACHE MISS - {key}", key);
            return null;
        }

        public async Task SetAsync(string key, MidiaModel model, TimeSpan diskTtl)
        {
            // Salva nos 2 lugares
            _memory.Set(key, model, _memoryTtl);

            _diskCache[key] = new CacheItem
            {
                Json = JsonConvert.SerializeObject(model),
                ExpiraEm = DateTime.UtcNow.Add(diskTtl)
            };

            LogServices.LogarInformacao("CACHE SET - {key} | Expira: {expira}", key, diskTtl);
            await SalvarNoDisco();
        }

        public async Task RemoveAsync(string key)
        {
            _memory.Remove(key);
            _diskCache.TryRemove(key, out _);
            await SalvarNoDisco();
        }

        private void CarregarDoDisco()
        {
            try
            {
                if (!File.Exists(_path)) return;

                var json = File.ReadAllText(_path);
                var dados = JsonConvert.DeserializeObject<Dictionary<string, CacheItem>>(json);

                if (dados != null)
                {
                    foreach (var kvp in dados)
                    {
                        // Já carrega só o que não expirou
                        if (kvp.Value.ExpiraEm > DateTime.UtcNow)
                            _diskCache[kvp.Key] = kvp.Value;
                    }
                }

                LogServices.LogarInformacao("CACHE CARREGADO - {count} itens válidos", _diskCache.Count);
            }
            catch (Exception ex)
            {
                LogServices.LogarErroComException(ex, "Erro ao carregar cache do disco");
                _diskCache.Clear();
            }
        }

        private async Task SalvarNoDisco()
        {
            await _fileLock.WaitAsync();
            try
            {
                var json = JsonConvert.SerializeObject(_diskCache, Formatting.Indented);
                await File.WriteAllTextAsync(_path, json);
            }
            catch (Exception ex)
            {
                LogServices.LogarErroComException(ex, "Erro ao salvar cache no disco");
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public void LimparExpirados()
        {
            var expirados = _diskCache.Where(x => x.Value.ExpiraEm <= DateTime.UtcNow).Select(x => x.Key).ToList();
            foreach (var key in expirados)
                _diskCache.TryRemove(key, out _);

            if (expirados.Any())
            {
                LogServices.LogarInformacao("CACHE CLEAN - {count} itens expirados removidos", expirados.Count);
                _ = SalvarNoDisco();
            }
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (_cache.TryGetValue(key, out value))
            {
                Interlocked.Increment(ref _hits);
                return true;
            }
            Interlocked.Increment(ref _misses);
            return false;
        }

        public double HitRate => _hits + _misses == 0 ? 0 : (double)_hits / (_hits + _misses) * 100;

        private class CacheItem
        {
            public string Json { get; set; } = string.Empty;
            public DateTime ExpiraEm { get; set; }
        }
    }
}