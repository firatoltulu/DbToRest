﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DbToRest.Core.Infrastructure.SmartForm.Descriptor.Builder
{
    /// <summary>
    /// Defines a filter from which a expression will be built.
    /// </summary>
    public interface IFilter<TClass> where TClass : class
    {
        /// <summary>
        /// Group of statements that compose this filter.
        /// </summary>
        IEnumerable<IFilterStatement> Statements { get; }
        /// <summary>
        /// Adds another statement to this filter.
        /// </summary>
        /// <param name="propertyName">Name of the property that will be filtered.</param>
        /// <param name="operation">Express the interaction between the property and the constant value.</param>
        /// <param name="value">Constant value that will interact with the property.</param>
        /// <param name="connector">Establishes how this filter statement will connect to the next one.</param>
        /// <returns>A FilterStatementConnection object that defines how this statement will be connected to the next one.</returns>
        IFilterStatementConnection<TClass> By<TPropertyType>(string propertyName, FilterOperator operation, TPropertyType value, FilterCompositionLogicalOperator connector = FilterCompositionLogicalOperator.And);
        /// <summary>
        /// Removes all statements from this filter.
        /// </summary>
        void Clear();
        /// <summary>
        /// Builds a LINQ expression based upon the statements included in this filter.
        /// </summary>
        /// <returns></returns>
        Expression<Func<TClass, bool>> BuildExpression();
    }
}