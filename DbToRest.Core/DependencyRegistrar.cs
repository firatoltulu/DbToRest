using Autofac;
using DbToRest.Core.Data.Provider;
using DbToRest.Core.Infrastructure;
using DbToRest.Core.Infrastructure.ComponentModel;
using NLog;

namespace DbToRest.Core
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, DbToRestConfig config)
        {
            builder.RegisterType<DataProvider>().As<IDataProvider>().InstancePerLifetimeScope();

            builder.Register((c, p) => new LoggerAdapter(LogManager.GetLogger("DbToRest")))
           .As<ILogService>().SingleInstance();

            builder.RegisterType<DbToRestFileProvider>().As<IDbToRestFileProvider>().InstancePerLifetimeScope();

        }
    }
}