using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.ResponseParsers;

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
        
        public TppClient(TppHttpClient httpClient, ITppConfig tppConfig, IXmlResponseParser responseParser, ILoggerFactory loggerFactory)
        {
            _httpClient = httpClient;
            _tppConfig = tppConfig;
            _responseParser = responseParser;

            _logger = loggerFactory.CreateLogger<TppClient>();
        }
        
        public async Task<TppApiObjectResponse<AuthenticateReply>> AuthenticatePost(Authenticate authenticateModel)
        {
            authenticateModel.Application = new Application
            {
                Name = _tppConfig.ApplicationName,
                Version = _tppConfig.ApplicationVersion,
                ProviderId = _tppConfig.ApplicationProviderId,
                DeviceType = _tppConfig.ApplicationDeviceType
            };
            
            var response = await Post<Authenticate, AuthenticateReply>(authenticateModel);

            return response;
        }
        
        public async Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession tppUserSession)
        {           
            var patientSelected = new PatientSelected
            {
                OnlineUserId = tppUserSession.OnlineUserId,
                PatientId =  tppUserSession.PatientId,
                UnitId = tppUserSession.UnitId,                  
            };
           
            return await Post<PatientSelected, PatientSelectedReply>(patientSelected, tppUserSession.Suid);          
        }      
        
        public async Task<TppApiObjectResponse<ViewPatientOverviewReply>> PatientOverviewPost(TppUserSession tppUserSession)
        {
            var request = new ViewPatientOverview
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.UnitId,
            };
            
            return await Post<ViewPatientOverview, ViewPatientOverviewReply>(request, tppUserSession.Suid);
        }
        
        public async Task<TppApiObjectResponse<RequestPatientRecordReply>> RequestPatientRecordPost(
            TppUserSession tppUserSession)

        {
            var request = new RequestPatientRecord
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.UnitId,
            };  
            
            return await Post<RequestPatientRecord, RequestPatientRecordReply>(request, tppUserSession.Suid);
        }

        public async Task<TppApiObjectResponse<BookAppointmentReply>> BookAppointmentSlotPost(BookAppointment bookAppointment, TppUserSession userSession)
        {
            return await Post<BookAppointment, BookAppointmentReply>(bookAppointment, userSession.Suid);
        }

        public async Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsView(TppUserSession tppUserSession,
            string startDate, string endDate)
        {
            var request = new TestResultsView
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.UnitId,
                StartDate = startDate,
                EndDate = endDate,
            }; 
    
            return await Post<TestResultsView, TestResultsViewReply>(request, tppUserSession.Suid);
        }
        
        private async Task<TppApiObjectResponse<TResponse>> Post<TRequest, TResponse>(TRequest model, string suid = null) where TRequest : ITppRequest
        {
            model.ApplyConfig(_tppConfig);
            var authenticateXml = model.SerializeXml();
            var authenticateContent = new StringContent(authenticateXml, Encoding.UTF8, TppHttpClient.MediaType);
            var request = BuildTppRequest(HttpMethod.Post, model.RequestType, authenticateContent, suid);
            
            var response = await SendRequestAndParseResponse<TResponse>(request);

            return response;
        }

        private async Task<TppApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new TppApiObjectResponse<TResponse>(responseMessage.StatusCode);

            if (!response.HasSuccessResponse) return response;

            var stringResponse = responseMessage.Content != null
                ? await responseMessage.Content.ReadAsStringAsync()
                : null;
            
            if (string.IsNullOrEmpty(stringResponse)) return response;

            try
            {
                if (IsErrorResponse(stringResponse))
                {
                    response.ErrorResponse = _responseParser.ParseBody<Error>(stringResponse, responseMessage);

                    _logger.LogError(
                        $"Server returned with error. {response.ErrorResponse?.UserFriendlyMessage}. {response.ErrorResponse?.TechnicalMessage}");

                    return response;
                }

                response.Body = _responseParser.ParseBody<TResponse>(stringResponse, responseMessage);
            }
            catch (FormatException e)
            {
                _logger
                    .LogError(e, "An error occured while parsing the response");
                return new TppApiObjectResponse<TResponse>(HttpStatusCode.InternalServerError);
            }

            if (responseMessage.Headers.TryGetValues(ResponseSuidHeader, out var values))
            {
                response.Headers = new Dictionary<string, string>
                {
                    { ResponseSuidHeader, values.First() }
                };
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
                UnitId = tppUserSession.UnitId,
            };

            var response =
                await Post<ListRepeatMedication, ListRepeatMedicationReply>(listRepeatMedication, tppUserSession.Suid);

            return response;
        }
        
        private static HttpRequestMessage BuildTppRequest(HttpMethod method, string requestType, StringContent stringContent, string suid = null)
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

            if (!String.IsNullOrEmpty(suid))
            {
                requestMessage.Headers.Add(RequestSuidHeader, suid);
            }

            return requestMessage;
        }

        private bool IsErrorResponse(string responseString)
        {
            return ErrorRegex.IsMatch(responseString);
        }

        public async Task<TppApiObjectResponse<ListSlotsReply>> ListSlotsPost(ListSlots model, string suid)
        {
            return await Post<ListSlots, ListSlotsReply>(model, suid);
        }

        public class TppApiResponse
        {
            public TppApiResponse(HttpStatusCode status)
            {
                StatusCode = status;
            }

            public Error ErrorResponse { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public bool HasSuccessResponse => ErrorResponse == null && StatusCode.IsSuccessStatusCode();

            public bool HasErrorWithMessage(string message)
            {
                return ErrorResponse?.TechnicalMessage == message;
            }
            
            // User does not have access, Sean to confirm with TPP re using error codes
            public bool HasForbiddenResponse => ErrorResponse != null && ErrorResponse.ErrorCode == TppApiErrorCodes.NoAccess;
        }

        public class TppApiObjectResponse<TBody> : TppApiResponse
        {
            public TppApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {
            }

            public TBody Body { get; set; }
            public Dictionary<string, string> Headers { get; set; }
        }
    }
}