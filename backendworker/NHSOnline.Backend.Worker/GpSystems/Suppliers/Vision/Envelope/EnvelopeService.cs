using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Soap;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Envelope
{
    public class EnvelopeService : IEnvelopeService
    {
        private readonly ILogger<EnvelopeService> _logger;

        public EnvelopeService(ILogger<EnvelopeService> logger)
        {
            _logger = logger;
        }

        private static VisionEnvelope GetVisionEnvelope(X509Certificate2 certificate, string requestUsername)
        {
            return new VisionEnvelope(certificate, requestUsername);
        }

        public string BuildEnvelope<T>(X509Certificate2 certificate, T request, string requestUsername)
        {
            try
            {
                var visionEnvelope = GetVisionEnvelope(certificate, requestUsername);
                visionEnvelope.AddBody(request);

                return visionEnvelope.Envelope.OuterXml;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Critical error building Vision SOAP XML");
                _logger.LogCritical(e.ToString());
                throw;
            }
        }
    }
}
