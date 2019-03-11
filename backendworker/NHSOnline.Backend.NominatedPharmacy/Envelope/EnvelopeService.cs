using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy.Soap;

namespace NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions
{
    public class EnvelopeService : IEnvelopeService
    {
        private readonly ILogger<EnvelopeService> _logger;
        private readonly INominatedPharmacyConfig _config;

        public EnvelopeService(ILogger<EnvelopeService> logger, INominatedPharmacyConfig config)
        {
            _logger = logger;
            _config = config;
        }

        public string BuildEnvelope<T>(T request, IServiceDefinition serviceDefinition)
        {
            try
            {
                var nominatedPharmacyEnvelope = new NominatedPharmacyEnvelope(serviceDefinition, _config, _logger);
                nominatedPharmacyEnvelope.AddBody(request);

                _logger.LogInformation(nominatedPharmacyEnvelope.Envelope.OuterXml);

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
