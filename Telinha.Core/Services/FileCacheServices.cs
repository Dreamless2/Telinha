using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using Telinha.Core.Models;

namespace Telinha.Core.Services
{
    public class FileCacheServices
    {
        private readonly IMemoryCache _memory;
        private readonly ConcurrentDictionary<string, CacheEntry> _disk = new();
        private readonly string _path;
        private readonly SemaphoreSlim _fileLock = new(1, 1);
        private readonly TimeSpan _memoryTtl = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _diskFlushDelay = TimeSpan.FromSeconds(2);
        private int _dirtyFlag = 0;
        private const int CACHE_VERSION = 1;

        public FileCacheServices(IMemoryCache memory)
        {
            _memory = memory;
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmdb_cache_v2.json");

            LoadFromDisk();
        }

        // =========================
        // GET (lock-free hot path)
        // =========================
        public Task<MidiaModel?> GetAsync(string key)
        {
            if (_memory.TryGetValue(key, out MidiaModel? mem))
                return Task.FromResult(mem);

            if (_disk.TryGetValue(key, out var entry))
            {
                if (entry.ExpiresAt <= DateTime.UtcNow)
                {
                    _disk.TryRemove(key, out _);
                    using var _ = MarkDirty();
                    return Task.FromResult<MidiaModel?>(null);
                }

                _memory.Set(key, entry.Model, _memoryTtl);
                return Task.FromResult<MidiaModel?>(entry.Model);
            }

            return Task.FromResult<MidiaModel?>(null);
        }

        // =========================
        // SET
        // =========================
        public async Task SetAsync(string key, MidiaModel model, TimeSpan diskTtl)
        {
            _memory.Set(key, model, _memoryTtl);

            var entry = new CacheEntry
            {
                Model = model,
                ExpiresAt = DateTime.UtcNow.Add(diskTtl),
                Version = CACHE_VERSION
            };

            _disk[key] = entry;

            await MarkDirty();
        }

        // =========================
        // REMOVE
        // =========================
        public async Task RemoveAsync(string key)
        {
            _memory.Remove(key);
            _disk.TryRemove(key, out _);

            await MarkDirty();
        }

        // =========================
        // DISK LOAD
        // =========================
        private void LoadFromDisk()
        {
            if (!File.Exists(_path))
                return;

            try
            {
                var json = File.ReadAllText(_path);

                var data = JsonSerializer.Deserialize<Dictionary<string, CacheEntry>>(json);

                if (data == null)
                    return;

                foreach (var kv in data)
                {
                    if (kv.Value.Version != CACHE_VERSION)
                        continue;

                    if (kv.Value.ExpiresAt > DateTime.UtcNow)
                        _disk[kv.Key] = kv.Value;
                }
            }
            catch
            {
                _disk.Clear();
            }
        }

        // =========================
        // WRITE BATCH (CORE FIX)
        // =========================
        private async Task MarkDirty()
        {
            if (Interlocked.Exchange(ref _dirtyFlag, 1) == 1)
                return;

            _ = Task.Run(async () =>
            {
                await Task.Delay(_diskFlushDelay);
                await FlushToDisk();

                Interlocked.Exchange(ref _dirtyFlag, 0);
            });
        }

        private async Task FlushToDisk()
        {
            await _fileLock.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(_disk);
                await File.WriteAllTextAsync(_path, json);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        // =========================
        // CLEANUP
        // =========================
        public void CleanupExpired()
        {
            var now = DateTime.UtcNow;

            foreach (var item in _disk)
            {
                if (item.Value.ExpiresAt <= now)
                    _disk.TryRemove(item.Key, out _);
            }

            MarkDirty();
        }

        // =========================
        // ENTRY MODEL
        // =========================
        private class CacheEntry
        {
            public MidiaModel Model { get; set; } = default!;
            public DateTime ExpiresAt { get; set; }
            public int Version { get; set; }
        }
    }
}