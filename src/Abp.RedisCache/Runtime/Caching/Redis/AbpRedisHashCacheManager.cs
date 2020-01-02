using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to create <see cref="AbpRedisHashCache"/> instances.
    /// </summary>
    public class AbpRedisHashCacheManager : CacheManagerBase<IAbpRedisHashCache>
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpRedisHashCacheManager"/> class.
        /// </summary>
        public AbpRedisHashCacheManager(IIocManager iocManager, ICachingConfiguration configuration) 
            : base(configuration)
        {
            _iocManager = iocManager;
            _iocManager.RegisterIfNot<AbpRedisHashCache>(DependencyLifeStyle.Transient);
        }

        protected override IAbpRedisHashCache CreateCacheImplementation(string name)
        {
            return _iocManager.Resolve<AbpRedisHashCache>(new { name });
        }

        protected override void DisposeCaches()
        {
            foreach (var cache in Caches)
            {
                _iocManager.Release(cache.Value);
            }
        }
    }
}
