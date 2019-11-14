using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.VisionServiceDefinition;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    /// <summary>
    /// Patient facing services client.
    /// </summary>
    public class VisionPFSClient : IVisionPFSClient
    {
        private readonly VisionPFSHttpClient _httpClient;
        private readonly IXmlResponseParser _responseParser;
        private readonly Uri _targetUri;
        private readonly string _requestUsername;
        private readonly string _providerId;
        private readonly X509Certificate2 _certificate;
        private readonly ILogger<VisionPFSClient> _logger;
        private readonly IEnvelopeService _envelopeService;
        private readonly VisionConfigurationSettings _settings;

        private const string MediaType = "text/xml";

        public VisionPFSClient(
            ILoggerFactory loggerFactory,
            ICertificateService certificateService,
            IEnvelopeService envelopeService,
            VisionPFSHttpClient httpClient,
            IXmlResponseParser responseParser,
            VisionConfigurationSettings settings)
        {
            _settings = settings;
            _targetUri = _settings.ApiUrl;
            _requestUsername = _settings.RequestUsername;
            _providerId = _settings.ApplicationProviderId;
            _logger = loggerFactory.CreateLogger<VisionPFSClient>();
            _envelopeService = envelopeService;
            _httpClient = httpClient;
            _responseParser = responseParser;
            _certificate =
                certificateService.GetCertificate(_settings.CertificatePath, _settings.CertificatePassphrase);

            _settings.Validate();
        }

        public async Task<VisionApiObjectResponse<VisionDemographicsResponse>> GetDemographics(
            VisionUserSession visionUserSession,
            DemographicsRequest requestContent)
        {
            var visionServiceDefinition = new DemographicsServiceDefinition();

            return await Post<VisionDemographicsResponse, DemographicsRequest>(
                visionServiceDefinition,
                visionUserSession,
                requestContent);
        }

        public async Task<VisionApiObjectResponse<VisionPatientDataResponse>> GetPatientData(
            VisionUserSession visionUserSession, PatientDataRequest requestContent)
        {
            var visionServiceDefinition = new PatientDataServiceDefinition();

            return await Post<VisionPatientDataResponse, PatientDataRequest>(
                visionServiceDefinition,
                visionUserSession,
                requestContent);
        }

        public async Task<VisionApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(
            VisionConnectionToken token, string odsCode)
        {
            new ValidateAndLog(_logger).IsNotNullOrWhitespace(odsCode, nameof(odsCode), ThrowError)
                .IsNotNull(token, nameof(token), ThrowError)
                .IsNotNullOrWhitespace(token?.RosuAccountId, nameof(VisionConnectionToken.RosuAccountId), ThrowError)
                .IsNotNullOrWhitespace(token?.ApiKey, nameof(VisionConnectionToken.ApiKey), ThrowError)
                .IsValid();

            var visionServiceDefinition = new ConfigurationServiceDefinition();

            var visionRequest = new VisionRequest<object>(
                visionServiceDefinition.Name,
                visionServiceDefinition.Version,
                token.RosuAccountId,
                token.ApiKey,
                odsCode,
                _providerId,
                null);

            return await Post<PatientConfigurationResponse, object>(visionRequest);
        }

        public async Task<VisionApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(VisionUserSession userSession)
        {
            var visionServiceDefinition = new ConfigurationServiceDefinition();

            var visionRequest = new VisionRequest<object>(
                visionServiceDefinition.Name,
                visionServiceDefinition.Version,
                userSession.RosuAccountId,
                userSession.ApiKey,
                userSession.OdsCode,
                _providerId,
                null);

            return await Post<PatientConfigurationResponse, object>(visionRequest);
        }

        public async Task<VisionApiObjectResponse<EligibleRepeatsResponse>> GetEligibleRepeats(
            VisionUserSession session)
        {
            var visionServiceDefinition = new EligibleRepeatsServiceDefinition();

            var requestContent = new CoursesRequest { PatientId = session.PatientId };

            return await Post<EligibleRepeatsResponse, CoursesRequest>(
                visionServiceDefinition,
                session,
                requestContent);
        }

        public async Task<VisionApiObjectResponse<PrescriptionHistoryResponse>> GetHistoricPrescriptions(
            VisionUserSession userSession, PrescriptionRequest prescriptionRequest)
        {
            var visionServiceDefinition = new PrescriptionHistoryServiceDefinition();

            return await Post<PrescriptionHistoryResponse, PrescriptionRequest>(
                visionServiceDefinition,
                userSession,
                prescriptionRequest);
        }

        public async Task<VisionApiObjectResponse<OrderNewPrescriptionResponse>> OrderNewPrescription(
            VisionUserSession userSession, OrderNewPrescriptionRequest newPrescriptionRequest)
        {
            var visionServiceDefinition = new NewPrescriptionServiceDefinition();

            return await Post<OrderNewPrescriptionResponse, OrderNewPrescriptionRequest>(
                visionServiceDefinition,
                userSession,
                newPrescriptionRequest);
        }

        public async Task<VisionApiObjectResponse<BookedAppointmentsResponse>> GetExistingAppointments(
            VisionUserSession userSession
        )
        {
            var visionServiceDefinition = new GetExistingAppointmentsServiceDefinition();

            return await Post<BookedAppointmentsResponse, PatientId>(
                visionServiceDefinition,
                userSession,
                new PatientId { Id = userSession.PatientId });
        }

        public async Task<VisionApiObjectResponse<AvailableAppointmentsResponse>> GetAvailableAppointments(
            VisionUserSession visionUserSession, AppointmentSlotsDateRange dateRange)
        {
            var visionServiceDefinition = new GetAvailableAppointmentsServiceDefinition();

            var request = new AvailableAppointmentsRequest
            {
                PatientId = visionUserSession.PatientId,

                Page = new Page
                {
                    Number = 1,
                    SlotsPerPage = _settings.VisionAppointmentSlotsRequestCount
                },
                Locations = visionUserSession.LocationIds,
                DateRange = new DateRange
                {
                    From = dateRange.FromDate.Date,
                    To = dateRange.ToDate.Date.AddDays(-1)
                }
            };

            return await Post<AvailableAppointmentsResponse, AvailableAppointmentsRequest>(
                visionServiceDefinition, visionUserSession, request);
        }

        public async Task<VisionApiObjectResponse<BookAppointmentResponse>> BookAppointment(
            VisionUserSession userSession,
            BookAppointmentRequest bookAppointmentRequest
        )
        {
            var visionServiceDefinition = new BookAppointmentServiceDefinition();

            return await Post<BookAppointmentResponse, BookAppointmentRequest>(
                visionServiceDefinition, userSession, bookAppointmentRequest);
        }

        public async Task<VisionApiObjectResponse<CancelledAppointmentResponse>> CancelAppointment(
            VisionUserSession userSession,
            CancelAppointmentRequest request
        )
        {
            var visionServiceDefinition = new CancelAppointmentServiceDefinition();

            return await Post<CancelledAppointmentResponse, CancelAppointmentRequest>(
                visionServiceDefinition, userSession, request);
        }

        public async Task<VisionApiObjectResponse<ServiceContentRegisterResponse>> PostLinkAccount(string odsCode,
            PatientIm1ConnectionRequest request, string dob)
        {
            var visionServiceDefinition = new RegisterServiceDefinition();

            var visionRequest = new VisionRequest<object>(
                visionServiceDefinition.Name,
                visionServiceDefinition.Version,
                odsCode,
                _providerId,
                request.AccountId,
                request.LinkageKey,
                request.Surname,
                dob);

            return await Post<ServiceContentRegisterResponse, object>(visionRequest);
        }

        public async Task<VisionApiObjectResponse<TResponse>> Post<TResponse, TRequest>(
            IVisionServiceDefinition visionServiceDefinition,
            VisionUserSession session,
            TRequest requestContent)
        {
            var request = new VisionRequest<TRequest>(
                visionServiceDefinition.Name,
                visionServiceDefinition.Version,
                session.RosuAccountId,
                session.ApiKey,
                session.OdsCode,
                _providerId,
                requestContent);

            return await Post<TResponse, TRequest>(request);
        }

        private async Task<VisionApiObjectResponse<TResponse>> Post<TResponse, TRequest>(VisionRequest<TRequest> request)
        {
            var httpRequest = BuildHttpRequest(request);
            var response = await TransmitAsync<TResponse>(httpRequest);

            return response;
        }

        private HttpRequestMessage BuildHttpRequest<T>(VisionRequest<T> request)
        {
            var envelope = _envelopeService.BuildEnvelope(_certificate, request, _requestUsername);
            var httpRequest =
                new HttpRequestMessage(HttpMethod.Post, _targetUri)
                {
                    Content = new StringContent(envelope, Encoding.UTF8, MediaType)
                };

            httpRequest.Headers.Add(Constants.VisionConstants.RequestIdentifierHeader, request?.ServiceDefinition?.Name);

            return httpRequest;
        }

        private async Task<VisionApiObjectResponse<TResponse>> TransmitAsync<TResponse>(HttpRequestMessage request)
        {
            var thirdPartyEnvelope = await SendRequestAndParseResponse<TResponse>(request);
            return thirdPartyEnvelope;
        }

        private async Task<VisionApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>
            (HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new VisionApiObjectResponse<TResponse>(responseMessage.StatusCode);
            return await response.Parse(responseMessage, _responseParser, _logger);
        }

        public class VisionApiObjectResponse<TBody> : ApiResponse
        {
            // Note
            // We don't know whether Vision always populates certain properties when there is an error.
            // So there are a lot of null checks below using the null conditional operator.

            public VisionApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            { }

            public async Task<VisionApiObjectResponse<TBody>> Parse(
                HttpResponseMessage responseMessage,
                IXmlResponseParser responseParser,
                ILogger logger)
            {
                var stringResponse = await GetStringResponse(responseMessage, logger);
                return string.IsNullOrEmpty(stringResponse)
                    ? this : ParseResponse(responseParser, logger, stringResponse, responseMessage);
            }

            public VisionResponseEnvelope<TBody> RawResponse { get; set; }

            public string UnparsableResultMessage { get; set; }

            public TBody Body => RawResponse.Body.VisionResponse.ServiceContent;

            public bool HasErrorResponse => !StatusCode.IsSuccessStatusCode()
                                            || FaultExists
                                            || OutcomeUnsuccessful
                                            || UnparsableResultMessage != null;

            private Fault Fault => RawResponse?.Body?.Fault;
            private Outcome Outcome => RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome;

            private bool FaultExists => Fault != null;

            private bool OutcomeUnsuccessful => bool.FalseString.Equals(
                Outcome?.Successful,
                StringComparison.OrdinalIgnoreCase);

            public override bool HasSuccessResponse => !HasErrorResponse;

            public override bool HasBadRequestResponse => StatusCode.IsBadRequestCode();

            public override string ErrorForLogging => $"fault: {JsonConvert.SerializeObject(Fault)}, " +
                                             $"error: {JsonConvert.SerializeObject(Outcome?.Error)}";

            protected override bool FormatResponseIfUnsuccessful => true;

            public bool IsInvalidRequestError =>
                (Fault?.Detail?.VisionFault?.Error?.Category ?? "").Contains("INVALID_REQUEST",
                    StringComparison.Ordinal);

            public bool IsInvalidSecurityHeaderError =>
                (Fault?.FaultCode ?? "").Contains("InvalidSecurity", StringComparison.Ordinal);

            public bool IsInvalidUserCredentialsError =>
                HasVisionApiErrorCode(VisionApiErrorCodes.InvalidUserCredentials);

            public bool IsUnknownError => HasVisionApiErrorCode(VisionApiErrorCodes.UnknownError);

            public bool IsAccessDeniedError => HasVisionApiErrorCode(VisionApiErrorCodes.AccessDenied);

            public bool IsAppointmentSlotNotBookedToCurrentUserError =>
                HasVisionApiErrorCode(VisionApiErrorCodes.AppointmentSlotNotBookedToCurrentUser);

            public bool IsAppointmentSlotNotFoundError =>
                HasVisionApiErrorCode(VisionApiErrorCodes.AppointmentSlotNotFound);

            public bool IsAppointmentSlotAlreadyBookedError =>
                HasVisionApiErrorCode(VisionApiErrorCodes.AppointmentSlotAlreadyBooked);

            public bool IsAppointmentBookingLimitReachedError =>
                HasVisionApiErrorCode(VisionApiErrorCodes.AppointmentBookingLimitReached);

            private bool HasVisionApiErrorCode(string visionErrorCode)
            {
                return visionErrorCode.Equals(Outcome?.Error?.Code, StringComparison.Ordinal);
            }
            public string ErrorCode => Outcome?.Error?.Code;
            public string ErrorMessage => Outcome?.Error?.Description;

            private VisionApiObjectResponse<TBody> ParseResponse(
                IResponseParser responseParser,
                ILogger logger,
                string stringResponse,
                HttpResponseMessage responseMessage)
            {
                try
                {
                    var parseSuccess = responseParser.TryParseBody<VisionResponseEnvelope<TBody>>(stringResponse, 
                        responseMessage, 
                        out var responseObject);
                    RawResponse = responseObject;

                    if (!parseSuccess)
                    {
                        logger.LogError($"Vision Error Response could not be parsed: : {stringResponse}");
                        UnparsableResultMessage = stringResponse;
                    }
                }
                catch (InvalidOperationException e)
                {
                    logger.LogError(e, $"Vision Error Response could not be parsed: : {stringResponse}" );
                    UnparsableResultMessage = stringResponse;
                }

                return this;
            }
        }
    }
}
