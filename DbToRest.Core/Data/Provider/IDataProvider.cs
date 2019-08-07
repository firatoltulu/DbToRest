using MyCouch;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbToRest.Core.Data.Provider
{
    public interface IDataProvider
    {
        IMyCouchClient Provider { get; }
    }
}
