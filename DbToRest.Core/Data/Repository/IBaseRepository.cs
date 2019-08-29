using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DbToRest.Core.Data.Repository
{
    public interface IBaseRepository<T> where T : class, new()
    {
        IEnumerable<T> Table();

        IEnumerable<T> Table(Expression<Func<T, bool>> where);

        T Single(object key);

        T Single(Expression<Func<T, bool>> where);

        void Delete(object key);

        void Delete(T value);

        void Delete(Expression<Func<T, bool>> where);

        void Delete(IEnumerable<object> key);

        void Save(List<T> value);

        void Save(T value);
    }
}