using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Soap;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope
{
    internal sealed class EnvelopeService : IEnvelopeService
    {
        private readonly ILogger<EnvelopeService> _logger;
        private readonly VisionPfsCertificate _visionPfsCertificate;

        public EnvelopeService(
            ILogger<EnvelopeService> logger,
            VisionPfsCertificate visionPfsCertificate)
        {
            _logger = logger;
            _visionPfsCertificate = visionPfsCertificate;
        }

        private static VisionEnvelope GetVisionEnvelope(X509Certificate2 certificate, string requestUsername)
        {
            return new VisionEnvelope(certificate, requestUsername);
        }

        public string BuildEnvelope<T>(T request, string requestUsername)
        {
            try
            {
                new ValidateAndLog(_logger)
                    .IsNotNull(request, nameof(request))
                    .IsNotNull(requestUsername, nameof(requestUsername))
                    .IsValid();

                var visionEnvelope = GetVisionEnvelope(_visionPfsCertificate.Certificate, requestUsername);
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
