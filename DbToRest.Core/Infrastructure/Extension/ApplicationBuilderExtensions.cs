using DbToRest.Core.Data;
using DbToRest.Core.Infrastructure.ComponentModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace DbToRest.Core.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);

            application.UseExceptionHandler();
            application.UseBadRequestResult();
            application.UseStaticFiles();
            application.UseKeepAlive();
            application.UseAuthentication();

            if (EngineContext.Current.Resolve<IHostingEnvironment>().IsStaging() == false)
                application.UseMvc();


        }

        public static void UseExceptionHandler(this IApplicationBuilder application)
        {
            var config = EngineContext.Current.Resolve<DbToRestConfig>();
            var hostingEnvironment = EngineContext.Current.Resolve<IHostingEnvironment>();
            var useDetailedExceptionPage = config.DisplayFullErrorStack || hostingEnvironment.IsDevelopment();
            if (useDetailedExceptionPage)
            {
                //get detailed exceptions for developing and testing purposes
                application.UseDeveloperExceptionPage();
            }
            else
            {
                //or use special exception handler
                application.UseExceptionHandler("/Error/Error");
            }

            /*
            //log errors
            application.UseExceptionHandler(handler =>
            {
                handler.Run(context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return Task.CompletedTask;

                    try
                    {
                        //check whether database is installed
                        if (CommonHelper.DatabaseIsInstalled)
                        {
                            //get current customer
                            // var currentCustomer = EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer;

                            //log error
                            //EngineContext.Current.Resolve<ILogger>().Error(exception.Message, exception, currentCustomer);
                        }
                    }
                    finally
                    {
                        //rethrow the exception to show the error page
                        ExceptionDispatchInfo.Throw(exception);
                    }

                    return Task.CompletedTask;
                });
            });
            */
        }

        public static void UsePageNotFound(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    if (!webHelper.IsStaticResource())
                    {
                        var originalPath = context.HttpContext.Request.Path;
                        var originalQueryString = context.HttpContext.Request.QueryString;

                        context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature
                        {
                            OriginalPathBase = context.HttpContext.Request.PathBase.Value,
                            OriginalPath = originalPath.Value,
                            OriginalQueryString = originalQueryString.HasValue ? originalQueryString.Value : null
                        });

                        context.HttpContext.Request.Path = "/page-not-found";
                        context.HttpContext.Request.QueryString = QueryString.Empty;

                        try
                        {
                            await context.Next(context.HttpContext);
                        }
                        finally
                        {
                            context.HttpContext.Request.QueryString = originalQueryString;
                            context.HttpContext.Request.Path = originalPath;
                            context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(null);
                        }
                    }
                }
            });
        }

        public static void UseBadRequestResult(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(context =>
            {
                //handle 404 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    var logger = EngineContext.Current.Resolve<ILogService>();
                    logger.Error("Error 400. Bad request");
                }

                return Task.CompletedTask;
            });
        }

        public static void UseResponseCompression(this IApplicationBuilder application)
        {
            ////whether to use compression (gzip by default)
            //if (DataSettingsManager.DatabaseIsInstalled && EngineContext.Current.Resolve<CommonSettings>().UseResponseCompression)
            //    application.UseResponseCompression();
        }

        public static void UseStaticFiles(this IApplicationBuilder application)
        {
            void staticFileResponse(StaticFileResponseContext context)
            {
                if (!CommonHelper.DatabaseIsInstalled)
                    return;

                //var commonSettings = EngineContext.Current.Resolve<CommonSettings>();
                //if (!string.IsNullOrEmpty(commonSettings.StaticFilesCacheControl))
                //    context.Context.Response.Headers.Append(HeaderNames.CacheControl, commonSettings.StaticFilesCacheControl);
            }

            var fileProvider = EngineContext.Current.Resolve<IDbToRestFileProvider>();

            application.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = staticFileResponse });

            //application.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Themes")),
            //    RequestPath = new PathString("/Themes"),
            //    OnPrepareResponse = staticFileResponse
            //});

            //var staticFileOptions = new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Plugins")),
            //    RequestPath = new PathString("/Plugins"),
            //    OnPrepareResponse = staticFileResponse
            //};

            //application.UseStaticFiles(staticFileOptions);

            //var provider = new FileExtensionContentTypeProvider
            //{
            //    Mappings = { [".bak"] = MimeTypes.ApplicationOctetStream }
            //};

            //application.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath("db_backups")),
            //    RequestPath = new PathString("/db_backups"),
            //    ContentTypeProvider = provider
            //});

            //provider.Mappings[".webmanifest"] = MimeTypes.ApplicationManifestJson;

            //application.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath("icons")),
            //    RequestPath = "/icons",
            //    ContentTypeProvider = provider
            //});
        }

        public static void UseKeepAlive(this IApplicationBuilder application)
        {
            //application.UseMiddleware<KeepAliveMiddleware>();
        }

        public static void UseInstallUrl(this IApplicationBuilder application)
        {
            // application.UseMiddleware<InstallUrlMiddleware>();
        }

        public static void UseAuthentication(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!CommonHelper.DatabaseIsInstalled)
                return;

            //  application.UseMiddleware<AuthenticationMiddleware>();
        }

        public static void UseRequestLocalization(this IApplicationBuilder application)
        {
            application.UseRequestLocalization(options =>
            {
                if (!CommonHelper.DatabaseIsInstalled)
                    return;

                //prepare supported cultures
                //var cultures = EngineContext.Current.Resolve<ILanguageService>().GetAllLanguages()
                //    .OrderBy(language => language.DisplayOrder)
                //    .Select(language => new CultureInfo(language.LanguageCulture)).ToList();
                //options.SupportedCultures = cultures;
                //options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault());
            });
        }

        public static void UseCulture(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!CommonHelper.DatabaseIsInstalled)
                return;

            //application.UseMiddleware<CultureMiddleware>();
        }

        public static void UseMvc(this IApplicationBuilder application)
        {
            application.UseMvc(routeBuilder =>
            {
                routeBuilder.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}