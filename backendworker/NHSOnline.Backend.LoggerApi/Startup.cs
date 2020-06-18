using CorrelationId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.DependencyInjection;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Middleware;

namespace NHSOnline.Backend.LoggerApi
{
    public class Startup
    {
        private readonly string _apiAppVersion;
        private IConfiguration Configuration { get; }
        private readonly ModularStartup _modularStartup;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;

            _apiAppVersion = Configuration.GetApiAppVersion();

            _modularStartup = new ModularStartup(configuration, loggerFactory);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services
                .AddControllers(ConfigureMvcOptions)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            services.AddOptions();
            services.AddCorrelationId();

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            services.AddSingleton(Configuration);

            services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            _modularStartup.ConfigureServices(services);
        }

        private static void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add(typeof(ModelStateValidationFilterAttribute), 1);
            options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.Use(async (context, next) =>
            {
                var logger = loggerFactory.CreateLogger<Startup>();
                logger.LogVersion(context, "LoggerApi", _apiAppVersion);
                await next();
            });

            app.UsePathBase("/v1");

            app.UseSecurityResponseHeadersMiddleware();
            app.UseResponseHeadersMiddleware();

            app.UseRouting();
            app.UseCors(Configuration);
            app.UseAuthentication();

            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = Constants.HttpHeaders.CorrelationIdentifier,
                UseGuidForCorrelationId = true,
                UpdateTraceIdentifier = false
            });

            app.UseLogRequestHeader(new LogRequestHeaderOptions
            {
                HeaderName = Constants.HttpHeaders.CorrelationIdentifier,
                LogTemplate = "CorrelationId={value}",
            });

            app.UseEndpoints(c => c.MapControllers());
        }
    }
}
