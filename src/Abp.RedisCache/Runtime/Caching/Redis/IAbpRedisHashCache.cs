using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to store cache (in hash) in a Redis server.
    /// </summary>
    public interface IAbpRedisHashCache : IAbpRedisHashCache<string, string, object>
    {
    }

    public interface IAbpRedisHashCache<TKey, TField, TValue> : IAbpCache
    {
        TValue GetOrDefault(TKey key, TField hashField);

        Task<TValue> GetOrDefaultAsync(TKey key, TField hashField);

        KeyValuePair<TField, TValue>[] FindAll(TKey key, RedisValue query);

        KeyValuePair<TField, TValue>[] GetAll(TKey key);

        Task<KeyValuePair<TField, TValue>[]> GetAllAsync(TKey key);

        void Set(TKey key, TField hashField, TValue hashValue);

        void Set(TKey key, KeyValuePair<TField, TValue>[] hashPairs);

        Task SetAsync(TKey key, TField hashField, TValue hashValue);

        Task SetAsync(TKey key, KeyValuePair<TField, TValue>[] hashPairs);

        void Remove(TKey key);

        void Remove(TKey key, TField hashField);

        Task RemoveAsync(TKey key);

        Task RemoveAsync(TKey key, TField hashField);
    }
}
