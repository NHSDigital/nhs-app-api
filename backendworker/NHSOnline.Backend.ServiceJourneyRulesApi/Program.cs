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
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    public static class Program
    {
        internal enum RunMode
        {
            Validate,
            Export,
            ServeWebApi
        }

        internal static RunMode Mode { get; private set; }

        public static async Task Main(string[] args)
        {
            Mode = DetermineRunModeFromCommandLineArgs(args);

            switch (Mode)
            {
                case RunMode.Validate:
                    await BuildConsoleApp(args).RunAsync();
                    break;
                case RunMode.Export:
                    await BuildExportApp(args).RunAsync();
                    break;
                default:
                    await BuildWebHost(args).RunAsync();
                    break;
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

        private static IHost BuildExportApp(string[] args) =>
            new HostBuilder()
                .UseConsoleLifetime()
                .ConfigureAppConfiguration(config => SetupConfiguration(config, args))
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<ExportService>();
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
                .ConfigureLogging((context, logBuilder) => logBuilder.ConfigureNhsAppLogging(context.Configuration))
                .Build();

        private static RunMode DetermineRunModeFromCommandLineArgs(IEnumerable<string> args) =>
            args.Any(s => s.Contains(Constants.Args.ValidateMode, StringComparison.Ordinal))
                ? RunMode.Validate :
                    args.Any(s => s.Contains(Constants.Args.ExportMode, StringComparison.Ordinal))
                    ? RunMode.Export
                    : RunMode.ServeWebApi;
    }
}