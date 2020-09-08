using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.HealthChecks;
using NHSOnline.Backend.ServiceJourneyRulesApi.Extensions;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.DependencyInjection;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Middleware;

namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private readonly ModularStartup _modularStartup;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;

            _modularStartup = new ModularStartup(configuration, loggerFactory);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(ConfigureMvcOptions)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            services.AddNhsAppHealthCheckService();

            services.AddSingleton(Configuration);
            services.AddSingleton(typeof(HttpTimeoutHandler<>));
            services.AddSingleton(typeof(HttpRequestIdentificationHandler<>));
            services.AddSingleton(services);
            services.AddOptions();

            _modularStartup.ConfigureServices(services);

            services.ConfigureJourneyRepository();
        }

        private static void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add(typeof(ModelStateValidationFilterAttribute), 1);
            options.Filters.Add(typeof(TimeoutExceptionFilterAttribute));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UsePathBase("/v1");

            app.UseSecurityResponseHeadersMiddleware();
            app.UseResponseHeadersMiddleware();

            app.UseRouting();
            app.UseCors(Configuration);

            app.UseLogRequestHeader(new LogRequestHeaderOptions
            {
               HeaderName = Support.Constants.HttpHeaders.CorrelationIdentifier,
               LogTemplate = "CorrelationId={value}",
            });

            app.UseEndpoints(b =>
            {
                b.MapHealthCheckEndpoints();
                b.MapControllers();
            });
        }
    }
}