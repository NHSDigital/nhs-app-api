using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Certificate
{
    public class AuthSigningConfig : IKeyConfig
    {
        public string KeyPath { get; }
        public string Password { get; }

        public AuthSigningConfig(IConfiguration configuration, ILogger<AuthSigningConfig> logger)
        {
            KeyPath = configuration.GetOrWarn("AUTH_SIGNING_KEY", logger);
            Password = configuration.GetOrWarn("AUTH_SIGNING_PASSWORD", logger);
        }
    }
}