using System.Collections.Generic;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Configuration;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Areas.KnownServices.Models
{
    public class KnownServicesV3: IValidatable
    {
        public Dictionary<string, KnownServiceV3> ServicesV3 { get; set; }

        public void Validate()
        {
            if (ServicesV3 == null || ServicesV3.Count == 0)
            {
                throw new ConfigurationNotValidException(nameof(ServicesV3));
            }

            ServicesV3.ForEach(ValidateKnownService);
        }

        private static void ValidateKnownService(KeyValuePair<string, KnownServiceV3> knownServicesEntry)
        {
            var (knownServiceId, knownService) = knownServicesEntry;

            knownService.Id = knownServiceId;

            if (knownService.Url is null)
            {
                throw new ConfigurationNotValidException(nameof(RootService.Url),
                    "Validation failed for KnownService with id: {knownServiceId}");
            }

            if (!knownService.Url.IsAbsoluteUri)
            {
                throw new ConfigurationNotValidException(nameof(RootService.Url),
                    $"Validation failed for KnownService with id: {knownServiceId}");
            }
        }
    }
}