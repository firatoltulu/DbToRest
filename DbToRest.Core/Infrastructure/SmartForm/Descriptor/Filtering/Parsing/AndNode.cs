// (c) Copyright 2002-2010 Telerik 
// This source is subject to the GNU General Public License, version 2
// See http://www.gnu.org/licenses/gpl-2.0.html. 
// All other rights reserved.

using System.Collections.Generic;
namespace DbToRest.Core.Infrastructure.SmartForm.Implementation
{
    public class AndNode : IFilterNode, ILogicalNode
    {
        public AndNode()
        {
        }
        public IFilterNode First
        {
            get;
            set;
        }

        public IFilterNode Second
        {
            get;
            set;
        }

        public FilterCompositionLogicalOperator LogicalOperator
        {
            get
            {
                return FilterCompositionLogicalOperator.And;
            }
        }

        public void Accept(IFilterNodeVisitor visitor)
        {
            visitor.StartVisit(this);
            First.Accept(visitor);
            Second.Accept(visitor);
            visitor.EndVisit();
        }
        public bool IsNested { get; set; }
    }
}
