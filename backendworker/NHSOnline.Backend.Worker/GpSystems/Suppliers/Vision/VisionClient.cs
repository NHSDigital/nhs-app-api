using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.VisionServiceDefinition;
using NHSOnline.Backend.Worker.Support.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionClient : IVisionClient
    {
        private readonly VisionHttpClient _httpClient;
        private readonly Uri _targetUri;
        private readonly string _requestUsername;
        private readonly string _providerId;
        private readonly X509Certificate2 _certificate;
        private readonly ILogger<VisionClient> _logger;
        private static IEnvelopeService _envelopeService;

        private const string MediaType = "text/xml";

        public VisionClient(IVisionConfig visionConfig,
            ILoggerFactory loggerFactory,
            ICertificateService certificateService,
            IEnvelopeService envelopeService,
            VisionHttpClient httpClient)
        {
            _targetUri = visionConfig.ApiUrl;
            _requestUsername = visionConfig.RequestUsername;
            _providerId = visionConfig.ApplicationProviderId;
            _logger = loggerFactory.CreateLogger<VisionClient>();
            _envelopeService = envelopeService;
            _httpClient = httpClient;
            _certificate =
                certificateService.GetCertificate(visionConfig.CertificatePath, visionConfig.CertificatePassphrase);
        }
            
        public async Task<VisionClient.VisionApiObjectResponse<VisionDemographicsResponse>> GetDemographics(VisionUserSession visionUserSession,
            DemographicsRequest requestContent)
        {
            IVisionServiceDefinition visionServiceDefinition = new DemographicsServiceDefinition();
              
            var visionRequest = new VisionRequest<DemographicsRequest>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                visionUserSession.RosuAccountId, visionUserSession.ApiKey, visionUserSession.OdsCode, _providerId, requestContent);
            
            return await SendRequestAndParseResponse<VisionDemographicsResponse, VisionRequest<DemographicsRequest>>(visionRequest);   
        }
        
        public async Task<VisionApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(VisionConnectionToken token, string odsCode)
        {
            IVisionServiceDefinition visionServiceDefinition = new ConfigurationServiceDefinition();

            var visionRequest = new VisionRequest<Object>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                token.RosuAccountId, token.ApiKey, odsCode, _providerId, null);

            return await SendRequestAndParseResponse<PatientConfigurationResponse, VisionRequest<Object>>(visionRequest);
        }

        public async Task<VisionApiObjectResponse<PrescriptionHistoryResponse>> GetHistoricPrescriptions(VisionUserSession userSession, PrescriptionRequest prescriptionRequest)
        {
            IVisionServiceDefinition visionServiceDefinition = new PrescriptionHistoryServiceDefinition();
            
            var visionRequest = new VisionRequest<PrescriptionRequest>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                userSession.RosuAccountId, userSession.ApiKey, userSession.OdsCode, _providerId, prescriptionRequest);

            return await SendRequestAndParseResponse<PrescriptionHistoryResponse, VisionRequest<PrescriptionRequest>>(visionRequest);
        }

        public async Task<VisionApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse, T>(T request)
        {
            var envelope = _envelopeService.BuildEnvelope(_certificate, request, _requestUsername);

            var response = await TransmitAsync<TResponse>(envelope);

            return response;
        }

        private async Task<VisionApiObjectResponse<TResponse>> TransmitAsync<TResponse>(string envelope)
        {
            var request =
                new HttpRequestMessage(HttpMethod.Post, _targetUri)
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

        private static T Deserializer<T>(string request)
        {
            T instance;

            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var stringreader = new StringReader(request))
            {
                instance = (T) xmlSerializer.Deserialize(stringreader);
            }

            return instance;
        }
        
        public class VisionApiResponse
        {
            protected VisionApiResponse(HttpStatusCode status)
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

            public TBody Body => RawResponse.Body.VisionResponse.ServiceContent;
            
            public bool HasErrorResponse
            {
                get
                {
                    return !StatusCodeIndicatesSuccess
                        || RawResponse?.Body?.Fault != null
                        || bool.FalseString.Equals(RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Successful, StringComparison.OrdinalIgnoreCase);
                }
            }

            public string ErrorContent
            {
                get
                {
                    return 
                        $"fault: { JsonConvert.SerializeObject(RawResponse?.Body?.Fault) }, " +
                        $"error: { JsonConvert.SerializeObject(RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error) }";
                }
            }

            public bool HasForbiddenResponse => StatusCode == HttpStatusCode.Forbidden;
            
            // Note
            // We don't know whether Vision always populates certain properties when there is an error.
            // So there are a lot of null checks below using the null conditional operator.

            public bool IsInvalidRequestError => (RawResponse?.Body?.Fault?.Detail?.VisionFault?.Error?.Category ?? "").Contains("INVALID_REQUEST", StringComparison.Ordinal);

            public bool IsInvalidUserCredentialsError => "-30".Equals(
                RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code, StringComparison.Ordinal);

            public bool IsInvalidSecurtyHeaderError => (RawResponse?.Body?.Fault?.FaultCode ?? "").Contains("InvalidSecurity", StringComparison.Ordinal);

            public bool IsUnknownError =>
                "-100".Equals(RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code,
                    StringComparison.Ordinal);
            
            public bool IsAccessDeniedError =>
                "-35".Equals(RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code,
                    StringComparison.Ordinal);

            private bool StatusCodeIndicatesSuccess => StatusCode == HttpStatusCode.OK;
        }
    }
}