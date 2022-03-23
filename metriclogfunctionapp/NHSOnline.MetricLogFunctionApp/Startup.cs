using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using NHSOnline.MetricLogFunctionApp;
using NHSOnline.MetricLogFunctionApp.Database.EntityFramework;
using NHSOnline.MetricLogFunctionApp.Etl;
using NHSOnline.MetricLogFunctionApp.Monitoring;
using NHSOnline.MetricLogFunctionApp.Slack;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NHSOnline.MetricLogFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddEntityFramework();
            builder.Services.AddMonitoring();
            builder.Services.AddSlack();
            builder.Services.AddEtl();
        }
    }
}
