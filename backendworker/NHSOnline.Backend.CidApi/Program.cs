using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NHSOnline.Backend.GpSystems.Suppliers.Fake;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.CidApi
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IConfigurationRoot BuildConfiguration(string[] args) => new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .AddFakeGpConfigurationFile()
            .Build();

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(BuildConfiguration(args))
                .ConfigureLogging((context, logBuilder) => logBuilder.ConfigureNhsAppLogging(context.Configuration))
                .Build();
    }
}
