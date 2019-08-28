// (c) Copyright 2002-2010 Telerik 
// This source is subject to the GNU General Public License, version 2
// See http://www.gnu.org/licenses/gpl-2.0.html. 
// All other rights reserved.

namespace DbToRest.Core.Infrastructure.SmartForm
{
    using System.Collections.Generic;
    using DbToRest.Core.Infrastructure.SmartForm.Implementation;

    public static class FilterTokenExtensions
    {
        private static readonly IDictionary<string, FilterOperator> tokenToOperator = new Dictionary<string, FilterOperator>
        {
            { "eq", FilterOperator.IsEqualTo },
            { "ne", FilterOperator.IsNotEqualTo },
            { "lt", FilterOperator.IsLessThan },
            { "le", FilterOperator.IsLessThanOrEqualTo },
            { "gt", FilterOperator.IsGreaterThan },
            { "ge", FilterOperator.IsGreaterThanOrEqualTo },
            { "startswith", FilterOperator.StartsWith },
            { "substringof", FilterOperator.Contains },
            { "endswith", FilterOperator.EndsWith },
            { "in", FilterOperator.IsContainedIn },
            { "beetween", FilterOperator.Beetween }
        };

        private static readonly IDictionary<FilterOperator, string> operatorToToken = new Dictionary<FilterOperator, string>
        {
            { FilterOperator.IsEqualTo, "eq" },
            { FilterOperator.IsNotEqualTo, "ne" },
            { FilterOperator.IsLessThan, "lt" },
            { FilterOperator.IsLessThanOrEqualTo, "le" },
            { FilterOperator.IsGreaterThan, "gt" },
            { FilterOperator.IsGreaterThanOrEqualTo, "ge" },
            { FilterOperator.StartsWith, "startswith" },
            { FilterOperator.Contains, "substringof" },
            { FilterOperator.EndsWith, "endswith" },
            { FilterOperator.IsContainedIn, "in" },
            { FilterOperator.Beetween, "beetween " }
        };

        public static FilterOperator ToFilterOperator(this FilterToken token)
        {
            return tokenToOperator[token.Value];
        }

        public static string ToToken(this FilterOperator filterOperator)
        {
            return operatorToToken[filterOperator];
        }
    }
}
