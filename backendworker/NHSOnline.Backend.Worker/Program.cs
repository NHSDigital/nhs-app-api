using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IConfigurationRoot BuildConfiguration(string[] args) => new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(BuildConfiguration(args))
                // Clear default logging providers these will be added later in startup.
                .ConfigureLogging((context, logBuilder) => logBuilder.ClearProviders())
                .Build();
    }
}
