using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisClient : IEmisClient
    {
        private readonly TimeZoneConverter _localTimeZoneConverter;

        public const string HeaderEndUserSessionId = "X-API-EndUserSessionId";
        public const string HeaderSessionId = "X-API-SessionId";

        private const string MeApplicationsPath = "me/applications";
        private const string SessionsEndUserSessionPath = "sessions/endusersession";
        private const string SessionsPath = "sessions";
        private const string DemographicsPath = "demographics?userPatientLinkToken={0}";
        private const string PatientRecordPath = "record?userPatientLinkToken={0}&itemType={1}";

        private const string AppointmentSlotsMetaPath =
            "appointmentslots/meta?userPatientLinkToken={0}&sessionStartDate={1}&sessionEndDate={2}";

        private const string AppointmentSlotsPath =
            "appointmentslots?userPatientLinkToken={0}&fromDateTime={1}&toDateTime={2}";

        private const string PracticeSettingsPath = "practices/{0}/settings";
        private const string PrescriptionsPath = "prescriptionrequests?userPatientLinkToken={0}";
        private const string PrescriptionsPostPath = "prescriptionrequests";
        private const string CoursesPath = "courses?userPatientLinkToken={0}";
        private const string AppointmentsPath = "appointments";
        private const string MeVerificationsPath = "me/verifications";
        private const string UsersNhsPath = "users/nhs";

        private readonly EmisHttpClient _httpClient;
        private readonly IOptions<ConfigurationSettings> _settings;

        private readonly ILogger<EmisClient> _logger;
        private readonly IJsonResponseParser _responseParser;

        public EmisClient(EmisHttpClient httpClient,
            TimeZoneConverter localTimeZoneConverter,
            ILogger<EmisClient> logger,
            IJsonResponseParser responseParser,
            IOptions<ConfigurationSettings> settings)
        {
            _logger = logger;
            _localTimeZoneConverter = localTimeZoneConverter;
            _httpClient = httpClient;
            _settings = settings;
            _responseParser = responseParser;
        }

        public async Task<EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost()
        {
            return await Post<SessionsEndUserSessionPostResponse>(
                SessionsEndUserSessionPath, customTimeout: _settings.Value.EmisExtendedHttpTimeoutSeconds);
        }

        public async Task<EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(string endUserSessionId,
            SessionsPostRequest model)
        {
            return await Post<SessionsPostRequest, SessionsPostResponse>(model, SessionsPath, endUserSessionId);
        }

        public async Task<EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(string userPatientLinkToken,
            string responseSessionId,
            string endUserSessionId)
        {
            _logger.LogInformation("EMIS: Fetching patient demographics");
            var path = string.Format(CultureInfo.InvariantCulture, DemographicsPath, userPatientLinkToken);

            return await Get<DemographicsGetResponse>(path, endUserSessionId, responseSessionId);
        }

        public async Task<EmisApiObjectResponse<MedicationRootObject>> MedicalRecordGet(string userPatientLinkToken,
            string responseSessionId, string endUserSessionId,
            RecordType recordType)
        {

            _logger.LogInformation("EMIS: Fetching patient medical record - {0}", recordType.ToString());
            var path = string.Format(CultureInfo.InvariantCulture, PatientRecordPath, userPatientLinkToken, recordType.ToString());

            return await Get<MedicationRootObject>(path, endUserSessionId, responseSessionId);
        }

        public async Task<EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(string endUserSessionId,
            MeApplicationsPostRequest model)
        {
            return await Post<MeApplicationsPostRequest, MeApplicationsPostResponse>(model, MeApplicationsPath,
                endUserSessionId);
        }

        public async Task<EmisApiObjectResponse<PrescriptionRequestsGetResponse>> PrescriptionsGet(
            string userPatientLinkToken, string responseSessionId,
            string endUserSessionId, DateTimeOffset? fromDateTime, DateTimeOffset? toDateTime)
        {
            var path = string.Format(CultureInfo.InvariantCulture, PrescriptionsPath, userPatientLinkToken);

            if (fromDateTime.HasValue)
            {
                path += $"&filterFromDate={EncodeDateTimeOffsetToIso(fromDateTime.Value)}";
            }

            if (toDateTime.HasValue)
            {
                path += $"&filterToDate={EncodeDateTimeOffsetToIso(toDateTime.Value)}";
            }

            return await Get<PrescriptionRequestsGetResponse>(path, endUserSessionId, responseSessionId);
        }

        public async Task<EmisApiObjectResponse<PrescriptionRequestPostResponse>> PrescriptionsPost(
            string responseSessionId,
            string endUserSessionId,
            PrescriptionRequestsPost model)
        {
            return await Post<PrescriptionRequestsPost, PrescriptionRequestPostResponse>(model, PrescriptionsPostPath,
                endUserSessionId, responseSessionId);
        }

        public async Task<EmisApiObjectResponse<AppointmentSlotsGetResponse>> AppointmentSlotsGet(
            EmisHeaderParameters headerParameters, SlotsGetQueryParameters queryParameters)
        {
            var fromDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.FromDateTime).ToString("s", CultureInfo.InvariantCulture);
            var toDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.ToDateTime).ToString("s", CultureInfo.InvariantCulture);

            var path = string.Format(CultureInfo.InvariantCulture, 
                AppointmentSlotsPath,
                queryParameters.UserPatientLinkToken,
                fromDateTime,
                toDateTime);

            var response = await Get<AppointmentSlotsGetResponse>(path, headerParameters.EndUserSessionId, headerParameters.SessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<PracticeSettingsGetResponse>> PracticeSettingsGet(
            EmisHeaderParameters headerParameters, string practiceCode)
        {
            var path = string.Format(CultureInfo.InvariantCulture,
                                     PracticeSettingsPath,
                                     practiceCode);

            var response = await Get<PracticeSettingsGetResponse>(path, headerParameters.EndUserSessionId, headerParameters.SessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> AppointmentSlotsMetadataGet(
            EmisHeaderParameters headerParameters, SlotsMetadataGetQueryParameters queryParameters)
        {
            var fromDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.SessionStartDate).ToString("s", CultureInfo.InvariantCulture);
            var toDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.SessionEndDate).ToString("s", CultureInfo.InvariantCulture);

            var path = string.Format(CultureInfo.InvariantCulture, 
                AppointmentSlotsMetaPath,
                queryParameters.UserPatientLinkToken,
                fromDateTime,
                toDateTime);

            return await Get<AppointmentSlotsMetadataGetResponse>(path, headerParameters.EndUserSessionId,
                headerParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<CoursesGetResponse>> CoursesGet(string userPatientLinkToken,
            string responseSessionId, string endUserSessionId)
        {
            var path = string.Format(CultureInfo.InvariantCulture, CoursesPath, userPatientLinkToken);

            return await Get<CoursesGetResponse>(path, endUserSessionId, responseSessionId);
        }

        public async Task<EmisApiObjectResponse<BookAppointmentSlotPostResponse>> AppointmentsPost(
            EmisHeaderParameters headerParameters,
            BookAppointmentSlotPostRequest postRequest)
        {
            return await Post<BookAppointmentSlotPostRequest, BookAppointmentSlotPostResponse>(postRequest,
                AppointmentsPath, headerParameters.EndUserSessionId, headerParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<AppointmentsGetResponse>> AppointmentsGet(
            EmisHeaderParameters headerParameters, string userPatientLinkToken)
        {
            var queryParams = string.Format(CultureInfo.InvariantCulture,
                "?UserPatientLinkToken={0}&FetchPreviousAppointments=true&PreviousAppointmentsFromDate={1}", 
                userPatientLinkToken,
                DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));


            var path = $"{AppointmentsPath}{queryParams}";

            var response =
                await Get<AppointmentsGetResponse>(path, headerParameters.EndUserSessionId, headerParameters.SessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<CancelAppointmentDeleteResponse>> AppointmentsDelete(
            EmisHeaderParameters headerParameters, CancelAppointmentDeleteRequest deleteRequest)
        {
            return await Delete<CancelAppointmentDeleteRequest, CancelAppointmentDeleteResponse>(
                deleteRequest, AppointmentsPath, headerParameters.EndUserSessionId, headerParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<AddVerificationResponse>> VerificationPost(
            EmisHeaderParameters headerParameters, AddVerificationRequest addVerificationRequest)
        {
            return await Post<AddVerificationRequest, AddVerificationResponse>(
                addVerificationRequest, MeVerificationsPath,
                headerParameters.EndUserSessionId,
                headerParameters.SessionId,
                _settings.Value.EmisExtendedHttpTimeoutSeconds);
        }

        public async Task<EmisApiObjectResponse<AddNhsUserResponse>> NhsUserPost(EmisHeaderParameters headerParameters, AddNhsUserRequest addNhsUserRequest)
        {
            return await Post<AddNhsUserRequest, AddNhsUserResponse>(
                addNhsUserRequest, UsersNhsPath,
                headerParameters.EndUserSessionId,
                headerParameters.SessionId,
                _settings.Value.EmisExtendedHttpTimeoutSeconds);
        }
        
        private async Task<EmisApiObjectResponse<TResponse>> Delete<TRequest, TResponse>(TRequest model, string path,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Delete, path, endUserSessionId, sessionId);

            var body = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Get<TResponse>(string path,
            string endUserSessionId = null, string sessionId = null, int? customTimeout = null)
        {
            var request = BuildEmisRequest(HttpMethod.Get, path, endUserSessionId, sessionId);

            request.CheckAndSetCustomTimeout(customTimeout);

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Post<TRequest, TResponse>(TRequest model, string path,
            string endUserSessionId = null, string sessionId = null, int? customTimeout = null)
        {
            var request = BuildEmisRequest(HttpMethod.Post, path, endUserSessionId, sessionId);

            request.CheckAndSetCustomTimeout(customTimeout);

            var body = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Post<TResponse>(string path,
            string endUserSessionId = null, string sessionId = null, int? customTimeout = null)
        {
            var request = BuildEmisRequest(HttpMethod.Post, path, endUserSessionId, sessionId);

            request.CheckAndSetCustomTimeout(customTimeout);

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private static HttpRequestMessage BuildEmisRequest(HttpMethod httpMethod, string path,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = new HttpRequestMessage(httpMethod, path);

            if (!string.IsNullOrEmpty(endUserSessionId))
            {
                request.Headers.Add(HeaderEndUserSessionId, new[] { endUserSessionId });
            }

            if (!string.IsNullOrEmpty(sessionId))
            {
                request.Headers.Add(HeaderSessionId, new[] { sessionId });
            }

            return request;
        }

        private async Task<EmisApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new EmisApiObjectResponse<TResponse>(responseMessage.StatusCode);
            await response.Parse(responseMessage, _responseParser, _logger);

            if (response.IsUnauthorisedResponse)
            {
                _logger.LogInformation("Unauthorised EMIS response");
                throw new UnauthorisedGpSystemHttpRequestException();
            }

            return response;
        }

        public abstract class EmisApiResponse: ApiResponse
        {
            protected EmisApiResponse(HttpStatusCode statusCode) :base(statusCode)
            {}

            public string RawResponse { get; protected internal set; }
            public StandardErrorResponse StandardErrorResponse { get; set; }
            public ExceptionErrorResponse ExceptionErrorResponse { get; set; }
            public BadRequestErrorResponse ErrorResponseBadRequest { get; set; }
            public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

            internal bool IsUnauthorisedResponse =>
                StatusCode == HttpStatusCode.Unauthorized ||
                string.Equals(RawResponse, EmisApiErrorMessages.EmisService_UnauthorisedRequest, StringComparison.Ordinal);

            public bool HasInternalErrorCode(EmisApiErrorCode code)
            {
                return StandardErrorResponse?.InternalResponseCode == (int) code;
            }

            public bool HasStatusCodeAndErrorCode(HttpStatusCode statusCode, EmisApiErrorCode emisApiErrorCode)
            {
                return (StatusCode == statusCode) && HasInternalErrorCode(emisApiErrorCode);
            }

            public bool HasExceptionWithMessage(string message)
            {
                return ExceptionErrorResponse?.Exceptions?.Any(x =>
                           string.Equals(x.Message, message, StringComparison.Ordinal)) ?? false;
            }


            public bool HasExceptionWithMessageContaining(string message)
            {
                return ExceptionErrorResponse?.Exceptions?.Any(x =>
                           x.Message.Contains(message, StringComparison.Ordinal)) ?? false;
            }

            public bool HasExceptionWithAnyMessage(string[] messages)
            {
                return ExceptionErrorResponse?.Exceptions?.Any(x => messages.Contains(x.Message)) ?? false;
            }

            public bool HasForbiddenResponse()
            {
                if (StatusCode == HttpStatusCode.Forbidden)
                {
                    return true;
                }

                return HasExceptionWithMessageContaining(
                    EmisApiErrorMessages.EmisService_NotEnabledForUser);
            }

            public string GetExceptionLogMessage(string methodCall)
            {
                var baseMessage = $"Emis {methodCall} returned with status code {StatusCode}";

                if (string.IsNullOrEmpty(ExceptionErrorResponse?.Exceptions?.FirstOrDefault()?.Message))
                {
                    return baseMessage + " and no error message";
                }

                return baseMessage
                       + " and error message "
                       + $"{ExceptionErrorResponse?.Exceptions.First().Message}";
            }

            public override string  ErrorForLogging => $"Error Code: '{StatusCode}'. " +
                                             $"Error Message:'{StandardErrorResponse?.Message}'. ";

        }

        public class EmisApiObjectResponse<TBody> : EmisApiResponse
        {
            public TBody Body { get; set; }

            public EmisApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {}

            public async Task Parse(
                HttpResponseMessage responseMessage,
                IJsonResponseParser responseParser,
                ILogger logger)
            {
                var stringResponse = await GetStringResponse(responseMessage, logger);
                RawResponse = stringResponse;
                if (!string.IsNullOrEmpty(stringResponse))
                {
                    ParseResponse(responseParser, stringResponse, responseMessage);
                }
            }

            private EmisApiObjectResponse<TBody> ParseResponse(
                IResponseParser responseParser, 
                string stringResponse, 
                HttpResponseMessage responseMessage)
            {
                Body = responseParser.ParseBody<TBody>(stringResponse, responseMessage);
                StandardErrorResponse =
                    responseParser.ParseBadRequest<StandardErrorResponse>(stringResponse, responseMessage);
                ErrorResponseBadRequest =
                    responseParser.ParseBadRequest<BadRequestErrorResponse>(stringResponse, responseMessage);
                ExceptionErrorResponse =
                    responseParser.ParseError<ExceptionErrorResponse>(stringResponse, responseMessage, HttpStatusCode.BadRequest);
                return this;
            }

            protected override bool FormatResponseIfUnsuccessful => true;
        }

        private static string EncodeDateTimeOffsetToIso(DateTimeOffset dateTimeOffset)
        {
            return HttpUtility.UrlEncode(dateTimeOffset.ToString("O", CultureInfo.InvariantCulture));
        }
    }
}
