using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy.Soap;

namespace NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions
{
    public class NominatedPharmacyEnvelopeService : INominatedPharmacyEnvelopeService
    {
        private readonly ILogger<NominatedPharmacyEnvelopeService> _logger;
        private readonly INominatedPharmacyConfigurationSettings _config;

        public NominatedPharmacyEnvelopeService(ILogger<NominatedPharmacyEnvelopeService> logger, INominatedPharmacyConfigurationSettings config)
        {
            _logger = logger;
            _config = config;
        }

        public string BuildEnvelope<T>(T request, IServiceDefinition serviceDefinition)
        {
            try
            {
                var nominatedPharmacyEnvelope = new NominatedPharmacyEnvelope(serviceDefinition, _config);
                nominatedPharmacyEnvelope.AddBody(request);

                return nominatedPharmacyEnvelope.Envelope.OuterXml;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Critical error building Nominated Pharmacy SOAP XML");
                throw;
            }
        }
    }
}
