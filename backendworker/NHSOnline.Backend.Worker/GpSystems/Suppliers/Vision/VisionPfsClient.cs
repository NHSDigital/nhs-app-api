using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.VisionServiceDefinition;
using NHSOnline.Backend.Worker.ResponseParsers;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Certificate;
using NHSOnline.Backend.Worker.Support;

using static NHSOnline.Backend.Worker.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
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
        private readonly ConfigurationSettings _settings;

        private const string MediaType = "text/xml";

        public VisionPFSClient(IVisionPFSConfig visionConfig,
            ILoggerFactory loggerFactory,
            ICertificateService certificateService,
            IEnvelopeService envelopeService,
            VisionPFSHttpClient httpClient,
            IXmlResponseParser responseParser,
            IOptions<ConfigurationSettings> settings)
        {
            _targetUri = visionConfig.ApiUrl;
            _requestUsername = visionConfig.RequestUsername;
            _providerId = visionConfig.ApplicationProviderId;
            _logger = loggerFactory.CreateLogger<VisionPFSClient>();
            _envelopeService = envelopeService;
            _httpClient = httpClient;
            _responseParser = responseParser;
            _certificate =
                certificateService.GetCertificate(visionConfig.CertificatePath, visionConfig.CertificatePassphrase);
            _settings = settings.Value;
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
            var visionServiceDefinition = new GetAvailableAppointmenServiceDefinition();

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

            public TBody Body => RawResponse.Body.VisionResponse.ServiceContent;

            public bool HasErrorResponse => !StatusCode.IsSuccessStatusCode()
                                            || FaultExists
                                            || OutcomeUnsuccessful;

            private Fault Fault => RawResponse?.Body?.Fault;
            private Outcome Outcome => RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome;

            private bool FaultExists => Fault != null;

            private bool OutcomeUnsuccessful => bool.FalseString.Equals(
                Outcome?.Successful,
                StringComparison.OrdinalIgnoreCase);

            public override bool HasSuccessResponse => !HasErrorResponse;


            public override string ErrorForLogging => $"fault: {JsonConvert.SerializeObject(Fault)}, " +
                                             $"error: {JsonConvert.SerializeObject(Outcome?.Error)}";

            protected override bool FormatResponseIfUnsuccessful => false;

            public bool IsInvalidRequestError =>
                (Fault?.Detail?.VisionFault?.Error?.Category ?? "").Contains("INVALID_REQUEST",
                    StringComparison.Ordinal);

            public bool IsInvalidSecurityHeaderError =>
                (Fault?.FaultCode ?? "").Contains("InvalidSecurity", StringComparison.Ordinal);

            public bool IsInvalidUserCredentialsError =>
                HasVisionApiErrorCode(VisionApiErrorCodes.InvalidUserCredentials);

            public bool IsAccountLockedError => HasVisionApiErrorCode(VisionApiErrorCodes.AccountLocked);

            public bool IsAlreadyRegisteredError => HasVisionApiErrorCode(VisionApiErrorCodes.AccountAlreadyRegistered);

            public bool IsInvalidDetailsError => HasVisionApiErrorCode(VisionApiErrorCodes.InvalidDetails);

            public bool IsInvalidParameterError => HasVisionApiErrorCode(VisionApiErrorCodes.InvalidParameter);

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

            private VisionApiObjectResponse<TBody> ParseResponse(
                IResponseParser responseParser,
                ILogger logger,
                string stringResponse,
                HttpResponseMessage responseMessage)
            {
                try
                {
                    var responseObject = responseParser.ParseBody<VisionResponseEnvelope<TBody>>(stringResponse, responseMessage);
                    RawResponse = responseObject;
                }
                catch (FormatException e)
                {
                    logger.LogError(e, "An error occured while parsing the response");
                    return new VisionApiObjectResponse<TBody>(HttpStatusCode.InternalServerError);
                }

                return this;
            }
        }
    }
}
