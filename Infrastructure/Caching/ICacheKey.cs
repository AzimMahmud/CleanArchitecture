﻿namespace Infrastructure.Caching;

public interface ICacheKey<TItem>
{
    string CacheKey { get; }
}