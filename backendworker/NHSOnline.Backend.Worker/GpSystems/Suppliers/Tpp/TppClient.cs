using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.Worker.ResponseParsers;
using NHSOnline.Backend.Worker.Support.Http;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppClient : ITppClient
    {
        public const string RequestTypeHeader = "type";
        public const string ResponseSuidHeader = "suid";
        public const string RequestSuidHeader = "suid";
        public const string TppDateTimeFormat = "yyy-MM-ddTHH:mm:ss+00:00";

        private static readonly Regex ErrorRegex = new Regex("errorCode\\s?=");

        private readonly TppHttpClient _httpClient;
        private readonly ITppConfig _tppConfig;
        private readonly IXmlResponseParser _responseParser;
        private readonly ILogger<TppClient> _logger;
        public TppClient(TppHttpClient httpClient, ITppConfig tppConfig, IXmlResponseParser responseParser, ILogger<TppClient> logger)
        {
            _httpClient = httpClient;
            _tppConfig = tppConfig;
            _responseParser = responseParser;

            _logger = logger;
        }

        public async Task<TppApiObjectResponse<LinkAccountReply>> LinkAccountPost(LinkAccount linkAccountModel)
        {
            var response = await Post<LinkAccount, LinkAccountReply>(linkAccountModel);

            if (response.Body != null)
            {
                response.Body.ProviderId = linkAccountModel.Application.ProviderId;
            }

            return response;
        }

        public async Task<TppApiObjectResponse<AuthenticateReply>> AuthenticatePost(Authenticate authenticate)
        {
            authenticate.Application = new Application
            {
                Name = _tppConfig.ApplicationName,
                Version = _tppConfig.ApplicationVersion,
                ProviderId = authenticate.ProviderId,
                DeviceType = _tppConfig.ApplicationDeviceType
            };

            var response = await Post<Authenticate, AuthenticateReply>(authenticate);

            return response;
        }

        public async Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession tppUserSession)
        {
            _logger.LogEnter();

            var patientSelected = new PatientSelected
            {
                OnlineUserId = tppUserSession.OnlineUserId,
                PatientId = tppUserSession.PatientId,
                UnitId = tppUserSession.OdsCode,
            };

            _logger.LogExit();
            return await Post<PatientSelected, PatientSelectedReply>(patientSelected, tppUserSession.Suid);
        }

        public async Task<TppApiObjectResponse<ViewPatientOverviewReply>> PatientOverviewPost(
            TppUserSession tppUserSession)
        {
            _logger.LogEnter();

            var request = new ViewPatientOverview
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.OdsCode,
            };

            _logger.LogExit();
            return await Post<ViewPatientOverview, ViewPatientOverviewReply>(request, tppUserSession.Suid);
        }

        public async Task<TppApiObjectResponse<RequestPatientRecordReply>> RequestPatientRecordPost(
            TppUserSession tppUserSession)

        {
            _logger.LogEnter();

            var request = new RequestPatientRecord
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.OdsCode,
            };

            _logger.LogExit();
            return await Post<RequestPatientRecord, RequestPatientRecordReply>(request, tppUserSession.Suid);
        }

        public async Task<TppApiObjectResponse<BookAppointmentReply>> BookAppointmentSlotPost(
            BookAppointment bookAppointment, TppUserSession userSession)
        {
            return await Post<BookAppointment, BookAppointmentReply>(bookAppointment, userSession.Suid);
        }

        public async Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsView(TppUserSession tppUserSession,
            string startDate, string endDate)
        {
            _logger.LogDebug($"Entered: {nameof(TestResultsView)} with { nameof(startDate)}:{startDate} and {nameof(endDate)}:{endDate}");


            var request = new TestResultsView
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.OdsCode,
                StartDate = startDate,
                EndDate = endDate,
            };

            _logger.LogExit();
            return await Post<TestResultsView, TestResultsViewReply>(request, tppUserSession.Suid);
        }

        public async Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsViewDetailed(
            TppUserSession tppUserSession,
            string testResultId)
        {
            _logger.LogEnter();

            var request = new TestResultsViewDetailed
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.OdsCode,
                TestResultId = testResultId,
            };

            _logger.LogExit(); 
            return await Post<TestResultsViewDetailed, TestResultsViewReply>(request, tppUserSession.Suid);
        }

        public async Task<TppApiObjectResponse<LogoffReply>> LogoffPost(TppUserSession tppUserSession)
        {
            _logger.LogEnter();
            var request = new Logoff();

            var response = await Post<Logoff, LogoffReply>(request);

            _logger.LogExit();
            return response;
        }

        public async Task<TppApiObjectResponse<AddNhsUserResponse>> NhsUserPost(AddNhsUserRequest addNhsUserRequest)
        {
            SetApplicationOnRequest(addNhsUserRequest);

            addNhsUserRequest.ApiVersion = _tppConfig.ApiVersion;
            addNhsUserRequest.Uuid = _tppConfig.CreateGuid();

            var response = await Post<AddNhsUserRequest, AddNhsUserResponse>(addNhsUserRequest);

            if (response.Body != null)
            {
                response.Body.ProviderId = addNhsUserRequest.Application.ProviderId;
            }

            return response;
        }

        public async Task<TppApiObjectResponse<ListRepeatMedicationReply>> ListRepeatMedicationPost(
            TppUserSession tppUserSession)
        {
            var listRepeatMedication = new ListRepeatMedication
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.OdsCode,
            };

            var response =
                await Post<ListRepeatMedication, ListRepeatMedicationReply>(listRepeatMedication, tppUserSession.Suid);

            return response;
        }

        public async Task<TppApiObjectResponse<RequestMedicationReply>> OrderPrescriptionsPost(
            TppUserSession tppUserSession,
            RequestMedication requestMedication)
        {
            var response =
                await Post<RequestMedication, RequestMedicationReply>(requestMedication, tppUserSession.Suid);

            return response;
        }

        public async Task<TppApiObjectResponse<ListSlotsReply>> ListSlotsPost(ListSlots listSlots, string suid)
        {
            return await Post<ListSlots, ListSlotsReply>(listSlots, suid);
        }

        public async Task<TppApiObjectResponse<ViewAppointmentsReply>> ViewAppointmentsPost(
            ViewAppointments viewAppointments, string suid)
        {
            return await Post<ViewAppointments, ViewAppointmentsReply>(viewAppointments, suid);
        }

        public async Task<TppApiObjectResponse<CancelAppointmentReply>> CancelAppointmentPost(
            CancelAppointment cancelAppointment,
            string suid)
        {
            return await Post<CancelAppointment, CancelAppointmentReply>(cancelAppointment, suid);
        }

        private async Task<TppApiObjectResponse<TResponse>> Post<TRequest, TResponse>(TRequest model,
            string suid = null) where TRequest : ITppRequest
        {
            if (model is ITppApplicationRequest applicationRequest)
            {
                SetApplicationOnRequest(applicationRequest);
            }

            model.ApiVersion = _tppConfig.ApiVersion;
            model.Uuid = _tppConfig.CreateGuid();

            var authenticateXml = model.SerializeXml();
            var authenticateContent = new StringContent(authenticateXml, Encoding.UTF8, TppHttpClient.MediaType);
            var request = BuildTppRequest(HttpMethod.Post, model.RequestType, authenticateContent, suid);

            var response = await SendRequestAndParseResponse<TResponse>(request);

            return response;
        }

        private static HttpRequestMessage BuildTppRequest(HttpMethod method, string requestType,
            StringContent stringContent, string suid = null)
        {
            if (stringContent == null)
            {
                throw new ArgumentNullException(nameof(stringContent));
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = method,
                Content = stringContent
            };

            requestMessage.Headers.Add(RequestTypeHeader, requestType);

            if (!string.IsNullOrEmpty(suid))
            {
                requestMessage.Headers.Add(RequestSuidHeader, suid);
            }

            return requestMessage;
        }

        private async Task<TppApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new TppApiObjectResponse<TResponse>(responseMessage.StatusCode);
            await response.Parse(responseMessage, _responseParser, _logger);

            if (response.IsUnauthorisedResponse)
            {
                _logger.LogInformation("Unauthorised TPP response");
                throw new UnauthorisedGpSystemHttpRequestException();
            }

            return response;
        }

        private void SetApplicationOnRequest(ITppApplicationRequest request)
        {
            request.Application = request.Application ?? new Application();
            request.Application.Name = _tppConfig.ApplicationName;
            request.Application.Version = _tppConfig.ApplicationVersion;
            request.Application.ProviderId = _tppConfig.ApplicationProviderId;
            request.Application.DeviceType = _tppConfig.ApplicationDeviceType;
        }

        public abstract class TppApiResponse : ApiResponse
        {
            protected TppApiResponse(HttpStatusCode status) :base(status)
            {}

            public Error ErrorResponse { get; set; }

            public override bool HasSuccessResponse => ErrorResponse == null && StatusCode.IsSuccessStatusCode();

            internal bool IsUnauthorisedResponse =>
                ErrorResponse != null &&
                TppApiErrorCodes.NotAuthenticated.Equals(ErrorResponse.ErrorCode, StringComparison.Ordinal);

            public bool HasErrorWithCode(string errorCode)
            {
                return string.Equals(ErrorResponse?.ErrorCode, errorCode, StringComparison.Ordinal);
            }

            // User does not have access, Sean to confirm with TPP re using error codes
            public bool HasForbiddenResponse => ErrorResponse != null &&
                                                TppApiErrorCodes.NoAccess.Equals(ErrorResponse.ErrorCode,
                                                    StringComparison.Ordinal);

            public bool HasErrorMessageContaining(string message)
            {
                return ErrorResponse?.UserFriendlyMessage.Contains(message, StringComparison.Ordinal) ?? false;
            }
        }

        public class TppApiObjectResponse<TBody> : TppApiResponse
        {
            public TppApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {
            }


            public async Task Parse(
                HttpResponseMessage responseMessage,
                IXmlResponseParser responseParser,
                ILogger logger)
            {
                var stringResponse = await GetStringResponse(responseMessage, logger);
                if (!string.IsNullOrEmpty(stringResponse))
                {
                    ParseResponse(responseParser, logger, stringResponse, responseMessage);
                }
            }

            public TBody Body { get; set; }
            public Dictionary<string, string> Headers { get; set; }

            public override string ErrorForLogging => $"Error Code: '{ErrorResponse?.ErrorCode}'. " +
                                             $"Error User Message:'{ErrorResponse?.UserFriendlyMessage}'. " +
                                             $"Error Technical Response:'{ErrorResponse?.TechnicalMessage}'.";

            protected override bool FormatResponseIfUnsuccessful => false;

            private TppApiObjectResponse<TBody> ParseResponse(
                IResponseParser responseParser,
                ILogger logger,
                string stringResponse,
                HttpResponseMessage responseMessage)
            {
                try
                {
                    if (IsErrorResponse(stringResponse))
                    {
                        ErrorResponse = responseParser.ParseBody<Error>(stringResponse, responseMessage);
                        logger.LogError($"Server returned with error. {ErrorForLogging}");
                        return this;
                    }

                    Body = responseParser.ParseBody<TBody>(stringResponse, responseMessage);
                }
                catch (FormatException e)
                {
                    logger.LogError(e, "An error occured while parsing the response");
                    return new TppApiObjectResponse<TBody>(HttpStatusCode.InternalServerError);
                }

                if (responseMessage.Headers.TryGetValues(ResponseSuidHeader, out var values))
                {
                    Headers = new Dictionary<string, string>
                    {
                        { ResponseSuidHeader, values.First() }
                    };
                }

                return this;
            }

            private static bool IsErrorResponse(string responseString)
            {
                return ErrorRegex.IsMatch(responseString);
            }
        }
    }
}