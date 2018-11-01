using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Worker.ResponseParsers;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest
{
    public class MicrotestClient : IMicrotestClient
    {
        public const string HeaderNhsNumber = "NHSO-Nhs-Number";
        public const string HeaderOdsCode = "NHSO-ODS-Code";
        private const string AppointmentSlotsPath = "patient/appointment-slots?fromDate={0}&toDate={1}";
        
        private readonly MicrotestHttpClient _httpClient;
        private readonly ILogger<MicrotestClient> _logger;
        private readonly IResponseParser _responseParser;

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
            var fromDateString = dateRange.FromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var toDateString = dateRange.ToDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            _logger.LogDebug($"fromDate: { fromDateString }, toDate: { toDateString }");

            var path = string.Format(
                CultureInfo.InvariantCulture,
                AppointmentSlotsPath,
                fromDateString,
                toDateString);

            var response = await Get<AppointmentSlotsGetResponse>(path, odsCode, nhsNumber);
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

        private async Task<MicrotestApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var methodName = nameof(SendRequestAndParseResponse);
            _logger.LogDebug("Entered: {0}", methodName);

            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new MicrotestApiObjectResponse<TResponse>(responseMessage.StatusCode);

            var stringResponse = responseMessage.Content != null
                ? await responseMessage.Content.ReadAsStringAsync()
                : null;

            if (string.IsNullOrEmpty(stringResponse)) return response;

            response.Body = _responseParser.ParseBody<TResponse>(stringResponse, responseMessage);
            
            _logger.LogDebug("Exiting: {0}", methodName);
            return response;
        }

        public class MicrotestApiResponse : ApiResponse
        {
            public MicrotestApiResponse(HttpStatusCode statusCode) : base(statusCode)
            {
            }

            public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

            public override string ErrorForLogging => StatusCode.ToString();

            protected override bool FormatResponseIfUnsuccessful => false;
        }

        public class MicrotestApiObjectResponse<TBody> : MicrotestApiResponse
        {
            public MicrotestApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {
            }

            public TBody Body { get; set; }
        }
    }
}