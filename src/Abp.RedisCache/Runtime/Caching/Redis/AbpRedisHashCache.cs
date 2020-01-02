using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to store cache (in hash) in a Redis server.
    /// </summary>
    internal class AbpRedisHashCache : AbpRedisCacheBase, IAbpRedisHashCache
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpRedisHashCache(
            string name,
            IAbpRedisCacheDatabaseProvider redisCacheDatabaseProvider,
            IRedisCacheSerializer redisCacheSerializer)
            : base (name, redisCacheDatabaseProvider, redisCacheSerializer)
        {
        }

        public virtual object GetOrDefault(string key, string hashField)
        {
            var objByte = Database.HashGet(GetLocalizedRedisKey(key), hashField);
            return objByte.HasValue ? Deserialize(objByte) : null;
        }

        public virtual async Task<object> GetOrDefaultAsync(string key, string hashField)
        {
            var objByte = await Database.HashGetAsync(GetLocalizedRedisKey(key), hashField);
            return objByte.HasValue ? Deserialize(objByte) : null;
        }

        public virtual KeyValuePair<string, object>[] FindAll(string key, RedisValue query)
        {
            var hashEntries = Database.HashScan(GetLocalizedRedisKey(key), query) ?? Array.Empty<HashEntry>();
            return hashEntries.Select(h => new KeyValuePair<string, object>(h.Name, Deserialize(h.Value))).ToArray();
        }

        public virtual KeyValuePair<string, object>[] GetAll(string key)
        {
            var hashEntries = Database.HashGetAll(GetLocalizedRedisKey(key));
            return hashEntries.Select(DeserializeHash).ToArray();
        }

        public virtual async Task<KeyValuePair<string, object>[]> GetAllAsync(string key)
        {
            var hashEntries = await Database.HashGetAllAsync(GetLocalizedRedisKey(key));
            return hashEntries.Select(DeserializeHash).ToArray();
        }

        public virtual void Set(string key, string hashField, object hashValue)
        {
            if (hashField == null || hashValue == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            Database.HashSet(
                GetLocalizedRedisKey(key),
                hashField,
                Serialize(hashValue, GetSerializableType(hashValue)));
        }

        public virtual void Set(string key, KeyValuePair<string, object>[] hashPairs)
        {
            if (hashPairs.Any(h => h.Key == null || h.Value == null))
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            Database.HashSet(GetLocalizedRedisKey(key), hashPairs.Select(SerializeHash).ToArray());
        }

        public virtual async Task SetAsync(string key, string hashField, object hashValue)
        {
            if (hashField == null || hashValue == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            await Database.HashSetAsync(
                GetLocalizedRedisKey(key),
                hashField,
                Serialize(hashValue, GetSerializableType(hashValue)));
        }

        public virtual async Task SetAsync(string key, KeyValuePair<string, object>[] hashPairs)
        {
            if(hashPairs.Any(h => h.Key == null || h.Value == null))
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            await Database.HashSetAsync(GetLocalizedRedisKey(key), hashPairs.Select(SerializeHash).ToArray());
        }

        public virtual void Remove(string key)
        {
            Database.KeyDelete(GetLocalizedRedisKey(key));
        }

        public virtual void Remove(string key, string hashField)
        {
            Database.HashDelete(GetLocalizedRedisKey(key), hashField);
        }

        public virtual async Task RemoveAsync(string key)
        {
            await Database.KeyDeleteAsync(GetLocalizedRedisKey(key));
        }

        public virtual async Task RemoveAsync(string key, string hashField)
        {
            await Database.HashDeleteAsync(GetLocalizedRedisKey(key), hashField);
        }

        protected virtual HashEntry SerializeHash(KeyValuePair<string, object> pair)
        {
            return new HashEntry(pair.Key, Serialize(pair.Value, GetSerializableType(pair.Value)));
        }

        protected virtual KeyValuePair<string, object> DeserializeHash(HashEntry hash)
        {
            return new KeyValuePair<string, object>(hash.Name, Serializer.Deserialize(hash.Value));
        }
    }
}
