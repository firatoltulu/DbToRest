using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DbToRest.Core.Infrastructure.SmartForm
{
    public class SmartFilterRavenFormatter
    {
        public SmartFilterRavenFormatter()
        {
            _builder = new StringBuilder();
        }

        private StringBuilder _builder = null;

        public static string Parse(SmartFilterCommand command)
        {
            var formatter = new SmartFilterRavenFormatter();
            if (command.FilterDescriptors.Count > 0)
            {
                formatter.filter(command.FilterDescriptors);
            }
            return formatter.ToString();
        }

        public override string ToString()
        {
            return this._builder.ToString();
        }

        #region Write

        protected void WriteMember(object value)
        {
            this._builder.AppendFormat("{0}=", value);
        }

        protected void Write(object value)
        {
            this._builder.AppendFormat("{0}", value);
        }

        protected void WriteWithSpace(object value)
        {
            this._builder.AppendFormat("{0}", value);
        }

        #endregion Write

        #region Formatter

        private void filter(IList<IFilterDescriptor> filters)
        {
            if (filters.Count > 0)
            {
                foreach (var item in filters)
                {
                    Visit(item);
                }
            }
        }

        #endregion Formatter

        #region prepaire

        private void Visit(IFilterDescriptor ex)
        {
            if (ex is CompositeFilterDescriptor)
            {
                var compositeFilter = ex as CompositeFilterDescriptor;

                if (compositeFilter.IsNested)
                    Write(" (");

                var left = compositeFilter.FilterDescriptors.FirstOrDefault();
                var right = compositeFilter.FilterDescriptors.LastOrDefault();
                Visit(left);
                Write(string.Format(" {0} ", compositeFilter.LogicalOperator.ToString().ToUpper()));
                Visit(right);

                if (compositeFilter.IsNested)
                    Write(") ");
            }
            else if (ex is FilterDescriptor)
            {
                var filter = ex as FilterDescriptor;
                VisitBinary(filter);
            }
        }

        private void VisitBinary(IFilterDescriptor b)
        {
            var filter = b as FilterDescriptor;

            var op = GetOperator(filter.Operator);
            switch (filter.Operator)
            {
                case FilterOperator.IsLessThan:
                case FilterOperator.IsLessThanOrEqualTo:
                case FilterOperator.IsEqualTo:
                case FilterOperator.IsNotEqualTo:
                case FilterOperator.IsGreaterThanOrEqualTo:
                case FilterOperator.IsGreaterThan:
                    if (Type.GetTypeCode(filter.Value.GetType()) == TypeCode.String)
                    {
                        WriteMember(filter.Member);
                        WriteValue(filter.Value);
                    }
                    else
                    {
                        WriteWithSpace(filter.Member);
                        Write(op);
                        WriteValue(filter.Value);
                    }
                    break;

                case FilterOperator.StartsWith:
                case FilterOperator.EndsWith:
                case FilterOperator.Contains:
                    //WriteMember(filter.Member);
                    WriteWithSpace(string.Format("boost(search({0},{1}),10)", filter.Member, string.Format(op, filter.Value.ToString())));
                    break;

                case FilterOperator.IsContainedIn:
                    WriteWithSpace(filter.Member);
                    WriteWithSpace(string.Format(op, filter.Value.ToString().Replace(",", "OR")));
                    break;

                case FilterOperator.Beetween:
                    WriteWithSpace(filter.Member);
                    WriteWithSpace(string.Format(op, filter.Value.ToString().Replace(",", "OR")));
                    break;

                default:
                    break;
            }
        }

        private string GetOperator(FilterOperator b)
        {
            switch (b)
            {
                case FilterOperator.IsLessThan:
                    return "<";

                case FilterOperator.IsLessThanOrEqualTo:
                    return "<=";

                case FilterOperator.IsEqualTo:
                    return "";

                case FilterOperator.IsNotEqualTo:
                    return "!=";

                case FilterOperator.IsGreaterThanOrEqualTo:
                    return ">=";

                case FilterOperator.IsGreaterThan:
                    return ">";

                case FilterOperator.StartsWith:
                    return "'{0}*'";

                case FilterOperator.EndsWith:
                    return "'{0}*'";

                case FilterOperator.Contains:
                    return "'*{0}*'";

                case FilterOperator.IsContainedIn:
                    return "({0})";
            }
            return "";
        }

        protected virtual void WriteValue(object value)
        {
            if (value == null)
            {
                this.Write("NULL");
            }
            else if (value.GetType().IsEnum)
            {
                this.Write(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));
            }
            else
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Boolean:
                        this.Write(((bool)value).ToString().ToLower());
                        break;

                    case TypeCode.String:
                        this.Write("'");
                        this.Write(value);
                        this.Write("'");
                        break;

                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
                    case TypeCode.Single:
                    case TypeCode.Double:
                        string str = value.ToString();
                        /*if (!str.Contains('.'))
                        {
                            str += ".0";
                        }*/
                        this.Write(str);
                        break;

                    case TypeCode.DateTime:
                        this.Write(Convert.ToDateTime(value).ToString(new CultureInfo("en-US")));
                        break;

                    default:
                        this.Write(value);
                        break;
                }
            }
        }

        #endregion prepaire
    }
}