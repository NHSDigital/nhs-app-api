using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddFakeGpConfigurationFile(
            this IConfigurationBuilder configurationBuilder
        )
        {
            var fakeGpUserConfigPath = EnvironmentExtensions.GetOrThrow("FAKE_GP_SUPPLIER_CONFIG_PATH");

            return configurationBuilder.AddYamlFile(
                $"{fakeGpUserConfigPath}",
                optional: false,
                reloadOnChange: false
            );
        }
    }
}
