using Autofac;
using DbToRest.Core.Infrastructure;
using DbToRest.Core.Infrastructure.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbToRest.Core
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, DbToRestConfig config)
        {
            


        }
    }
}
