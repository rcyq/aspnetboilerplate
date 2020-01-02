using System;
using System.Reflection;
using Abp.Domain.Entities;
using Abp.Reflection.Extensions;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// Base class for redis cache implementation
    /// </summary>
    public abstract class AbpRedisCacheBase : AbpCacheBase
    {
        protected readonly IDatabase Database;
        protected readonly IRedisCacheSerializer Serializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpRedisCacheBase(
            string name,
            IAbpRedisCacheDatabaseProvider redisCacheDatabaseProvider,
            IRedisCacheSerializer redisCacheSerializer)
            : base (name)
        {
            Database = redisCacheDatabaseProvider.GetDatabase();
            Serializer = redisCacheSerializer;
        }

        public override void Clear()
        {
            Database.KeyDeleteWithPrefix(GetLocalizedRedisKey("*"));
        }

        protected virtual Type GetSerializableType(object value)
        {
            //TODO: This is a workaround for serialization problems of entities.
            //TODO: Normally, entities should not be stored in the cache, but currently Abp.Zero packages does it. It will be fixed in the future.
            var type = value.GetType();
            if (EntityHelper.IsEntity(type) && type.GetAssembly().FullName.Contains("EntityFrameworkDynamicProxies"))
            {
                type = type.GetTypeInfo().BaseType;
            }
            return type;
        }

        protected virtual RedisValue Serialize(object value, Type type)
        {
            return Serializer.Serialize(value, type);
        }

        protected virtual object Deserialize(RedisValue value)
        {
            return Serializer.Deserialize(value);
        }

        protected virtual RedisKey GetLocalizedRedisKey(string key)
        {
            return "n:" + Name + ",c:" + key;
        }
    }
}
