using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DbToRest.Core.Data.Repository
{
    public interface IBaseRepository<T> where T : class, new()
    {
        IQueryable<T> Table();

        IQueryable<T> Table(Expression<Func<T, bool>> where);

        T Single(object key);

        T Single(Expression<Func<T, bool>> where);

        void Delete(object key);

        void Delete(T value);

        void Delete(Expression<Func<T, bool>> where);

        void Delete(IEnumerable<object> key);

        void Insert(T value);

        void Insert(List<T> value);

        void Update(List<T> value);

        void Update(T value);

        void Update(System.Linq.Expressions.Expression<Func<T, bool>> where, params System.Linq.Expressions.Expression<Func<T, bool>>[] columns);

        void Add(List<T> value);

        void Add(T value);

        void Save(T value);

    }
}