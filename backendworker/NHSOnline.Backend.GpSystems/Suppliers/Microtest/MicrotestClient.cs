using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestClient : IMicrotestClient
    {
        public const string HeaderNhsNumber = "NHSO-Nhs-Number";
        public const string HeaderOdsCode = "NHSO-ODS-Code";
        private const string AppointmentSlotsPath = "patient/appointment-slots?fromDate={0}&toDate={1}";
        private const string AppointmentsPath = "patient/appointments";
        private const string DemographicsPath = "patient/demographics";

        private readonly MicrotestHttpClient _httpClient;
        private readonly ILogger<MicrotestClient> _logger;
        private readonly IJsonResponseParser _responseParser;

        public MicrotestClient(
            MicrotestHttpClient httpClient,
            ILoggerFactory loggerFactory,
            IJsonResponseParser responseParser)
        {
            _httpClient = httpClient;
            _logger = loggerFactory.CreateLogger<MicrotestClient>();
            _responseParser = responseParser;
        }

        public async Task<MicrotestApiObjectResponse<AppointmentSlotsGetResponse>> AppointmentSlotsGet(
            string odsCode,
            string nhsNumber,
            AppointmentSlotsDateRange dateRange)
        {
            _logger.LogEnter();

            var fromDateString = dateRange.FromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var toDateString = dateRange.ToDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            _logger.LogDebug($"fromDate: { fromDateString }, toDate: { toDateString }");

            var path = string.Format(
                CultureInfo.InvariantCulture,
                AppointmentSlotsPath,
                fromDateString,
                toDateString);

            var response = await Get<AppointmentSlotsGetResponse>(path, odsCode, nhsNumber);

            _logger.LogExit();
            return response;
        }
        
        public async Task<MicrotestApiObjectResponse<string>> AppointmentsPost(
            BookAppointmentSlotPostRequest bookAppointmentSlotPostRequest,
            MicrotestUserSession userSession)
        {
            _logger.LogEnter();
            _logger.LogDebug($"booking slot: { bookAppointmentSlotPostRequest.SlotId }");

            var response = await Post(bookAppointmentSlotPostRequest, AppointmentsPath, userSession.OdsCode, userSession.NhsNumber);

            _logger.LogExit();
            return response;
        }
        
        public async Task<MicrotestApiObjectResponse<string>> AppointmentsDelete(
            CancelAppointmentDeleteRequest cancelAppointmentDeleteRequest, MicrotestUserSession userSession)
        {
            _logger.LogEnter();
            _logger.LogDebug($"Cancelling slot: { cancelAppointmentDeleteRequest.AppointmentId }");

            var response = await Delete(cancelAppointmentDeleteRequest, AppointmentsPath, userSession.OdsCode, userSession.NhsNumber);

            _logger.LogExit();
            return response;
        }

        public async Task<MicrotestApiObjectResponse<AppointmentsGetResponse>> AppointmentsGet(
            string odsCode,
            string nhsNumber)
        {
            _logger.LogEnter();

            var response = await Get<AppointmentsGetResponse>(AppointmentsPath, odsCode, nhsNumber);

            _logger.LogExit();
            return response;
        }

        public async Task<MicrotestApiObjectResponse<DemographicsGetResponse>> DemographicsGet(
            string odsCode,
            string nhsNumber)
        {
            _logger.LogEnter();
            var response = await Get<DemographicsGetResponse>(DemographicsPath, odsCode, nhsNumber);

            _logger.LogExit();
            return response;
        }

        private async Task<MicrotestApiObjectResponse<TResponse>> Get<TResponse>(
            string path,
            string odsCode = null,
            string nhsNumber = null)
        {
            var request = BuildRequest(HttpMethod.Get, path, odsCode, nhsNumber);
            return await SendRequestAndParseResponse<TResponse>(request);
        }
        
        private async Task<MicrotestApiObjectResponse<string>> Post<TRequest>(
            TRequest requestBody,
            string path,
            string odsCode = null,
            string nhsNumber = null)
        {
            var request = BuildRequest(HttpMethod.Post, path, odsCode, nhsNumber);
            var requestBodyJson = JsonConvert.SerializeObject(requestBody);
            request.Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
            
            return await SendRequestAndGetResponse(request);
        }
        
        private async Task<MicrotestApiObjectResponse<string>> Delete<TRequest>(
            TRequest requestBody,
            string path,
            string odsCode = null,
            string nhsNumber = null)
        {
            var request = BuildRequest(HttpMethod.Delete, path, odsCode, nhsNumber);
            var requestBodyJson = JsonConvert.SerializeObject(requestBody);
            request.Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
            
            return await SendRequestAndGetResponse(request);
        }

        private static HttpRequestMessage BuildRequest(
            HttpMethod httpMethod,
            string path,
            string odsCode = null,
            string nhsNumber = null)
        {
            var request = new HttpRequestMessage(httpMethod, path);
            
            if (!string.IsNullOrEmpty(odsCode))
            {
                request.Headers.Add(HeaderOdsCode, new[] { odsCode });
            }
            
            if (!string.IsNullOrEmpty(nhsNumber))
            {
                request.Headers.Add(HeaderNhsNumber, new[] { nhsNumber });
            }

            return request;
        }
        
        private async Task<MicrotestApiObjectResponse<string>> SendRequestAndGetResponse(
            HttpRequestMessage request)
        {
            _logger.LogEnter();

            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new MicrotestApiObjectResponse<string>(responseMessage.StatusCode);

            if (!response.HasSuccessResponse)
            {
                response.ErrorResponseMessage = await response.GetStringResponse(responseMessage, _logger);
            }

            _logger.LogExit();
            return response;
        }

        private async Task<MicrotestApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new MicrotestApiObjectResponse<TResponse>(responseMessage.StatusCode);

            return await response.Parse(responseMessage, _responseParser, _logger);
        }

        public class MicrotestApiResponse : ApiResponse
        {
            public MicrotestApiResponse(HttpStatusCode statusCode) : base(statusCode)
            {
            }
            public string ErrorResponseMessage { get; set; }
            public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();
            public override string ErrorForLogging => ErrorResponseMessage;

            protected override bool FormatResponseIfUnsuccessful => false;
        }

        public class MicrotestApiObjectResponse<TBody> : MicrotestApiResponse
        {
            public MicrotestApiObjectResponse(HttpStatusCode statusCode) : base(statusCode) { }

            public TBody Body { get; set; }

            public async Task<MicrotestApiObjectResponse<TBody>> Parse(
                HttpResponseMessage responseMessage,
                IJsonResponseParser responseParser,
                ILogger logger)
            {
                var stringResponse = await GetStringResponse(responseMessage, logger);
                return string.IsNullOrEmpty(stringResponse)
                    ? this
                    : ParseResponse(responseParser,
                        stringResponse,
                        responseMessage,
                        logger);
            }

            private MicrotestApiObjectResponse<TBody> ParseResponse(
                IResponseParser responseParser,
                string stringResponse,
                HttpResponseMessage responseMessage,
                ILogger logger)
            {

                Body = responseParser.ParseBody<TBody>(stringResponse, responseMessage);
                
                if (Body == null)
                { 
                    logger.LogError($"Response parsing failed. Raw response: {stringResponse}");
                    return new MicrotestApiObjectResponse<TBody>(HttpStatusCode.InternalServerError);
                }
                return this;
            }
        }
    }
}