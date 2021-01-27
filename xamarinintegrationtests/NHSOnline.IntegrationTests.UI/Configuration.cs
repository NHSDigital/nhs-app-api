using System.IO;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.IntegrationTests.UI
{
    internal static class Configuration
    {
        internal static void Initialize()
        {
            ConfigurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }

        private static IConfigurationRoot ConfigurationRoot { get; set; } = null!;

        internal static TConfig Get<TConfig>(string name)
            where TConfig : new()
            => ConfigurationRoot.GetSection(name).Get<TConfig>() ?? new TConfig();
    }
}