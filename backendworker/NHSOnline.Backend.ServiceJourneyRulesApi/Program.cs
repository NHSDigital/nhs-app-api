using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Service;

namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    public static class Program
    {
        internal enum RunMode
        {
            Validate,
            ServeWebApi
        }

        internal static RunMode Mode { get; private set; }

        public static async Task Main(string[] args)
        {
            Mode = DetermineRunModeFromCommandLineArgs(args);

            if (Mode == RunMode.Validate)
            {
                await BuildConsoleApp(args).RunAsync();
            }
            else
            {
                await BuildWebHost(args).RunAsync();
            }
        }

        private static IConfigurationRoot BuildConfiguration(string[] args)
            => SetupConfiguration(new ConfigurationBuilder(), args).Build();

        private static IConfigurationBuilder SetupConfiguration(IConfigurationBuilder builder, string[] args)
            => builder.AddEnvironmentVariables().AddCommandLine(args);
        
        private static IHost BuildConsoleApp(string[] args) =>
            new HostBuilder()
                .UseConsoleLifetime()
                .ConfigureAppConfiguration(config => SetupConfiguration(config, args))
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<ValidationService>();
                    new ServiceConfigurationModule().ConfigureServices(services, context.Configuration);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"))
                        .AddConsole();
                })
                .Build();

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(BuildConfiguration(args))
                // Clear default logging providers these will be added later in startup.
                .ConfigureLogging((context, logBuilder) => logBuilder.ClearProviders())
                .Build();

        private static RunMode DetermineRunModeFromCommandLineArgs(IEnumerable<string> args) => 
            args.Any(s => s.Contains(Constants.Args.ValidateMode, StringComparison.Ordinal)) 
                ? RunMode.Validate 
                : RunMode.ServeWebApi;
    }
}