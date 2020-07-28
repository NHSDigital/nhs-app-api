using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public sealed class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGpSessionCreator, GpSessionCreator>();

            services.AddTransient<GpSessionFilter>();

            services.Configure<MvcOptions>(opts =>
            {
                // Special binding source prevents the session parameters from being validated as part of the model state
                // https://stackoverflow.com/a/56893947
                opts.ModelMetadataDetailsProviders.Add(new BindingSourceMetadataProvider(typeof(GpUserSession), BindingSource.Special));
            });
        }
    }
}
