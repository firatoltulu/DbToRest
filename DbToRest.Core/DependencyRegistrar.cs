using Autofac;
using DbToRest.Core.Data.Provider;
using DbToRest.Core.Data.Repository;
using DbToRest.Core.Infrastructure;
using DbToRest.Core.Infrastructure.Caching;
using DbToRest.Core.Infrastructure.ComponentModel;
using DbToRest.Core.Services.Authentication;
using NLog;

namespace DbToRest.Core
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, DbToRestConfig config)
        {

            builder.RegisterType<CookieAuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            builder.RegisterType<DataProvider>().As<IDataProvider>().InstancePerLifetimeScope();
            builder.RegisterType<RedisCache>().As<ICache>().SingleInstance();


            builder.RegisterType<DataProvider>().As<IDataProvider>().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(DbRepository<>)).As(typeof(IDbRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(CacheRepository<>)).As(typeof(ICacheRepository<>)).InstancePerLifetimeScope();

            builder.Register((c, p) => new LoggerAdapter(LogManager.GetLogger("DbToRest")))
           .As<ILogService>().SingleInstance();

            builder.RegisterType<DbToRestFileProvider>().As<IDbToRestFileProvider>().InstancePerLifetimeScope();

        }
    }
}