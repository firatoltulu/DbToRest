namespace DbToRest.Core.Data.Repository
{
    public interface ICacheRepository<T> : IBaseRepository<T> where T : class, new()
    {
        void Clear();
    }
}