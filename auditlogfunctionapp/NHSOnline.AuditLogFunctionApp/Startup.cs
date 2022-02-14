using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.AuditLogFunctionApp;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NHSOnline.AuditLogFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder) =>
            builder.Services.AddLogging();
    }
}
