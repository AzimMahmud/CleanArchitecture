namespace Infrastructure.Caching;

public interface ICacheStoreItem
{
    string CacheKey { get; }
}