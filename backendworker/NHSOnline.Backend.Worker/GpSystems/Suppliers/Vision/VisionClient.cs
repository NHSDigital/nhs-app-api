using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Soap;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.VisionServiceDefinition;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionClient : IVisionClient
    {
        private readonly VisionHttpClient _httpClient;
        private readonly string _targetUri;
        private readonly string _requestUsername;
        private readonly string _providerId;
        private readonly X509Certificate2 _certificate;
        private readonly ILogger<VisionClient> _logger;
        private static ICertificateService _certificateService;
        private static IEnvelopeService _envelopeService;
        private readonly IVisionConfig _visionConfig;

        private const string MediaType = "text/xml";

        public VisionClient(IVisionConfig visionConfig, ILoggerFactory loggerFactory, ICertificateService certificateService, IEnvelopeService envelopeService, VisionHttpClient httpClient)
        {
            _targetUri = visionConfig.ApiUrl;
            _requestUsername = visionConfig.RequestUsername;
            _providerId = visionConfig.ApplicationProviderId;
            _logger = loggerFactory.CreateLogger<VisionClient>();
            _certificateService = certificateService;
            _envelopeService = envelopeService;
            _httpClient = httpClient;
            _visionConfig = visionConfig;
            _certificate =
            _certificateService.GetCertificate(_visionConfig.CertificatePath, _visionConfig.CertificatePassphrase);
        }

        public async Task<VisionApiObjectResponse<PatientConfiguration>> GetConfiguration(VisionConnectionToken token, string odsCode)
        {
            IVisionServiceDefinition visionServiceDefinition = new ConfigurationServiceDefinition();

            var visionRequest = new VisionRequest<Object>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                token.RosuAccountId, token.ApiKey, odsCode, _providerId, null);

            return await SendRequestAndParseResponse<PatientConfiguration, VisionRequest<Object>>(visionRequest);
        }

        public async Task<VisionApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse, T>(T request)
        {
            var uri = new Uri(_targetUri);

            var envelope = _envelopeService.BuildEnvelope(_certificate, request, _requestUsername);

            var response = await TransmitAsync<TResponse>(uri, envelope);

            return response;
        }

        private async Task<VisionApiObjectResponse<TResponse>> TransmitAsync<TResponse>(Uri uri, string envelope)
        {
            var request =
                new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(envelope, Encoding.UTF8, MediaType)
                };

            var thirdpartyEnvelope = await SendRequestAndParseResponse<TResponse>(request);
            return thirdpartyEnvelope;
        }

        private async Task<VisionApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>
            (HttpRequestMessage request)
        {
            try
            {
                _logger.LogInformation($"{nameof(VisionClient)} sending request");

                var responseMessage = await _httpClient.Client.SendAsync(request);

                var response = new VisionApiObjectResponse<TResponse>(responseMessage.StatusCode);

                var stringResponse = responseMessage.Content != null
                    ? await responseMessage.Content.ReadAsStringAsync()
                    : null;

                if (string.IsNullOrEmpty(stringResponse)) return response;

                try
                {
                    var responseObject = Deserializer<VisionResponseEnvelope<TResponse>>(stringResponse);
                    response.RawResponse = responseObject;
                }
                catch (FormatException e)
                {
                    _logger.LogError(e, "An error occured while parsing the response");
                    return new VisionApiObjectResponse<TResponse>(HttpStatusCode.InternalServerError);
                }

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private T Deserializer<T>(string request)
        {
            T instance = default(T);
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var stringreader = new StringReader(request))
                {
                    instance = (T) xmlSerializer.Deserialize(stringreader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return instance;
        }
        
        public class VisionApiResponse
        {
            public VisionApiResponse(HttpStatusCode status)
            {
                StatusCode = status;
            }

            public HttpStatusCode StatusCode { get; set; }
        }

        public class VisionApiObjectResponse<TBody> : VisionApiResponse
        {
            public VisionApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {
            }

            public VisionResponseEnvelope<TBody> RawResponse { get; set; }

            public TBody Body => RawResponse.Body.VisionResponse.ServiceContent.Payload;
            
            public bool HasErrorResponse
            {
                get
                {
                    return !StatusCodeIndicatesSuccess
                        || RawResponse?.Body?.Fault != null
                        || RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Successful?.ToLower() == bool.FalseString.ToLower();
                }
            }

            // Note
            // We don't know whether Vision always populates certain properties when there is an error.
            // So there are a lot of null checks below using the null conditional operator.

            public bool IsInvalidRequestError => (RawResponse?.Body?.Fault?.Detail?.VisionFault?.Error?.Category ?? "").Contains("INVALID_REQUEST");

            public bool IsInvalidUserCredentialsError => RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code == "-30";

            public bool IsInvalidSecurtyHeaderError => (RawResponse?.Body?.Fault?.FaultCode ?? "").Contains("InvalidSecurity");

            public bool IsUnknownError => (RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code == "-100");

            private bool StatusCodeIndicatesSuccess => StatusCode == HttpStatusCode.OK;
        }
    }
}