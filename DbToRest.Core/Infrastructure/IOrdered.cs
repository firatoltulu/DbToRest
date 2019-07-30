using System;

namespace DbToRest.Core.Infrastructure
{
    public interface IOrdered
    {
        // TODO: (MC) Make Nullable!
        int Ordinal { get; set; }
    }
}
