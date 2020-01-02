using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to create <see cref="AbpRedisStringCache"/> instances.
    /// </summary>
    public class AbpRedisStringCacheManager : CacheManagerBase<ICache>, ICacheManager
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpRedisStringCacheManager"/> class.
        /// </summary>
        public AbpRedisStringCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(configuration)
        {
            _iocManager = iocManager;
            _iocManager.RegisterIfNot<AbpRedisStringCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return _iocManager.Resolve<AbpRedisStringCache>(new { name });
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
