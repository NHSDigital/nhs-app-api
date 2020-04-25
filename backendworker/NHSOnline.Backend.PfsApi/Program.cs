using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IConfigurationRoot BuildConfiguration(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("KNOWNSERVICES_PATH");

            return new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{environment}", optional: false, reloadOnChange: false)
                .Build();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(BuildConfiguration(args))
                .ConfigureLogging((context, logBuilder) => logBuilder.ConfigureNhsAppLogging(context.Configuration))
                .Build();
    }
}
