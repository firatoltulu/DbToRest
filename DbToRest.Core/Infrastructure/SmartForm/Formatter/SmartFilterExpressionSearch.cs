using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace DbToRest.Core.Infrastructure.SmartForm
{
    public class SmartFilterExpressionSearch
    {
        public SmartFilterExpressionSearch()
        {
        }

        public static dynamic Search<TClass>(IQueryable<TClass> query, SmartFilterCommand command) where TClass : class
        {
            int count = 0;
            object result = null;

            var expression = SmartForm.Descriptor.Builder.Builder.GetExpression<TClass>(command.FilterDescriptors);
            query = query.Where(expression);
            query = OrderBy<TClass>(query, command.SortDescriptors);

            count = query.Count();

            if (command.Skip > 0)
                query = query.Skip(command.Skip);

            if (command.Top > 0)
                query = query.Take(command.Top);

            result = query.ToList();

            return new
            {
                Count = count,
                Result = result
            };
        }

        public static dynamic Search<TClass>(IEnumerable<TClass> query, SmartFilterCommand command) where TClass : class
        {
            return Search<TClass>(query.AsQueryable(), command);
        }

        private static IQueryable<T> OrderBy<T>(IQueryable<T> datasource, IList<SortDescriptor> sortDescriptor)
        {
            foreach (var item in sortDescriptor)
            {
                datasource = OrderBy<T>(datasource, item.Member, item.SortDirection);
            }

            return datasource;
        }

        private static IQueryable<T> OrderBy<T>(IQueryable<T> datasource, string propertyName, ListSortDirection direction)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return datasource;
            }

            var type = typeof(T);
            var property = type.GetProperty(propertyName);

            if (property == null)
            {
                throw new InvalidOperationException(string.Format("Could not find a property called '{0}' on type {1}", propertyName, type));
            }

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            const string orderBy = "OrderBy";
            const string orderByDesc = "OrderByDescending";

            string methodToInvoke = direction == ListSortDirection.Ascending ? orderBy : orderByDesc;

            var orderByCall = Expression.Call(typeof(Queryable),
                methodToInvoke,
                new[] { type, property.PropertyType },
                datasource.Expression,
                Expression.Quote(orderByExp));

            return datasource.Provider.CreateQuery<T>(orderByCall);
        }
    }
}