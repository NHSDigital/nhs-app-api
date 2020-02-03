using System;
using System.Collections.Generic;
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
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
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
        private const string GetMessageDetailsPath = "messages/{0}/?userPatientLinkToken={1}";
        private const string GetMessageRecipientsPath = "messagerecipients?userPatientLinkToken={0}";
        private const string PutMessageReadStatusUpdate = "messages";
        private const string PostMessagePath = "messages";
        private const string DeleteMessagePath = "messages/{0}";

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
                SessionsEndUserSessionPath,
                requestType: RequestsForSuccessOutcome.SessionsEndUserSessionPost, GetDefaultSuccessStatusCodeList(), customTimeout: _settings.EmisExtendedHttpTimeoutSeconds);
        }

        public async Task<EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(string endUserSessionId,
            SessionsPostRequest model)
        {
            return await Post<SessionsPostRequest, SessionsPostResponse>(
                model,
                SessionsPath,
                RequestsForSuccessOutcome.SessionsPost, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: endUserSessionId);
        }

        public async Task<EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(EmisRequestParameters emisRequestParameters)

        {
            _logger.LogInformation("EMIS: Fetching patient demographics");
            var path = string.Format(CultureInfo.InvariantCulture, DemographicsPath, emisRequestParameters.UserPatientLinkToken);

            return await Get<DemographicsGetResponse>
                (path,
                RequestsForSuccessOutcome.DemographicsGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: emisRequestParameters.EndUserSessionId,
                sessionId: emisRequestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<MedicationRootObject>> MedicalRecordGet(EmisRequestParameters emisRequestParameters, RecordType recordType)
        {
            _logger.LogInformation("EMIS: Fetching patient medical record - {0}", recordType.ToString());
            var path = string.Format(CultureInfo.InvariantCulture,
                PatientRecordPath, emisRequestParameters.UserPatientLinkToken, recordType.ToString());

            var successStatusCodeLists = GetDefaultSuccessStatusCodeList();
            successStatusCodeLists.Add(HttpStatusCode.Forbidden);
            return await Get<MedicationRootObject>(path,
                RequestsForSuccessOutcome.MedicalRecordGet, successStatusCodeLists,
                endUserSessionId: emisRequestParameters.EndUserSessionId,
                sessionId: emisRequestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<IndividualDocument>> MedicalDocumentGet(string userPatientLinkToken,
            string responseSessionId, string documentGuid, string endUserSessionId)
        {

            _logger.LogInformation("EMIS: Fetching patient document - {0}", documentGuid);
              var path = string.Format(CultureInfo.InvariantCulture,
                DocumentPath,
                documentGuid,
                userPatientLinkToken);

            return await Get<IndividualDocument>(
                path,
                RequestsForSuccessOutcome.MedicalDocumentGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: endUserSessionId,
                sessionId: responseSessionId);
        }

        public async Task<EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(string endUserSessionId,
            MeApplicationsPostRequest model)
        {
            return await Post<MeApplicationsPostRequest, MeApplicationsPostResponse>(
                model,
                MeApplicationsPath,
                RequestsForSuccessOutcome.MeApplicationsPost, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: endUserSessionId, customTimeout: _settings.EmisExtendedHttpTimeoutSeconds);
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

            return await Get<PrescriptionRequestsGetResponse>(path,
                RequestsForSuccessOutcome.PrescriptionsGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: emisRequestParameters.EndUserSessionId, sessionId: emisRequestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<PrescriptionRequestPostResponse>> PrescriptionsPost(
            string responseSessionId,
            string endUserSessionId,
            PrescriptionRequestsPost model)
        {
            return await Post<PrescriptionRequestsPost, PrescriptionRequestPostResponse>(model, PrescriptionsPostPath,
                RequestsForSuccessOutcome.PrescriptionsPost, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: endUserSessionId, sessionId: responseSessionId);
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

            var response = await Get<AppointmentSlotsGetResponse>(path,
                RequestsForSuccessOutcome.AppointmentSlotsGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId, sessionId: requestParameters.SessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<PracticeSettingsGetResponse>> PracticeSettingsGet(
            EmisRequestParameters requestParameters, string practiceCode)
        {
            var path = string.Format(CultureInfo.InvariantCulture,
                                     PracticeSettingsPath,
                                     practiceCode);

            var response = await Get<PracticeSettingsGetResponse>(path,
                RequestsForSuccessOutcome.PracticeSettingsGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId, sessionId: requestParameters.SessionId);
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

            return await Get<AppointmentSlotsMetadataGetResponse>(path,
                RequestsForSuccessOutcome.AppointmentSlotsMetadataGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId,
                sessionId: requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<CoursesGetResponse>> CoursesGet(EmisRequestParameters emisRequestParameters)
        {
            var path = string.Format(CultureInfo.InvariantCulture, CoursesPath, emisRequestParameters.UserPatientLinkToken);

            return await Get<CoursesGetResponse>(path,
                RequestsForSuccessOutcome.CoursesGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: emisRequestParameters.EndUserSessionId, sessionId: emisRequestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<BookAppointmentSlotPostResponse>> AppointmentsPost(
            EmisRequestParameters requestParameters,
            AppointmentBookRequest bookRequest)
        {
            var postRequest = new BookAppointmentSlotPostRequest(requestParameters.UserPatientLinkToken, bookRequest);

            return await Post<BookAppointmentSlotPostRequest, BookAppointmentSlotPostResponse>(postRequest,
                AppointmentsPath,
                RequestsForSuccessOutcome.AppointmentsPost, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId, sessionId: requestParameters.SessionId);
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
                await Get<AppointmentsGetResponse>(path,
                    RequestsForSuccessOutcome.AppointmentsGet, GetDefaultSuccessStatusCodeList(),
                    endUserSessionId: requestParameters.EndUserSessionId, sessionId: requestParameters.SessionId);
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
                deleteRequest, AppointmentsPath,
                RequestsForSuccessOutcome.AppointmentsDelete, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId, sessionId: requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<AddVerificationResponse>> VerificationPost(
            EmisRequestParameters requestParameters, AddVerificationRequest addVerificationRequest)
        {
            var successStatusCodes = GetDefaultSuccessStatusCodeList();
            successStatusCodes.Add(HttpStatusCode.Conflict);
            return await Post<AddVerificationRequest, AddVerificationResponse>(
                addVerificationRequest, MeVerificationsPath,
                RequestsForSuccessOutcome.VerificationPost, successStatusCodes,
                endUserSessionId: requestParameters.EndUserSessionId,
                sessionId: requestParameters.SessionId,
                customTimeout: _settings.EmisExtendedHttpTimeoutSeconds);
        }

        public async Task<EmisApiObjectResponse<MeSettingsGetResponse>> MeSettingsGet(EmisRequestParameters requestParameters)
        {
            _logger.LogInformation("EMIS: Fetching patient settings");
            var path = string.Format(CultureInfo.InvariantCulture, MeSettingsPath, requestParameters.UserPatientLinkToken);

            return await Get<MeSettingsGetResponse>(path,
                RequestsForSuccessOutcome.MeSettingsGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId, sessionId: requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<AddNhsUserResponse>> NhsUserPost(EmisRequestParameters requestParameters, AddNhsUserRequest addNhsUserRequest)
        {
            return await Post<AddNhsUserRequest, AddNhsUserResponse>(
                addNhsUserRequest, UsersNhsPath,
                RequestsForSuccessOutcome.NhsUserPost, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId,
                sessionId: requestParameters.SessionId,
                customTimeout: _settings.EmisExtendedHttpTimeoutSeconds);
        }

        public async Task<EmisApiObjectResponse<MessagesGetResponse>> PatientMessagesGet(
            EmisRequestParameters requestParameters)
        {
            return await Get<MessagesGetResponse>(
                string.Format(CultureInfo.InvariantCulture, GetMessagesPath, requestParameters.UserPatientLinkToken),
                RequestsForSuccessOutcome.PatientMessagesGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId,
                sessionId: requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<MessageGetResponse>> PatientMessageDetailsGet(string messageId, EmisRequestParameters requestParameters)
        {
            return await Get<MessageGetResponse>(
                string.Format(CultureInfo.InvariantCulture, GetMessageDetailsPath, messageId, requestParameters.UserPatientLinkToken),
                RequestsForSuccessOutcome.PatientMessageDetailsGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId,
                sessionId: requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<MessageUpdateResponse>> PatientMessageUpdatePut(
            EmisRequestParameters requestParameters, UpdateMessageReadStatusRequest updateMessageReadStatusRequest)
        {
            return await Put<UpdateMessageReadStatusRequest, MessageUpdateResponse>(
                updateMessageReadStatusRequest, PutMessageReadStatusUpdate,
                RequestsForSuccessOutcome.PatientMessageUpdatePut, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId,
                sessionId: requestParameters.SessionId,
                customTimeout: _settings.EmisExtendedHttpTimeoutSeconds);
        }

        public async Task<EmisApiObjectResponse<MessageRecipientsGetResponse>> PatientMessageRecipientsGet(
            EmisRequestParameters requestParameters)
        {
            return await Get<MessageRecipientsGetResponse>(
                string.Format(CultureInfo.InvariantCulture, GetMessageRecipientsPath,
                    requestParameters.UserPatientLinkToken),
                RequestsForSuccessOutcome.PatientMessageRecipientsGet, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId,
                sessionId: requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<MessagePostResponse>> PatientMessagePost(EmisRequestParameters requestParameters,
            CreatePatientMessage message)
        {
            var sendMessageRequest = new PostMessageRequest(requestParameters.UserPatientLinkToken, message);
            return await Post<PostMessageRequest, MessagePostResponse>(sendMessageRequest, PostMessagePath,
                RequestsForSuccessOutcome.SendPatientMessagePost, GetDefaultSuccessStatusCodeList(),
                requestParameters.EndUserSessionId, requestParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<MessageDeleteResponse>> PatientPracticeMessageDelete(EmisRequestParameters requestParameters, string messageId)
        {
            var deleteMessageRequest = new MessageDeleteRequest()
            {
                UserPatientLinkToken = requestParameters.UserPatientLinkToken
            };

            return await Delete<MessageDeleteRequest, MessageDeleteResponse>(
                deleteMessageRequest,
                string.Format(CultureInfo.InvariantCulture, DeleteMessagePath, messageId),
                RequestsForSuccessOutcome.PatientPracticeMessageDelete, GetDefaultSuccessStatusCodeList(),
                endUserSessionId: requestParameters.EndUserSessionId, sessionId: requestParameters.SessionId);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Delete<TRequest, TResponse>(TRequest model, string path,
            RequestsForSuccessOutcome requestType,
            List<HttpStatusCode> successStatusCodes,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Delete, path, endUserSessionId, sessionId);

            var body = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await SendRequestAndParseResponse<TResponse>(request, requestType, successStatusCodes);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Get<TResponse>(string path,
            RequestsForSuccessOutcome requestType,
            List<HttpStatusCode> successStatusCodes,
            string endUserSessionId = null, string sessionId = null, int? customTimeout = null)
        {
            var request = BuildEmisRequest(HttpMethod.Get, path, endUserSessionId, sessionId);

            request.CheckAndSetCustomTimeout(customTimeout);

            return await SendRequestAndParseResponse<TResponse>(request, requestType, successStatusCodes);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Post<TRequest, TResponse>(TRequest model, string path,
            RequestsForSuccessOutcome requestType,
            List<HttpStatusCode> successStatusCodes,
            string endUserSessionId = null, string sessionId = null, int? customTimeout = null)
        {
            var request = BuildEmisRequest(HttpMethod.Post, path, endUserSessionId, sessionId);

            request.CheckAndSetCustomTimeout(customTimeout);

            var body = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await SendRequestAndParseResponse<TResponse>(request, requestType, successStatusCodes);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Post<TResponse>(string path,
            RequestsForSuccessOutcome requestType,
            List<HttpStatusCode> successStatusCodes,
            string endUserSessionId = null, string sessionId = null, int? customTimeout = null)
        {
            var request = BuildEmisRequest(HttpMethod.Post, path, endUserSessionId, sessionId);

            request.CheckAndSetCustomTimeout(customTimeout);

            return await SendRequestAndParseResponse<TResponse>(request, requestType, successStatusCodes);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Put<TRequest, TResponse>(TRequest model, string path,
            RequestsForSuccessOutcome requestType,
            List<HttpStatusCode> successStatusCodes,
            string endUserSessionId = null, string sessionId = null, int? customTimeout = null)
        {
            var request = BuildEmisRequest(HttpMethod.Put, path, endUserSessionId, sessionId);

            request.CheckAndSetCustomTimeout(customTimeout);

            var body = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await SendRequestAndParseResponse<TResponse>(request, requestType, successStatusCodes);
        }

        private static List<HttpStatusCode> GetDefaultSuccessStatusCodeList()
        {
            return new List<HttpStatusCode>()
            {
                HttpStatusCode.OK
            };
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
            HttpRequestMessage request, RequestsForSuccessOutcome requestType, List<HttpStatusCode> successStatusCodes)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new EmisApiObjectResponse<TResponse>(responseMessage.StatusCode, requestType, successStatusCodes);
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

            public bool HasBadRequestResponse => StatusCode.IsBadRequestCode();

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

            public string ErrorForLogging => $"Error Code: '{StatusCode}'. " +
                                             $"Error Message:'{StandardErrorResponse?.Message}'. ";

        }

        public class EmisApiObjectResponse<TBody> : EmisApiResponse
        {
            public TBody Body { get; set; }
            private RequestsForSuccessOutcome RequestType { get; }
            private List<HttpStatusCode> StatusCodes { get; }

            public EmisApiObjectResponse(HttpStatusCode statusCode, RequestsForSuccessOutcome typeOfRequest,
                List<HttpStatusCode> statusCodes) : base(
                statusCode)
            {
                RequestType = typeOfRequest;
                StatusCodes = statusCodes;
            }

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

                var successStrategy = GetOutcomeEvaluator(RequestType);
                Body = successStrategy.IsSuccess(StatusCodes, StatusCode,
                    responseMessage.IsSuccessStatusCode , stringResponse) ?  responseParser.ParseBody<TBody>(stringResponse): default;

                if (successStrategy.PopulateErrors(StatusCodes, responseMessage.IsSuccessStatusCode, StatusCode))
                {
                    StandardErrorResponse = ParseBadRequestError<StandardErrorResponse>(
                        responseParser, stringResponse, responseMessage);
                    ErrorResponseBadRequest = ParseBadRequestError<BadRequestErrorResponse>(
                        responseParser, stringResponse, responseMessage);
                    ExceptionErrorResponse = responseParser.ParseError<ExceptionErrorResponse>(
                        stringResponse,
                        responseMessage,
                        HttpStatusCode.BadRequest);
                }
                else
                {
                    StandardErrorResponse = default;
                    ErrorResponseBadRequest = default;
                    ExceptionErrorResponse = default;
                }
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

        private static IResponseSuccessOutcomeStrategy GetOutcomeEvaluator(RequestsForSuccessOutcome requestType)
        {
            switch (requestType)
            {
                case RequestsForSuccessOutcome.PatientMessageDetailsGet:
                    return new RegexSuccessOutcomeEvaluation();
                case RequestsForSuccessOutcome.VerificationPost:
                case RequestsForSuccessOutcome.MedicalRecordGet:
                    return new StatusCodeSuccessOutcomeEvaluation();
                default:
                    return new DefaultSuccessOutcomeEvaluation();
            }
        }
    }
}
