using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client.Session
{
    internal sealed class CreateSessionResponseParser : IApiClientResponseParser<ApiCreateSessionResult>
    {
        private const HttpStatusCode OdsCodeNotSupportedOrNoNhsNumber = (HttpStatusCode)464;
        private const HttpStatusCode FailedAgeRequirement = (HttpStatusCode)465;

        private readonly ILogger _logger;
        private readonly JsonResponseParser _jsonResponseParser;

        public CreateSessionResponseParser(ILogger<CreateSessionResponseParser> logger, JsonResponseParser jsonResponseParser)
        {
            _logger = logger;
            _jsonResponseParser = jsonResponseParser;
        }

        public async Task<ApiCreateSessionResult> Parse(
            HttpResponseMessage httpResponseMessage,
            CookieContainer cookies)
        {
            var handler = httpResponseMessage.StatusCode switch
            {
                HttpStatusCode.Created => HandleCreated(httpResponseMessage, cookies),
                HttpStatusCode.BadRequest => HandleBadRequest(),
                HttpStatusCode.Forbidden => HandleForbidden(),
                OdsCodeNotSupportedOrNoNhsNumber => HandleOdsCodeNotSupportedOrNoNhsNumber(),
                FailedAgeRequirement => HandleFailedAgeRequirement(),
                HttpStatusCode.InternalServerError => HandleInternalServerError(),
                HttpStatusCode.BadGateway => HandleBadGateway(),
                HttpStatusCode.GatewayTimeout => HandleGatewayTimeout(),
                _ => HandleUnknownStatusCode(httpResponseMessage)
            };

            return await handler.ResumeOnThreadPool();
        }

        private async Task<ApiCreateSessionResult> HandleCreated(
            HttpResponseMessage httpResponseMessage,
            CookieContainer cookies)
        {
            var responseModel = await _jsonResponseParser.Parse<UserSessionResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var createSessionResponse = new ApiCreateSessionResponse(responseModel, cookies);
            return new ApiCreateSessionResult.Success(createSessionResponse);
        }

        private Task<ApiCreateSessionResult> HandleBadRequest()
        {
            _logger.LogWarning("Create Session returned bad bequest");
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }

        private Task<ApiCreateSessionResult> HandleForbidden()
        {
            _logger.LogWarning("Create Session returned forbidden");
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }

        private Task<ApiCreateSessionResult> HandleOdsCodeNotSupportedOrNoNhsNumber()
        {
            _logger.LogWarning("Create Session returned not supported or no NHS number");
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }

        private Task<ApiCreateSessionResult> HandleFailedAgeRequirement()
        {
            _logger.LogWarning("Create Session returned failed age requirement");
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }

        private Task<ApiCreateSessionResult> HandleInternalServerError()
        {
            _logger.LogWarning("Create Session returned internal server error");
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }

        private Task<ApiCreateSessionResult> HandleBadGateway()
        {
            _logger.LogWarning("Create Session returned bad gateway");
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }

        private Task<ApiCreateSessionResult> HandleGatewayTimeout()
        {
            _logger.LogWarning("Create Session returned gateway timeout");
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }

        private Task<ApiCreateSessionResult> HandleUnknownStatusCode(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned expected HTTP status {UnexpectedHttpStatue}", httpResponseMessage.StatusCode);
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }
    }
}