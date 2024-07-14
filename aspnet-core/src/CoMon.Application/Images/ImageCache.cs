using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;

namespace CoMon.Images
{
    public class ImageCache : EntityCache<Image, ImageCacheItem, long>, ITransientDependency
    {
        public ImageCache(ICacheManager cacheManager, IRepository<Image, long> repository, IUnitOfWorkManager unitOfWorkManager)
        : base(cacheManager, repository, unitOfWorkManager)
        { }
    }
}
