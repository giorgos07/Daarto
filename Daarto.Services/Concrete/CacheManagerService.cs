using Daarto.Services.Abstract;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;

namespace Daarto.Services.Concrete
{
    public sealed class CacheManagerService : ICacheManagerService
    {
        /// <summary>
        /// The cache store. A dictionary that stores different memory caches by the type being cached.
        /// </summary>
        private readonly ConcurrentDictionary<Type, MemoryCache> _cacheStore;

        /// <summary>
        /// The default duration (in minutes) that an item stays in the cache.
        /// </summary>
        private const int DefaultCacheDurationInMinutes = 24 * 60;

        /// <summary>
        /// Initializes the <see cref="CacheManagerService"/> class.
        /// </summary>
        public CacheManagerService()
        {
            _cacheStore = new ConcurrentDictionary<Type, MemoryCache>();
        }

        /// <summary>
        /// Sets the specified cache using the absolute timeout specified in minutes.
        /// </summary>
        /// <typeparam name="T">The type of the item to be cached.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheItem">The item to be cached.</param>
        /// <param name="cacheDurationInMinutes">The absolute expiration (in minutes).</param>
        public void Set<T>(string cacheKey, T cacheItem, int cacheDurationInMinutes = DefaultCacheDurationInMinutes)
        {
            Type type = typeof(T);

            if (!_cacheStore.ContainsKey(type))
            {
                RegisterCache(type);
            }

            MemoryCache memoryCache = _cacheStore[type];
            memoryCache.Set(cacheKey, cacheItem, MemoryCacheEntryOptions(cacheDurationInMinutes));
        }

        /// <summary>
        /// Sets the specified cache using the passed function to generate the data. Uses the specified absolute timeout (in minutes).
        /// </summary>
        /// <typeparam name="T">The type of the item to be cached.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="getDataFromSourceCallback">The function to generate the item to be cached.</param>
        /// <param name="cacheDurationInMinutes">The absolute expiration (in minutes).</param>
        public void Set<T>(string cacheKey, Func<T> getDataFromSourceCallback, int cacheDurationInMinutes = DefaultCacheDurationInMinutes)
        {
            Type type = typeof(T);

            if (!_cacheStore.ContainsKey(type))
            {
                RegisterCache(type);
            }

            MemoryCache memoryCache = _cacheStore[type];
            T data = getDataFromSourceCallback();
            memoryCache.Set(cacheKey, data, MemoryCacheEntryOptions(cacheDurationInMinutes));
        }

        /// <summary>
        /// Tries to retrieve data from the cache first. If the data is not found, the provided function will be used to generate and store the 
        /// data in cache. Data is returned via the returnData parameter. Function returns true if successful. Uses the specified absolute timeout 
        /// (in minutes).
        /// </summary>
        /// <typeparam name="T">The type of the item to be cached.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="getDataFromSourceCallback">The function to generate the item to be cached.</param>
        /// <param name="cacheDurationInMinutes">The absolute expiration (in minutes).</param>
        /// <param name="returnData">The return data.</param>
        /// <returns>True if successful. False if data is null.</returns>
        public bool TryGetAndSet<T>(string cacheKey, Func<T> getDataFromSourceCallback, out T returnData, int cacheDurationInMinutes = DefaultCacheDurationInMinutes)
        {
            bool isRetrievedFromCache = TryGet(cacheKey, out returnData);

            if (isRetrievedFromCache)
            {
                return true;
            }

            returnData = getDataFromSourceCallback();
            Set(cacheKey, returnData, cacheDurationInMinutes);

            return returnData != null;
        }

        /// <summary>
        /// Attempts to retrieve an item from the cache.
        /// </summary>
        /// <typeparam name="T">The type of the item to be retrieved from the cache.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="returnItem">The item from cache.</param>
        /// <returns>True if successful. False if data is null or not found.</returns>
        public bool TryGet<T>(string cacheKey, out T returnItem)
        {
            Type type = typeof(T);

            if (_cacheStore.ContainsKey(type))
            {
                MemoryCache memoryCache = _cacheStore[type];
                object item = memoryCache.Get(cacheKey);

                if (item != null)
                {
                    returnItem = (T)item;
                    return true;
                }
            }

            returnItem = default(T);
            return false;
        }

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <typeparam name="T">The type of the item to be removed from the cache.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        public void Remove<T>(string cacheKey)
        {
            Type type = typeof(T);

            if (!_cacheStore.ContainsKey(type))
            {
                return;
            }

            MemoryCache memoryCache = _cacheStore[type];
            memoryCache.Remove(cacheKey);
        }

        /// <summary>
        /// Registers the cache in the dictionary.
        /// </summary>
        /// <param name="type">The type used as the key for the MemoryCache that stores this type of data.</param>
        private void RegisterCache(Type type)
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheStore.TryAdd(type, memoryCache);
        }

        /// <summary>
        /// Gets the memory cache entry options.
        /// </summary>
        /// <param name="cacheDurationInMinutes">The absolute expiration, in minutes.</param>
        /// <returns>A standard MemoryCacheEntryOptions object, varying only in expiration duration, for all items stored in MemoryCache.</returns>
        private static MemoryCacheEntryOptions MemoryCacheEntryOptions(int cacheDurationInMinutes = DefaultCacheDurationInMinutes)
        {
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.Normal,
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheDurationInMinutes)
            };

            return memoryCacheEntryOptions;
        }
    }
}