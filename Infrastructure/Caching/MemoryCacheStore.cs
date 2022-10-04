using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

public class MemoryCacheStore : ICacheStore
{
    private readonly IMemoryCache _memoryCache;
    private readonly Dictionary<string, TimeSpan> _expirationConfiguration;

    public MemoryCacheStore(IMemoryCache memoryCache, Dictionary<string, TimeSpan> expirationConfiguration)
    {
        _memoryCache = memoryCache;
        _expirationConfiguration = expirationConfiguration;
    }

    public void Add<TItem>(TItem item, ICacheKey<TItem> key, TimeSpan? expirationTime = null)
    {
        var cacheObjectName = item!.GetType().Name;

        var timeSpan = expirationTime!.HasValue
            ? expirationTime.Value
            : _expirationConfiguration[cacheObjectName];

        _memoryCache.Set(key.CacheKey, item, timeSpan);
    }

    public void Add<TItem>(TItem item, ICacheKey<TItem> key, DateTime? absoluteExpiration = null)
    {
        var offset = absoluteExpiration!.HasValue
            ? absoluteExpiration.Value
            : DateTimeOffset.MaxValue;

        _memoryCache.Set(key.CacheKey, item, offset);
    }

    public TItem Get<TItem>(ICacheKey<TItem> key) where TItem : class
    {
        return _memoryCache.TryGetValue(key.CacheKey, out TItem value) ? value : null!;
    }

    public void Remove<TItem>(ICacheKey<TItem> key)
    {
        _memoryCache.Remove(key.CacheKey);
    }
}