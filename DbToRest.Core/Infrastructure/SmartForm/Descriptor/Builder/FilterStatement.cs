using System;
using System.Collections.Generic;


namespace DbToRest.Core.Infrastructure.SmartForm.Descriptor.Builder
{
    public class FilterStatement<TPropertyType> : IFilterStatement
    {
        public FilterCompositionLogicalOperator Connector { get; set; }
        public string PropertyName { get; set; }
        public FilterOperator Operation { get; set; }
        public object Value { get; set; }

        public FilterStatement(string propertyName, FilterOperator operation, TPropertyType value, FilterCompositionLogicalOperator connector = FilterCompositionLogicalOperator.And)
        {
            PropertyName = propertyName;
            Connector = connector;
            Operation = operation;
            if (typeof(TPropertyType).IsArray)
            {
                if (operation != FilterOperator.Contains) throw new ArgumentException("Only 'Operacao.Contains' supports arrays as parameters.");
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(typeof(TPropertyType).GetElementType());
                Value = Activator.CreateInstance(constructedListType, value);
            }
            else
            {
                Value = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", PropertyName, Operation, Value);
        }
    }
}