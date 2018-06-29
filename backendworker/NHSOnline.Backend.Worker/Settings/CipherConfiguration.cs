using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Worker.Settings
{
    public class CipherConfiguration
    {
        public string CipherKeyFilePath { get; set; }

        public const string CipherKeyFilePathConfigurationName = "SESSION_ENCRYPTION_KEY_FILE";

        public CipherConfiguration(IConfiguration configuration)
        {
            CipherKeyFilePath = configuration[CipherKeyFilePathConfigurationName];
            EnsureConfigurationSettingsPopulated();
        }

        private void EnsureConfigurationSettingsPopulated()
        {
            if (CipherKeyFilePath == null)
            {
                throw new ConfigurationNotFoundException(nameof(CipherKeyFilePath));
            }
        }
    }
}
