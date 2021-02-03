using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Settings
{
    public class OnlineConsultationsConfigurationSettings
    {
        public OnlineConsultationsConfigurationSettings(int onlineConsultationsHttpTimeoutSeconds)
        {
            OnlineConsultationsHttpTimeoutSeconds = onlineConsultationsHttpTimeoutSeconds;
        }

        public int OnlineConsultationsHttpTimeoutSeconds { get; set; }

        public static OnlineConsultationsConfigurationSettings CreateAndValidate(IConfiguration configuration, ILogger logger)
        {
            var onlineConsultationsHttpTimeoutSeconds  = configuration.GetIntOrWarn("ConfigurationSettings:OnlineConsultationsHttpTimeoutSeconds", logger);
            var config = new OnlineConsultationsConfigurationSettings(onlineConsultationsHttpTimeoutSeconds);

            config.Validate();
            return config;
        }

        private void Validate()
        {
            if (OnlineConsultationsHttpTimeoutSeconds < 1)
            {
                throw new ConfigurationNotValidException(nameof(OnlineConsultationsHttpTimeoutSeconds));
            }
        }
    }
}