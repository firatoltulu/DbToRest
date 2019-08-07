using Autofac;
using DbToRest.Core.Data.Provider;
using DbToRest.Core.Infrastructure;
using DbToRest.Core.Infrastructure.ComponentModel;

namespace DbToRest.Core
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, DbToRestConfig config)
        {
            builder.RegisterType<DataProvider>().As<IDataProvider>().InstancePerRequest();
            
        }
    }
}