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
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses;
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
            
        public async Task<VisionApiObjectResponse<VisionDemographicsResponse>> GetDemographics(VisionUserSession visionUserSession,
            DemographicsRequest requestContent)
        {
            IVisionServiceDefinition visionServiceDefinition = new DemographicsServiceDefinition();
              
            var visionRequest = new VisionRequest<DemographicsRequest>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                visionUserSession.RosuAccountId, visionUserSession.ApiKey, visionUserSession.OdsCode, _providerId, requestContent);
            
            return await SendRequestAndParseResponse<VisionDemographicsResponse, VisionRequest<DemographicsRequest>>(visionRequest);   
        }
        
        public async Task<VisionApiObjectResponse<VisionPatientDataResponse>> GetPatientData(
            VisionUserSession visionUserSession, PatientDataRequest requestContent)
        {
            IVisionServiceDefinition visionServiceDefinition = new PatientDataServiceDefinition();
            var visionRequest = new VisionRequest<PatientDataRequest>(visionServiceDefinition.Name,
                visionServiceDefinition.Version,
                visionUserSession.RosuAccountId, visionUserSession.ApiKey, visionUserSession.OdsCode, _providerId,
                requestContent);

            return await SendRequestAndParseResponse<VisionPatientDataResponse, VisionRequest<PatientDataRequest>>(
                visionRequest);
        }
        
        public async Task<VisionApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(
            VisionConnectionToken token, string odsCode)
        {
            IVisionServiceDefinition visionServiceDefinition = new ConfigurationServiceDefinition();

            var visionRequest = new VisionRequest<Object>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                token.RosuAccountId, token.ApiKey, odsCode, _providerId, null);

            return await SendRequestAndParseResponse<PatientConfigurationResponse, VisionRequest<Object>>(
                visionRequest);
        }

        public async Task<VisionApiObjectResponse<EligibleRepeatsResponse>> GetEligibleRepeats(
            VisionUserSession session)
        {
            IVisionServiceDefinition visionServiceDefinition = new EligibleRepeatsServiceDefinition();

            var visionRequest = new VisionRequest<CoursesRequest>(visionServiceDefinition.Name,
                visionServiceDefinition.Version,
                session.RosuAccountId, session.ApiKey, session.OdsCode, _providerId,
                new CoursesRequest { PatientId = session.PatientId });

            return await SendRequestAndParseResponse<EligibleRepeatsResponse, VisionRequest<CoursesRequest>>(
                visionRequest);
        }
        
        public async Task<VisionApiObjectResponse<PrescriptionHistoryResponse>> GetHistoricPrescriptions(VisionUserSession userSession, PrescriptionRequest prescriptionRequest)
        {
            IVisionServiceDefinition visionServiceDefinition = new PrescriptionHistoryServiceDefinition();
            
            var visionRequest = new VisionRequest<PrescriptionRequest>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                userSession.RosuAccountId, userSession.ApiKey, userSession.OdsCode, _providerId, prescriptionRequest);

            return await SendRequestAndParseResponse<PrescriptionHistoryResponse, VisionRequest<PrescriptionRequest>>(visionRequest);
        }

        public async Task<VisionApiObjectResponse<OrderNewPrescriptionResponse>> OrderNewPrescription(VisionUserSession userSession, OrderNewPrescriptionRequest newPrescriptionRequest)
        {
            IVisionServiceDefinition visionServiceDefinition = new NewPrescriptionServiceDefinition();

            var visionRequest = new VisionRequest<OrderNewPrescriptionRequest>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                userSession.RosuAccountId, userSession.ApiKey, userSession.OdsCode, _providerId, newPrescriptionRequest);

            return await SendRequestAndParseResponse<OrderNewPrescriptionResponse, VisionRequest<OrderNewPrescriptionRequest>>(visionRequest);
        }
        
        public async Task<VisionApiObjectResponse<BookedAppointmentsResponse>> GetExistingAppointments(
            VisionUserSession userSession
            )
        {
            IVisionServiceDefinition visionServiceDefinition = new GetExistingAppointmentsServiceDefinition();

            var visionRequest = new VisionRequest<PatientId>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                userSession.RosuAccountId, userSession.ApiKey, userSession.OdsCode, _providerId, new PatientId { Id = userSession.PatientId });

            return await SendRequestAndParseResponse<BookedAppointmentsResponse, VisionRequest<PatientId>>(visionRequest);
        }
        
        public async Task<VisionApiObjectResponse<AvailableAppointmentsResponse>> GetAvailableAppointments(
            VisionUserSession visionUserSession, AppointmentSlotsDateRange dateRange)
        {
            IVisionServiceDefinition visionServiceDefinition = new GetAvailableAppointmenServiceDefinition();

            var request = new AvailableAppointmentsRequest
            {
                PatientId = visionUserSession.PatientId,
                Page = new Page
                {
                    Number = 1,
                    SlotsPerPage = 1000,
                },
                DateRange = new DateRange
                {
                    From = dateRange.FromDate.Date,
                    To = dateRange.ToDate.Date,
                }
            };
            var visionRequest = new VisionRequest<AvailableAppointmentsRequest>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                visionUserSession.RosuAccountId, visionUserSession.ApiKey, visionUserSession.OdsCode, _providerId, request);
            
            return await SendRequestAndParseResponse<AvailableAppointmentsResponse, VisionRequest<AvailableAppointmentsRequest>>(visionRequest);
        }
        
        public async Task<VisionApiObjectResponse<BookAppointmentResponse>> BookAppointment(
            VisionUserSession userSession,
            BookAppointmentRequest bookAppointmentRequest
        )
        {
            IVisionServiceDefinition visionServiceDefinition = new BookAppointmentServiceDefinition();

            var visionRequest = new VisionRequest<BookAppointmentRequest>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                userSession.RosuAccountId, userSession.ApiKey, userSession.OdsCode, _providerId, bookAppointmentRequest);

            return await SendRequestAndParseResponse<BookAppointmentResponse, VisionRequest<BookAppointmentRequest>>(visionRequest);
        }

        public async Task<VisionApiObjectResponse<CancelledAppointmentResponse>> CancelAppointment(
            VisionUserSession userSession, CancelAppointmentRequest request
        )
        {
            IVisionServiceDefinition visionServiceDefinition = new CancelAppointmentServiceDefinition();
            
            var visionRequest = new VisionRequest<CancelAppointmentRequest>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                userSession.RosuAccountId, userSession.ApiKey, userSession.OdsCode, _providerId, request);

            
            return await SendRequestAndParseResponse<CancelledAppointmentResponse, VisionRequest<CancelAppointmentRequest>>(visionRequest);
        }

        public async Task<VisionApiObjectResponse<ServiceContentRegisterResponse>> PostLinkAccount(string odsCode,
            PatientIm1ConnectionRequest request, string dob)
        {
            IVisionServiceDefinition visionServiceDefinition = new RegisterServiceDefinition();

            var visionRequest = new VisionRequest<Object>(visionServiceDefinition.Name, visionServiceDefinition.Version,
                odsCode, _providerId, request.AccountId, request.LinkageKey, request.Surname, dob);

            return await SendRequestAndParseResponse<ServiceContentRegisterResponse, VisionRequest<Object>>(
                visionRequest);
        }

        private async Task<VisionApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse, T>(T request)
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
            _logger.LogInformation($"{nameof(VisionClient)} sending request");

            var responseMessage = await _httpClient.Client.SendAsync(request);
            
            var response = new VisionApiObjectResponse<TResponse>(responseMessage.StatusCode);

            if (!response.HasSuccessResponse)
            {
                _logger.LogError($"VISION request failed with status code {responseMessage.StatusCode}");
                return response;
            }

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

        private T Deserializer<T>(string request)
        {
            T instance;

            var xmlSerializer = new XmlSerializer(typeof(T));
            
            xmlSerializer.UnknownNode+= new   
                XmlNodeEventHandler(serializer_UnknownNode);  
            xmlSerializer.UnknownAttribute+= new   
                XmlAttributeEventHandler(serializer_UnknownAttribute);
            
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
                           || bool.FalseString.Equals(
                               RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Successful,
                               StringComparison.OrdinalIgnoreCase);
                }
            }

            public bool HasSuccessResponse => !HasErrorResponse && StatusCode.IsSuccessStatusCode();

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

            public bool IsInvalidRequestError =>
                (RawResponse?.Body?.Fault?.Detail?.VisionFault?.Error?.Category ?? "").Contains("INVALID_REQUEST",
                    StringComparison.Ordinal);

            public bool IsInvalidUserCredentialsError => "-30".Equals(
                RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code, StringComparison.Ordinal);

            public bool IsInvalidSecurityHeaderError => (RawResponse?.Body?.Fault?.FaultCode ?? "").Contains("InvalidSecurity", StringComparison.Ordinal);

            public bool IsAccountLockedError => VisionApiErrorCodes.AccountLocked.Equals(
                RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code, StringComparison.Ordinal);

            public bool IsAlreadyRegisteredError => VisionApiErrorCodes.AccountAlreadyRegistered.Equals(
                RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code, StringComparison.Ordinal);

            public bool IsInvalidDetailsError => VisionApiErrorCodes.InvalidDetails.Equals(
                RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code, StringComparison.Ordinal);
            
            public bool IsInvalidParameterError => VisionApiErrorCodes.InvalidParameter.Equals(
                RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code, StringComparison.Ordinal);

            public bool IsUnknownError =>
                "-100".Equals(RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code,
                    StringComparison.Ordinal);
            
            public bool IsAccessDeniedError =>
                "-35".Equals(RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code,
                    StringComparison.Ordinal);
            
            public bool IsAppointmentSlotNotBookedToCurrentUserError =>
                "-100".Equals(RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code,
                    StringComparison.Ordinal);
            
            public bool IsAppointmentSlotNotFoundError =>
                "-21".Equals(RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome?.Error?.Code,
                    StringComparison.Ordinal);

            private bool StatusCodeIndicatesSuccess => StatusCode == HttpStatusCode.OK;
        }
        
        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
           _logger.LogDebug("Unknown node while deserialising Vision response - Node name: {nodeName}",  e.Name);
        }

        private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)  
        {
            _logger.LogDebug("Unknown attribute while deserialising Vision response - Attribute name: {attrName}", e.Attr.Name);
        }
    }
}