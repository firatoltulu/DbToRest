using System;

namespace DbToRest.Core.Infrastructure.SmartForm.Descriptor.Builder
{
    public class FilterStatementConnection<TClass> : IFilterStatementConnection<TClass> where TClass : class
    {
        readonly IFilter<TClass> _filter;
        readonly IFilterStatement _statement;

        public FilterStatementConnection(IFilter<TClass> filter, IFilterStatement statement)
        {
            _filter = filter;
            _statement = statement;
        }

        public IFilter<TClass> And
        {
            get
            {
                _statement.Connector = FilterCompositionLogicalOperator.And;
                return _filter;
            }
        }

        public IFilter<TClass> Or
        {
            get
            {
                _statement.Connector = FilterCompositionLogicalOperator.Or;
                return _filter;
            }
        }
    }
}
