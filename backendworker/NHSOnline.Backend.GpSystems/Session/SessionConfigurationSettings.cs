using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Session
{
    public class SessionConfigurationSettings
    {
        public bool ProxyEnabled { get; set; }
        public SessionConfigurationSettings(bool proxyEnabled)
        {
            ProxyEnabled = proxyEnabled;
        }

        public static SessionConfigurationSettings CreateAndValidate(IConfiguration configuration, ILogger logger)
        {
            var proxyEnabled = bool.TrueString.Equals(configuration.GetOrThrow("PROXY_ACCESS_ENABLED", logger), StringComparison.OrdinalIgnoreCase);
            return new SessionConfigurationSettings(proxyEnabled);
        }
    }
}