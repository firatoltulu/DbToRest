using DbToRest.Core.Data;
using DbToRest.Core.Infrastructure.ComponentModel;
using DbToRest.Core.Infrastructure.Http;
using DbToRest.Core.Services.Authentication;
using EasyCaching.Core;
using EasyCaching.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;

namespace DbToRest.Core.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider ConfigureApplicationServices(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var nopConfig = services.ConfigureStartupConfig<DbToRestConfig>(configuration.GetSection("DbToRest"));

            services.ConfigureStartupConfig<HostingConfig>(configuration.GetSection("Hosting"));
            services.AddHttpContextAccessor();
            services.AddDbToRestAntiForgery();
            services.AddDbToRestHttpSession();
            services.AddDbToRestMvc();
            services.AddCors();
            services.AddEasyCaching();

            CommonHelper.DefaultFileProvider = new DbToRestFileProvider(hostingEnvironment);

            var mvcCoreBuilder = services.AddMvcCore();

            var engine = EngineContext.Create();
            var serviceProvider = engine.ConfigureServices(services, configuration, nopConfig);

            if (!CommonHelper.DatabaseIsInstalled)
                return serviceProvider;

            engine.Resolve<ILogService>().Info("Application started");

            return serviceProvider;
        }

        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            //create instance of config
            var config = new TConfig();

            //bind it to the appropriate section of configuration
            configuration.Bind(config);

            //and register it as a service
            services.AddSingleton(config);

            return config;
        }

        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void AddDbToRestAntiForgery(this IServiceCollection services)
        {
            //override cookie name
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = $"{DbToRestCookieDefaults.Prefix}{DbToRestCookieDefaults.AntiforgeryCookie}";

                //whether to allow the use of anti-forgery cookies from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        public static void AddDbToRestHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = $"{DbToRestCookieDefaults.Prefix}{DbToRestCookieDefaults.SessionCookie}";
                options.Cookie.HttpOnly = true;

                //whether to allow the use of session values from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        public static void AddDbToRestAuthentication(this IServiceCollection services)
        {
            //set default authentication schemes
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = DbToRestAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = DbToRestAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = DbToRestAuthenticationDefaults.ExternalAuthenticationScheme;
            });

            //add main cookie authentication
            authenticationBuilder.AddCookie(DbToRestAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{DbToRestCookieDefaults.Prefix}{DbToRestCookieDefaults.AuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.LoginPath = DbToRestAuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = DbToRestAuthenticationDefaults.AccessDeniedPath;

                //whether to allow the use of authentication cookies from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            //add external authentication
            authenticationBuilder.AddCookie(DbToRestAuthenticationDefaults.ExternalAuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{DbToRestCookieDefaults.Prefix}{DbToRestCookieDefaults.ExternalAuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.LoginPath = DbToRestAuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = DbToRestAuthenticationDefaults.AccessDeniedPath;

                //whether to allow the use of authentication cookies from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        public static IMvcBuilder AddDbToRestMvc(this IServiceCollection services)
        {
            //add basic MVC feature
            var mvcBuilder = services.AddMvc();

            //we use legacy (from previous versions) routing logic
            //   mvcBuilder.AddMvcOptions(options => options.EnableEndpointRouting = false);

            //sets the default value of settings on MvcOptions to match the behavior of asp.net core mvc 2.2
            mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var nopConfig = services.BuildServiceProvider().GetRequiredService<DbToRestConfig>();
            if (nopConfig.UseSessionStateTempDataProvider)
            {
                //use session-based temp data provider
                mvcBuilder.AddSessionStateTempDataProvider();
            }
            else
            {
                //use cookie-based temp data provider
                mvcBuilder.AddCookieTempDataProvider(options =>
                {
                    options.Cookie.Name = $"{DbToRestCookieDefaults.Prefix}{DbToRestCookieDefaults.TempDataCookie}";

                    //whether to allow the use of cookies from SSL protected page on the other store pages which are not
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                });
            }

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //add custom display metadata provider
            //mvcBuilder.AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new NopMetadataProvider()));

            //add custom model binder provider (to the top of the provider list)
            //mvcBuilder.AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new NopModelBinderProvider()));

            //add fluent validation
            //mvcBuilder.AddFluentValidation(configuration =>
            //{
            //    //register all available validators from Nop assemblies
            //    var assemblies = mvcBuilder.PartManager.ApplicationParts
            //        .OfType<AssemblyPart>()
            //        .Where(part => part.Name.StartsWith("Nop", StringComparison.InvariantCultureIgnoreCase))
            //        .Select(part => part.Assembly);
            //    configuration.RegisterValidatorsFromAssemblies(assemblies);

            //    //implicit/automatic validation of child properties
            //    configuration.ImplicitlyValidateChildProperties = true;
            //});

            //register controllers as services, it'll allow to override them
            mvcBuilder.AddControllersAsServices();

            return mvcBuilder;
        }

        //public static void AddRedirectResultExecutor(this IServiceCollection services)
        //{
        //    //we use custom redirect executor as a workaround to allow using non-ASCII characters in redirect URLs
        //    services.AddSingleton<IActionResultExecutor<RedirectResult>, NopRedirectResultExecutor>();
        //}

        //public static void AddObjectContext(this IServiceCollection services)
        //{
        //    services.AddDbContextPool<NopObjectContext>(optionsBuilder =>
        //    {
        //        optionsBuilder.UseSqlServerWithLazyLoading(services);
        //    });
        //}

        public static void AddEasyCaching(this IServiceCollection services)
        {
            services.AddEasyCaching(option =>
            {
                //use memory cache
                option.UseInMemory("nopCommerce_memory_cache");
            });
        }

        public static void AddHttpClients(this IServiceCollection services)
        {
            //default client
            /* services.AddHttpClient(NopHttpDefaults.DefaultHttpClient).WithProxy();

             //client to request current store
             services.AddHttpClient<StoreHttpClient>();

             //client to request nopCommerce official site
             services.AddHttpClient<NopHttpClient>().WithProxy();

             //client to request reCAPTCHA service
             services.AddHttpClient<CaptchaHttpClient>().WithProxy();
             */
        }
    }
}