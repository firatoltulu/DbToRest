namespace DbToRest.Core.Domain.Repository
{
    public interface IRavenRepository<T> : IRepository<T> where T : class, new()
    {

    }
}