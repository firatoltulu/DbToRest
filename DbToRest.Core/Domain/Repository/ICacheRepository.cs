namespace DbToRest.Core.Domain.Repository
{
    public interface ICacheRepository<T> : IRepository<T> where T : class, new()
    {
        void Clear();
    }
}