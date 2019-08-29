using DbToRest.Core.Infrastructure.SmartForm;
using System.Collections.Generic;

namespace DbToRest.Core.Data.Repository
{
    public interface IDbRepository<T> : IBaseRepository<T> where T : class, new()
    {
        IEnumerable<T> Table(SmartFilterCommand cmd);
        IEnumerable<T> Table(string indexName, string whereClause);
    }
}