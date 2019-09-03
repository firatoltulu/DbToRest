using DbToRest.Core.Data.Provider;
using DbToRest.Core.Services.Authentication;

namespace DbToRest.Core.Data.Repository
{
    public class CacheRepository<T> : DbRepository<T>, ICacheRepository<T> where T : class, new()
    {
        //    private readonly ICache _cache = null;

        public CacheRepository(
                IDataProvider provider
    ) : base(provider)
        {
        }

        //    public override IEnumerable<T> Table()
        //    {
        //        return _cache.Get<IList<T>>(createCacheKey(CoreSystemKeywords.CacheRepositoryKeywords.Table), () =>
        //        {
        //            var all = base.Table().ToList();
        //            return all;
        //        }).AsQueryable();
        //    }

        //    public override void Save(T value)
        //    {
        //        base.Save(value);
        //        updateCache(value);
        //    }

        //    public override void Delete(T value)
        //    {
        //        base.Delete(value);
        //        updateCache(value);
        //    }

        //    public override void Delete(Expression<Func<T, bool>> where)
        //    {
        //        base.Delete(where);
        //        Clear();
        //    }

        //    public override void Delete(object key)
        //    {
        //        base.Delete(key);
        //        var all = Table().ToList();
        //        var first = all.Where("Id=@0", key).FirstOrDefault();
        //        if (first != null)
        //        {
        //            if (all.Remove(first))
        //            {
        //                _cache.Set<IList<T>>(createCacheKey(CoreSystemKeywords.CacheRepositoryKeywords.Table), all);
        //            }
        //        }
        //    }

        //    public override void Update(Expression<Func<T, bool>> where, params Expression<Func<T, bool>>[] columns)
        //    {
        //        base.Update(where, columns);
        //    }

        //    #region [ Private Utilities ]

        //    private void updateCache(T value)
        //    {
        //        var all = Table().ToList();
        //        var first = all.Where("Id=@0", value.GetPropertyValue("Id")).FirstOrDefault();
        //        if (first != null)
        //            all.Remove(first);

        //        all.Add(value);

        //        _cache.Set<IList<T>>(createCacheKey(CoreSystemKeywords.CacheRepositoryKeywords.Table), all);
        //    }

        //    private string makeKey()
        //    {
        //        return string.Format("{0}{1}", CoreSystemKeywords.CacheRepositoryKeywords.Region, typeof(T).Name.StripString());
        //    }

        //    private string createCacheKey(string value)
        //    {
        //        return string.Format("{0}{1}{2}", CoreSystemKeywords.CacheRepositoryKeywords.Region, value, typeof(T).Name);
        //    }

        public void Clear()
        {
            // _cache.Delete(createCacheKey(CoreSystemKeywords.CacheRepositoryKeywords.Table));
        }

        //    #endregion [ Private Utilities ]
    }
}