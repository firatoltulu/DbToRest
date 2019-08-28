﻿using System;
using System.Collections.Generic;

namespace DbToRest.Core.Infrastructure.SmartForm.Descriptor.Builder
{
    public class Filter<TClass> : IFilter<TClass> where TClass : class
    {
        private readonly List<IFilterStatement> _statements;

        public IEnumerable<IFilterStatement> Statements
        {
            get
            {
                return _statements.ToArray();
            }
        }

        public Filter()
        {
            _statements = new List<IFilterStatement>();
        }

        public IFilterStatementConnection<TClass> By<TPropertyType>(string propertyName, FilterOperator operation, TPropertyType value, FilterCompositionLogicalOperator connector = FilterCompositionLogicalOperator.And)
        {
            IFilterStatement statement = null;
            statement = new FilterStatement<TPropertyType>(propertyName, operation, value, connector);
            _statements.Add(statement);
            return new FilterStatementConnection<TClass>(this, statement);
        }

        public void Clear()
        {
            _statements.Clear();
        }

        public System.Linq.Expressions.Expression<Func<TClass, bool>> BuildExpression()
        {
            return null; //Builder.GetExpression<TClass>(this);
        }

        public override string ToString()
        {
            var result = "";
            FilterCompositionLogicalOperator lastConector = FilterCompositionLogicalOperator.And;
            foreach (var statement in _statements)
            {
                if (!string.IsNullOrWhiteSpace(result)) result += " " + lastConector + " ";
                result += statement.ToString();
                lastConector = statement.Connector;
            }

            return result.Trim();
        }
    }
}
