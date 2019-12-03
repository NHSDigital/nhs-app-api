using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisClient : IEmisClient
    {
        private readonly TimeZoneConverter _localTimeZoneConverter;

        public const string HeaderEndUserSessionId = "X-API-EndUserSessionId";
        public const string HeaderSessionId = "X-API-SessionId";

        private const string MeApplicationsPath = "me/applications";
        private const string MeSettingsPath = "me/settings?userPatientLinkToken={0}";

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
        private const string DocumentPath = "documents/{0}?userPatientLinkToken={1}";
        private const string AppointmentsPath = "appointments";
        private const string MeVerificationsPath = "me/verifications";
        private const string UsersNhsPath = "users/nhs";
        private const string GetMessagesPath = "messages?userPatientLinkToken={0}";

        private readonly EmisHttpClient _httpClient;
        private readonly EmisConfigurationSettings _settings;

        private readonly ILogger<EmisClient> _logger;
        private readonly IJsonResponseParser _responseParser;

        public EmisClient(EmisHttpClient httpClient,
            TimeZoneConverter localTimeZoneConverter,
            ILogger<EmisClient> logger,
            IJsonResponseParser responseParser,
            EmisConfigurationSettings settings)
        {
            _logger = logger;
            _localTimeZoneConverter = localTimeZoneConverter;
            _httpClient = httpClient;
            _settings = settings;
            _responseParser = responseParser;

            _settings.Validate();
        }

        public async Task<EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost()
        {
            return await Post<SessionsEndUserSessionPostResponse>(
                SessionsEndUserSessionPath, customTimeout: _settings.EmisExtendedHttpTimeoutSeconds);
        }

        public async Task<EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(string endUserSessionId,
            SessionsPostRequest model)
        {
            return await Post<SessionsPostRequest, SessionsPostResponse>(model, SessionsPath, endUserSessionId);
        }

        public async Task<EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(EmisRequestParameters emisRequestParameters)

        {
            _logger.LogInformation("EMIS: Fetching patient demographics");
            var path = string.Format(CultureInfo.InvariantCulture, DemographicsPath, emisRequestParameters.UserPatientLinkToken);

            return await Get<DemographicsGetResponse>
                (path, emisRequestParameters.EndUserSessionId, emisRequestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<MedicationRootObject>> MedicalRecordGet(EmisRequestParameters emisRequestParameters, RecordType recordType)
        {
            _logger.LogInformation("EMIS: Fetching patient medical record - {0}", recordType.ToString());
            var path = string.Format(CultureInfo.InvariantCulture,
                PatientRecordPath, emisRequestParameters.UserPatientLinkToken, recordType.ToString());

            return await Get<MedicationRootObject>(path, emisRequestParameters.EndUserSessionId, emisRequestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<IndividualDocument>> MedicalDocumentGet(string userPatientLinkToken,
            string responseSessionId, string documentGuid, string endUserSessionId)
        {

            _logger.LogInformation("EMIS: Fetching patient document - {0}", documentGuid);
              var path = string.Format(CultureInfo.InvariantCulture,
                DocumentPath,
                documentGuid,
                userPatientLinkToken);

            return await Get<IndividualDocument>(path, endUserSessionId, responseSessionId);
        }

        public async Task<EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(string endUserSessionId,
            MeApplicationsPostRequest model)
        {
            return await Post<MeApplicationsPostRequest, MeApplicationsPostResponse>(model, MeApplicationsPath,
                endUserSessionId, customTimeout: _settings.EmisExtendedHttpTimeoutSeconds);
        }

        public async Task<EmisApiObjectResponse<PrescriptionRequestsGetResponse>> PrescriptionsGet(
            EmisRequestParameters emisRequestParameters, DateTimeOffset? fromDateTime, DateTimeOffset? toDateTime)
        {
            var path = string.Format(CultureInfo.InvariantCulture, PrescriptionsPath, emisRequestParameters.UserPatientLinkToken);

            if (fromDateTime.HasValue)
            {
                path += $"&filterFromDate={EncodeDateTimeOffsetToIso(fromDateTime.Value)}";
            }

            if (toDateTime.HasValue)
            {
                path += $"&filterToDate={EncodeDateTimeOffsetToIso(toDateTime.Value)}";
            }

            return await Get<PrescriptionRequestsGetResponse>(path, emisRequestParameters.EndUserSessionId, emisRequestParameters.SessionId);
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
            EmisRequestParameters requestParameters, SlotsGetQueryParameters queryParameters)
        {
            var fromDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.FromDateTime).ToString("s", CultureInfo.InvariantCulture);
            var toDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.ToDateTime).ToString("s", CultureInfo.InvariantCulture);

            var path = string.Format(CultureInfo.InvariantCulture,
                AppointmentSlotsPath,
                requestParameters.UserPatientLinkToken,
                fromDateTime,
                toDateTime);

            var response = await Get<AppointmentSlotsGetResponse>(path, requestParameters.EndUserSessionId, requestParameters.SessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<PracticeSettingsGetResponse>> PracticeSettingsGet(
            EmisRequestParameters requestParameters, string practiceCode)
        {
            var path = string.Format(CultureInfo.InvariantCulture,
                                     PracticeSettingsPath,
                                     practiceCode);

            var response = await Get<PracticeSettingsGetResponse>(path, requestParameters.EndUserSessionId, requestParameters.SessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> AppointmentSlotsMetadataGet(
            EmisRequestParameters requestParameters, SlotsMetadataGetQueryParameters queryParameters)
        {
            var fromDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.SessionStartDate).ToString("s", CultureInfo.InvariantCulture);
            var toDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.SessionEndDate).ToString("s", CultureInfo.InvariantCulture);

            var path = string.Format(CultureInfo.InvariantCulture,
                AppointmentSlotsMetaPath,
                requestParameters.UserPatientLinkToken,
                fromDateTime,
                toDateTime);

            return await Get<AppointmentSlotsMetadataGetResponse>(path, requestParameters.EndUserSessionId,
                requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<CoursesGetResponse>> CoursesGet(EmisRequestParameters emisRequestParameters)
        {
            var path = string.Format(CultureInfo.InvariantCulture, CoursesPath, emisRequestParameters.UserPatientLinkToken);

            return await Get<CoursesGetResponse>(path, emisRequestParameters.EndUserSessionId, emisRequestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<BookAppointmentSlotPostResponse>> AppointmentsPost(
            EmisRequestParameters requestParameters,
            AppointmentBookRequest bookRequest)
        {
            var postRequest = new BookAppointmentSlotPostRequest(requestParameters.UserPatientLinkToken, bookRequest);

            return await Post<BookAppointmentSlotPostRequest, BookAppointmentSlotPostResponse>(postRequest,
                AppointmentsPath, requestParameters.EndUserSessionId, requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<AppointmentsGetResponse>> AppointmentsGet(
            EmisRequestParameters requestParameters)
        {
            var queryParams = string.Format(CultureInfo.InvariantCulture,
                "?UserPatientLinkToken={0}&FetchPreviousAppointments=true&PreviousAppointmentsFromDate={1}",
                requestParameters.UserPatientLinkToken,
                DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

            var path = $"{AppointmentsPath}{queryParams}";

            var response =
                await Get<AppointmentsGetResponse>(path, requestParameters.EndUserSessionId, requestParameters.SessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<CancelAppointmentDeleteResponse>> AppointmentsDelete(
            EmisRequestParameters requestParameters, long slotId, CancellationReason cancellationReason)
        {
            var deleteRequest = new CancelAppointmentDeleteRequest(
                requestParameters.UserPatientLinkToken,
                cancellationReason.DisplayName,
                slotId);

            return await Delete<CancelAppointmentDeleteRequest, CancelAppointmentDeleteResponse>(
                deleteRequest, AppointmentsPath, requestParameters.EndUserSessionId, requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<AddVerificationResponse>> VerificationPost(
            EmisRequestParameters requestParameters, AddVerificationRequest addVerificationRequest)
        {
            return await Post<AddVerificationRequest, AddVerificationResponse>(
                addVerificationRequest, MeVerificationsPath,
                requestParameters.EndUserSessionId,
                requestParameters.SessionId,
                _settings.EmisExtendedHttpTimeoutSeconds);
        }

        public async Task<EmisApiObjectResponse<MeSettingsGetResponse>> MeSettingsGet(EmisRequestParameters requestParameters)
        {
            _logger.LogInformation("EMIS: Fetching patient settings");
            var path = string.Format(CultureInfo.InvariantCulture, MeSettingsPath, requestParameters.UserPatientLinkToken);

            return await Get<MeSettingsGetResponse>(path, requestParameters.EndUserSessionId, requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<AddNhsUserResponse>> NhsUserPost(EmisRequestParameters requestParameters, AddNhsUserRequest addNhsUserRequest)
        {
            return await Post<AddNhsUserRequest, AddNhsUserResponse>(
                addNhsUserRequest, UsersNhsPath,
                requestParameters.EndUserSessionId,
                requestParameters.SessionId,
                _settings.EmisExtendedHttpTimeoutSeconds);
        }

        public async Task<EmisApiObjectResponse<MessagesGetResponse>> PatientMessagesGet(
            EmisRequestParameters requestParameters)
        {
            return await Get<MessagesGetResponse>(
                string.Format(CultureInfo.InvariantCulture, GetMessagesPath, requestParameters.UserPatientLinkToken),
                requestParameters.EndUserSessionId,
                requestParameters.SessionId);
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

            public override bool HasBadRequestResponse => StatusCode.IsBadRequestCode();

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
                    ParseResponse(responseParser, stringResponse, responseMessage, logger);
                }
            }

            private void ParseResponse(
                IResponseParser responseParser,
                string stringResponse,
                HttpResponseMessage responseMessage,
                ILogger logger)
            {
                if (string.IsNullOrEmpty(stringResponse))
                {
                    logger.LogError("No response body");
                    return;
                }

                Body = responseParser.ParseBody<TBody>(stringResponse);

                StandardErrorResponse = ParseBadRequestError<StandardErrorResponse>(
                        responseParser, stringResponse, responseMessage);
                ErrorResponseBadRequest = ParseBadRequestError<BadRequestErrorResponse>(
                    responseParser, stringResponse, responseMessage);
                ExceptionErrorResponse = responseParser.ParseError<ExceptionErrorResponse>(
                    stringResponse,
                    responseMessage,
                    HttpStatusCode.BadRequest);
            }
            protected override bool FormatResponseIfUnsuccessful => true;
        }

        private static T ParseBadRequestError<T>(
            IResponseParser responseParser,
            string stringResponse, HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                return responseParser.ParseBody<T>(stringResponse);
            }

            return default;
        }

        private static string EncodeDateTimeOffsetToIso(DateTimeOffset dateTimeOffset)
        {
            return HttpUtility.UrlEncode(dateTimeOffset.ToString("O", CultureInfo.InvariantCulture));
        }
    }
}
