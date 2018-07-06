using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.ResponseParsers;
using NHSOnline.Backend.Worker.Support.Date;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisClient : IEmisClient
    {
        private readonly TimeZoneConverter _localTimeZoneConverter;

        public const string HeaderEndUserSessionId = "X-API-EndUserSessionId";
        public const string HeaderSessionId = "X-API-SessionId";
        public const string HeaderNhsNumber = "NhsNumber";
        public const string HeaderOdsCode = "OdsCode";

        private const string MeApplicationsPath = "me/applications";
        private const string SessionsEndUserSessionPath = "sessions/endusersession";
        private const string SessionsPath = "sessions";
        private const string DemographicsPath = "demographics?userPatientLinkToken={0}";
        private const string PatientRecordPath = "record?userPatientLinkToken={0}&itemType={1}";

        private const string AppointmentSlotsMetaPath =
            "appointmentslots/meta?userPatientLinkToken={0}&sessionStartDate={1}&sessionEndDate={2}";

        private const string AppointmentSlotsPath =
            "appointmentslots?userPatientLinkToken={0}&fromDateTime={1}&toDateTime={2}";

        private const string PrescriptionsPath = "prescriptionrequests?userPatientLinkToken={0}";
        private const string PrescriptionsPostPath = "prescriptionrequests";
        private const string CoursesPath = "courses?userPatientLinkToken={0}";
        private const string AppointmentsPath = "appointments";

        private readonly EmisHttpClient _httpClient;
        private const string LinkagePath = "patient/linkage";
        
        private readonly ILogger<EmisClient> _logger;
        private readonly IJsonResponseParser _responseParser;

        public EmisClient(EmisHttpClient httpClient,
            IEmisConfig config,
            TimeZoneConverter localTimeZoneConverter,
            ILoggerFactory loggerFactory,
            IJsonResponseParser responseParser)
        {
            _logger = loggerFactory.CreateLogger<EmisClient>();
            _localTimeZoneConverter = localTimeZoneConverter;
            _httpClient = httpClient;

            _responseParser = responseParser;
        }

        public async Task<EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost()
        {
            return await Post<SessionsEndUserSessionPostResponse>(SessionsEndUserSessionPath);
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
            var path = string.Format(DemographicsPath, userPatientLinkToken);

            return await Get<DemographicsGetResponse>(path, endUserSessionId, responseSessionId);
        }

        public async Task<EmisApiObjectResponse<MedicationRootObject>> MedicalRecordGet(string userPatientLinkToken,
            string responseSessionId, string endUserSessionId,
            RecordType recordType)
        {
            var path = string.Format(PatientRecordPath, userPatientLinkToken, recordType.ToString());

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
            var path = string.Format(PrescriptionsPath, userPatientLinkToken);

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
            EmisHeaderParameters headerParameters, SlotsGetQueryParameters getQueryParameters)
        {
            var dateStringFormatter = new DateStringFormatter();

            var fromDateTime =
                dateStringFormatter.Format(_localTimeZoneConverter.ToLocalTime(getQueryParameters.FromDateTime));
            var toDateTime =
                dateStringFormatter.Format(_localTimeZoneConverter.ToLocalTime(getQueryParameters.ToDateTime));

            var path = string.Format(AppointmentSlotsPath,
                getQueryParameters.UserPatientLinkToken,
                fromDateTime,
                toDateTime);

            var response = await Get<AppointmentSlotsGetResponse>(path, headerParameters.EndUserSessionId, headerParameters.SessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> AppointmentSlotsMetadataGet(
            EmisHeaderParameters headerParameters, SlotsMetadataGetQueryParameters getQueryParameters)
        {
            var dateStringFormatter = new DateStringFormatter();
            var fromDateTime =
                dateStringFormatter.Format(_localTimeZoneConverter.ToLocalTime(getQueryParameters.SessionStartDate));
            var toDateTime =
                dateStringFormatter.Format(_localTimeZoneConverter.ToLocalTime(getQueryParameters.SessionEndDate));

            var path = string.Format(AppointmentSlotsMetaPath,
                getQueryParameters.UserPatientLinkToken,
                fromDateTime,
                toDateTime);

            return await Get<AppointmentSlotsMetadataGetResponse>(path, headerParameters.EndUserSessionId,
                headerParameters.SessionId);
        }

        public async Task<EmisApiObjectResponse<CoursesGetResponse>> CoursesGet(string userPatientLinkToken,
            string responseSessionId, string endUserSessionId)
        {
            var path = string.Format(CoursesPath, userPatientLinkToken);

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
            EmisHeaderParameters headerParameters, string userPatientLinkToken, bool fetchPreviousAppointments,
            DateTimeOffset? previousAppointmentsFromDate)
        {
            var qb = new QueryBuilder
            {
                { "userPatientLinkToken", userPatientLinkToken },
                { "fetchPreviousAppointments", fetchPreviousAppointments.ToString().ToLowerInvariant() }
            };

            if (fetchPreviousAppointments && previousAppointmentsFromDate.HasValue)
            {
                qb.Add("previousAppointmentsFromDate", EncodeDateTimeOffsetToIso(previousAppointmentsFromDate.Value));
            }

            var path = $"{AppointmentsPath}{qb.ToQueryString()}";

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

        public async Task<EmisApiObjectResponse<LinkageDetailsResponse>> LinkageGet(string nhsNumber, string odsCode)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, LinkagePath);

            if (!string.IsNullOrEmpty(nhsNumber))
            {
                request.Headers.Add(HeaderNhsNumber, new[] { nhsNumber });
            }

            if (!string.IsNullOrEmpty(odsCode))
            {
                request.Headers.Add(HeaderOdsCode, new[] { odsCode });
            }

            var response = await SendRequestAndParseResponse<LinkageDetailsResponse>(request);
            return response;
        }

        public async Task<EmisApiObjectResponse<LinkageDetailsResponse>> LinkagePost(LinkagePostRequest linkagePostRequest)
        {
            var path = LinkagePath;

            var response = await Post<LinkagePostRequest, LinkageDetailsResponse>(linkagePostRequest, path);
            return response;
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
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Get, path, endUserSessionId, sessionId);
            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Post<TRequest, TResponse>(TRequest model, string path,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Post, path, endUserSessionId, sessionId);

            var body = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Post<TResponse>(string path,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Post, path, endUserSessionId, sessionId);

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
            _logger.LogHttpRequest(request);
            var responseMessage = await _httpClient.Client.SendAsync(request);
            _logger.LogHttpResponse(request, responseMessage);
            var response = new EmisApiObjectResponse<TResponse>(responseMessage.StatusCode);

            var stringResponse = responseMessage.Content != null
                ? await responseMessage.Content.ReadAsStringAsync()
                : null;

            if (string.IsNullOrEmpty(stringResponse)) return response;

            response.Body = _responseParser.ParseBody<TResponse>(stringResponse, responseMessage);
            response.ErrorResponseBadRequest =
                _responseParser.ParseBadRequest<BadRequestErrorResponse>(stringResponse, responseMessage);
            response.ErrorResponse =
                _responseParser.ParseError<ErrorResponse>(stringResponse, responseMessage, HttpStatusCode.BadRequest);

            return response;
        }

        public class EmisApiResponse
        {
            public EmisApiResponse(HttpStatusCode statusCode)
            {
                StatusCode = statusCode;
            }

            public HttpStatusCode StatusCode { get; set; }
            public ErrorResponse ErrorResponse { get; set; }
            public BadRequestErrorResponse ErrorResponseBadRequest { get; set; }
            public bool HasSuccessStatusCode => StatusCode.IsSuccessStatusCode();

            public bool HasExceptionWithMessage(string message)
            {
                return ErrorResponse?.Exceptions?.Any(x => x.Message == message) ?? false;
            }

            public bool HasExceptionWithMessageContaining(string message)
            {
                return ErrorResponse?.Exceptions?.Any(x => x.Message.Contains(message)) ?? false;
            }
            
            public bool HasExceptionWithAnyMessage(string[] messages)
            {
                return ErrorResponse?.Exceptions.Any(x => messages.Contains(x.Message)) ?? false;
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
        }

        public class EmisApiObjectResponse<TBody> : EmisApiResponse
        {
            public EmisApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {
            }

            public TBody Body { get; set; }
        }

        private static string EncodeDateTimeOffsetToIso(DateTimeOffset dateTimeOffset)
        {
            return HttpUtility.UrlEncode(dateTimeOffset.ToString("O"));
        }
    }
}
