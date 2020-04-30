using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Configuration;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration.Models
{
    public class KnownServices : IValidatable
    {
        public Dictionary<string, RootService> Services { get; set; }

        public void Validate()
        {
            if (Services == null || Services.Count == 0)
            {
                throw new ConfigurationNotValidException(nameof(Services));
            }

            Services.ForEach(ValidateKnownService);
        }

        private static void ValidateKnownService(KeyValuePair<string, RootService> knownServicesEntry)
        {
            var knownServiceId = knownServicesEntry.Key;
            var knownService = knownServicesEntry.Value;

            knownService.Id = knownServiceId;

            if (knownService.Url is null)
            {
                throw new ConfigurationNotValidException(nameof(RootService.Url));
            }

            if (!knownService.Url.IsAbsoluteUri)
            {
                throw new ConfigurationNotValidException(nameof(RootService.Url));
            }

            knownService.SubServices?.ForEach(ValidateSubService);
        }

        private static void ValidateSubService(SubService subService)
        {
            if (subService is null)
            {
                throw new ConfigurationNotValidException(nameof(SubService));
            }

            if (string.IsNullOrWhiteSpace(subService.Path))
            {
                throw new ConfigurationNotValidException(nameof(SubService.Path));
            }
        }

        public void FixUpWebAppBaseUrl(Uri settingsWebAppBaseUrl)
        {
            var webAppKnownService = Services.Values.SingleOrDefault(x =>
                string.Equals(x.Url.Host, "WebAppBaseUrl", StringComparison.OrdinalIgnoreCase));

            if (webAppKnownService != null)
            {
                webAppKnownService.Url = settingsWebAppBaseUrl;
            }
        }
    }
}