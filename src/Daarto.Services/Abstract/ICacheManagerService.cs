using System;

namespace Daarto.Services.Abstract
{
    public interface ICacheManagerService
    {
        void Set<T>(string cacheKey, T cacheItem, int cacheDurationInMinutes);
        void Set<T>(string cacheKey, Func<T> getDataFromSourceCallback, int cacheDurationInMinutes);
        bool TryGetAndSet<T>(string cacheKey, Func<T> getDataFromSourceCallback, out T returnData, int cacheDurationInMinutes = 24 * 60);
        bool TryGet<T>(string cacheKey, out T returnItem);
        void Remove<T>(string cacheKey);
    }
}