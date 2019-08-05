namespace DbToRest.Core.Data.Repository
{
    public interface ICouchDbRepository<T> : IBaseRepository<T> where T : class, new()
    {

    }
}