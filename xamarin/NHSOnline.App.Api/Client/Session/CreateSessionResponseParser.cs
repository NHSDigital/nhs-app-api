using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Client.Errors;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client.Session
{
    internal sealed class CreateSessionResponseParser : IApiClientResponseParser<ApiCreateSessionResult>
    {
        private const HttpStatusCode OdsCodeNotSupportedOrNoNhsNumber = (HttpStatusCode)464;
        private const HttpStatusCode FailedAgeRequirement = (HttpStatusCode)465;

        private readonly ILogger _logger;
        private readonly JsonResponseParser _jsonResponseParser;
        private readonly IResponseModelValidator<UserSessionResponseModel, UserSessionResponse> _responseModelValidator;
        private readonly IResponseModelValidator<PfsErrorResponseModel, PfsErrorResponse> _errorResponseModelValidator;

        public CreateSessionResponseParser(
            ILogger<CreateSessionResponseParser> logger,
            JsonResponseParser jsonResponseParser,
            IResponseModelValidator<UserSessionResponseModel, UserSessionResponse> responseModelValidator,
            IResponseModelValidator<PfsErrorResponseModel, PfsErrorResponse> errorResponseModelValidator)
        {
            _logger = logger;
            _jsonResponseParser = jsonResponseParser;
            _responseModelValidator = responseModelValidator;
            _errorResponseModelValidator = errorResponseModelValidator;
        }

        public async Task<ApiCreateSessionResult> Parse(HttpResponseMessage httpResponseMessage)
        {
            var handler = httpResponseMessage.StatusCode switch
            {
                HttpStatusCode.Created => HandleCreated(httpResponseMessage),
                HttpStatusCode.BadRequest => HandleBadRequest(httpResponseMessage),
                OdsCodeNotSupportedOrNoNhsNumber => HandleOdsCodeNotSupportedOrNoNhsNumber(httpResponseMessage),
                FailedAgeRequirement => HandleFailedAgeRequirement(httpResponseMessage),
                HttpStatusCode.InternalServerError => HandleInternalServerError(),
                HttpStatusCode.BadGateway => HandleBadGateway(httpResponseMessage),
                HttpStatusCode.GatewayTimeout => HandleGatewayTimeout(httpResponseMessage),
                _ => HandleUnknownStatusCode(httpResponseMessage)
            };

            return await handler.ResumeOnThreadPool();
        }

        private async Task<ApiCreateSessionResult> HandleCreated(
            HttpResponseMessage httpResponseMessage)
        {
            var model = await _jsonResponseParser.Parse<UserSessionResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _responseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response =>
                {
                    var cookies = new CookieContainer();
                    if (httpResponseMessage.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders) &&
                        setCookieHeaders != null)
                    {
                        cookies.SetCookies(
                            httpResponseMessage.RequestMessage.RequestUri,
                            string.Join(",", setCookieHeaders));
                    }

                    return new ApiCreateSessionResult.Success(response, cookies);
                },
                () => new ApiCreateSessionResult.Failure());
        }

        private async Task<ApiCreateSessionResult> HandleBadRequest(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned bad bequest");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.BadRequest(response),
                () => new ApiCreateSessionResult.Failure());
        }

        private async Task<ApiCreateSessionResult> HandleOdsCodeNotSupportedOrNoNhsNumber(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned not supported or no NHS number");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.OdsCodeNotSupportedOrNoNhsNumber(response),
                () => new ApiCreateSessionResult.Failure());
        }

        private async Task<ApiCreateSessionResult> HandleFailedAgeRequirement(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned failed age requirement");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.FailedAgeRequirement(response),
                () => new ApiCreateSessionResult.Failure());
        }

        private Task<ApiCreateSessionResult> HandleInternalServerError()
        {
            _logger.LogWarning("Create Session returned internal server error");
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }

        private async Task<ApiCreateSessionResult> HandleBadGateway(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned bad gateway");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.BadResponseFromUpstreamSystem(response),
                () => new ApiCreateSessionResult.Failure());
        }

        private async Task<ApiCreateSessionResult> HandleGatewayTimeout(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned gateway timeout");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.UpstreamSystemTimeout(response),
                () => new ApiCreateSessionResult.Failure());
        }

        private Task<ApiCreateSessionResult> HandleUnknownStatusCode(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned expected HTTP status {UnexpectedHttpStatue}", httpResponseMessage.StatusCode);
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }
    }
}