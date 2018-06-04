using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.Date;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisClient : IEmisClient
    {
        private readonly TimeZoneConverter _localTimeZoneConverter;
        public const string HeaderApplicationId = "X-API-ApplicationId";
        public const string HeaderEndUserSessionId = "X-API-EndUserSessionId";
        public const string HeaderSessionId = "X-API-SessionId";
        public const string HeaderVersion = "X-API-Version";

        private const string MeApplicationsPath = "me/applications";
        private const string SessionsEndUserSessionPath = "sessions/endusersession";
        private const string SessionsPath = "sessions";
        private const string DemographicsPath = "demographics?userPatientLinkToken={0}";
        private const string PatientAllergiesPath = "record?userPatientLinkToken={0}&itemType=Allergies";
        private const string AppointmentSlotsMetaPath = "appointmentslots/meta?userPatientLinkToken={0}&sessionStartDate={1}&sessionEndDate={2}";
        private const string AppointmentSlotsPath = "appointmentslots?userPatientLinkToken={0}&fromDateTime={1}&toDateTime={2}";
        private const string PrescriptionsPath = "prescriptionrequests?userPatientLinkToken={0}";
        private const string CoursesPath = "courses?userPatientLinkToken={0}";
        private const string AppointmentsPath = "appointments";

        private readonly HttpClient _httpClient;
        private readonly ILogger<EmisClient> _logger;

        public EmisClient(IHttpClientFactory httpClientFactory, 
            IEmisConfig config,
            TimeZoneConverter localTimeZoneConverter,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EmisClient>();
            _localTimeZoneConverter = localTimeZoneConverter;
            _httpClient = httpClientFactory.GetClient(HttpClientName.EmisApiClient);
            _httpClient.DefaultRequestHeaders.Add(HeaderApplicationId, config.ApplicationId);
            _httpClient.DefaultRequestHeaders.Add(HeaderVersion, config.Version);
            _httpClient.BaseAddress = config.BaseUrl;
        }

        public async Task<EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost()
        {
            var response = await Post<SessionsEndUserSessionPostResponse>(SessionsEndUserSessionPath);
            return response;
        }

        public async Task<EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(string endUserSessionId, SessionsPostRequest model)
        {
            var response = await Post<SessionsPostRequest, SessionsPostResponse>(model, SessionsPath, endUserSessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(string userPatientLinkToken, string responseSessionId,
            string endUserSessionId)
        {
            var path = string.Format(DemographicsPath, userPatientLinkToken);

            _logger.LogInformation("DemographicsGet path:" + path);
            
            return await Get<DemographicsGetResponse>(path, endUserSessionId, responseSessionId);
        }

        public async Task<EmisApiObjectResponse<AllergyRequestsGetResponse>> AllergiesGet(string userPatientLinkToken, string responseSessionId,
            string endUserSessionId)
        {
            var path = string.Format(PatientAllergiesPath, userPatientLinkToken);

            var response = await Get<AllergyRequestsGetResponse>(path, endUserSessionId, responseSessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(string endUserSessionId, MeApplicationsPostRequest model)
        {
            var response = await Post<MeApplicationsPostRequest, MeApplicationsPostResponse>(model, MeApplicationsPath, endUserSessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<PrescriptionRequestsGetResponse>> PrescriptionsGet(string userPatientLinkToken, string responseSessionId,
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

            var response = await Get<PrescriptionRequestsGetResponse>(path, endUserSessionId, responseSessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<PrescriptionRequestPostResponse>> PrescriptionsPost( string responseSessionId, string endUserSessionId,
            PrescriptionRequestsPost model)
        {  
            return await Post<PrescriptionRequestsPost, PrescriptionRequestPostResponse>(model, PrescriptionsPath, endUserSessionId, responseSessionId);
        }

        public async Task<EmisApiObjectResponse<AppointmentsSlotsGetResponse>> AppointmentsSlotsGet(
            EmisHeaderParameters headerParameters, SlotsGetQueryParameters getQueryParameters)
        {
            var dateStringFormatter = new DateStringFormatter();

            var fromDateTime = dateStringFormatter.Format(_localTimeZoneConverter.ToLocalTime(getQueryParameters.FromDateTime));
            var toDateTime = dateStringFormatter.Format(_localTimeZoneConverter.ToLocalTime(getQueryParameters.ToDateTime));

            var path = string.Format(AppointmentSlotsPath,
                getQueryParameters.UserPatientLinkToken,
                fromDateTime,
                toDateTime);

            var response = await Get<AppointmentsSlotsGetResponse>(path, headerParameters.EndUserSessionId, headerParameters.SessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> AppointmentsSlotsMetadataGet(
            EmisHeaderParameters headerParameters, SlotsMetadataGetQueryParameters getQueryParameters)
        {
            var dateStringFormatter = new DateStringFormatter();
            var fromDateTime = dateStringFormatter.Format(_localTimeZoneConverter.ToLocalTime(getQueryParameters.SessionStartDate));
            var toDateTime = dateStringFormatter.Format(_localTimeZoneConverter.ToLocalTime(getQueryParameters.SessionEndDate));

            var path = string.Format(AppointmentSlotsMetaPath,
                getQueryParameters.UserPatientLinkToken,
                fromDateTime,
                toDateTime);

            var response = await Get<AppointmentSlotsMetadataGetResponse>(path, headerParameters.EndUserSessionId, headerParameters.SessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<CoursesGetResponse>> CoursesGet(string userPatientLinkToken, string responseSessionId, string endUserSessionId)
        {
            var path = string.Format(CoursesPath, userPatientLinkToken);

            var response = await Get<CoursesGetResponse>(path, endUserSessionId, responseSessionId);
            return response;
        }
        
        public async Task<EmisApiObjectResponse<BookAppointmentSlotPostResponse>> AppointmentPost(EmisHeaderParameters headerParameters,
            BookAppointmentSlotPostRequest postRequest)
        {
            return await Post<BookAppointmentSlotPostRequest, BookAppointmentSlotPostResponse>(postRequest, AppointmentsPath, headerParameters.EndUserSessionId, headerParameters.SessionId);
        }

        private async Task<EmisApiObjectResponse<TResponse>> Get<TResponse>(string path,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Get, path, endUserSessionId, sessionId);
            var response = await SendRequestAndParseResponse<TResponse>(request);
            return response;
        }

        private async Task<EmisApiObjectResponse<TResponse>> Post<TRequest, TResponse>(TRequest model, string path,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Post, path, endUserSessionId, sessionId);

            var body = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await SendRequestAndParseResponse<TResponse>(request);
            return response;
        }

        private async Task<EmisApiObjectResponse<TResponse>> Post<TResponse>(string path,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Post, path, endUserSessionId, sessionId);

            var response = await SendRequestAndParseResponse<TResponse>(request);
            return response;
        }

        private static HttpRequestMessage BuildEmisRequest(HttpMethod httpMethod, string path, string endUserSessionId = null, string sessionId = null)
        {
            var request = new HttpRequestMessage(httpMethod, path);

            if (!string.IsNullOrEmpty(endUserSessionId))
            {
                request.Headers.Add(HeaderEndUserSessionId, new[] {endUserSessionId});
            }

            if (!string.IsNullOrEmpty(sessionId))
            {
                request.Headers.Add(HeaderSessionId, new[] {sessionId});
            }

            return request;
        }

        private async Task<EmisApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.SendAsync(request);
            var response = new EmisApiObjectResponse<TResponse>(responseMessage.StatusCode);

            var stringResponse = responseMessage.Content != null
                ? await responseMessage.Content.ReadAsStringAsync()
                : null;

            if (string.IsNullOrEmpty(stringResponse)) return response;
            
            response.Body = stringResponse.ParseBody<TResponse>(responseMessage);
            response.ErrorResponseBadRequest = stringResponse.ParseBadRequest<BadRequestErrorResponse>(responseMessage);
            response.ErrorResponse = stringResponse.ParseError<ErrorResponse>(responseMessage, HttpStatusCode.BadRequest);

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
            public bool HasSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode <= 299;

            public bool HasExceptionWithMessage(string message)
            {
                return ErrorResponse?.Exceptions?.Any(x => x.Message == message) ?? false;
            }
            
            public bool HasExceptionWithMessageContaining(string message)
            {
                return ErrorResponse?.Exceptions?.Any(x => x.Message.Contains(message)) ?? false;
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
