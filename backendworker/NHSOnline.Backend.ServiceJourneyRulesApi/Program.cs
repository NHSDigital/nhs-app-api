using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    public static class Program
    {
        private enum RunMode
        {
            Validate,
            ServeWebApi
        }

        public static void Main(string[] args)
        {
            var runMode = DetermineRunModeFromCommandLineArgs(args);

            if (runMode == RunMode.Validate)
            {
                ValidateJourneyConfigurationFiles();
            }

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

        private static void ValidateJourneyConfigurationFiles()
        {
            var errorHandler = new ErrorHandler(new LoggerFactory().AddConsole().CreateLogger("SJR Configuration Validation"));

            var fileHandler = new FileHandler(errorHandler, Assembly.GetExecutingAssembly());

            var schemaValidator = new SchemaValidator(errorHandler);

            var deserializer = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();
            var serializer = new SerializerBuilder().JsonCompatible().Build();
            var yamlToJsonConverter = new YamlToJsonConverter(errorHandler, deserializer, serializer);

            var configurationRuleFileValidator =
                new ConfigurationRuleFileValidator(errorHandler, fileHandler, yamlToJsonConverter, schemaValidator);

            var exitCode = configurationRuleFileValidator.ValidateJourneyConfigurationFiles();

            Environment.Exit(exitCode);
        }

        private static RunMode DetermineRunModeFromCommandLineArgs(IEnumerable<string> args)
        {
            foreach (var s in args)
            {
                if (s.Contains(Constants.Args.ValidateMode, StringComparison.Ordinal))
                {
                    return RunMode.Validate;
                }
            }

            return RunMode.ServeWebApi;
        }
    }
}