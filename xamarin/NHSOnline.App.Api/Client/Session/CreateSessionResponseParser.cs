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
                HttpStatusCode.BadRequest => HandleBadRequest(),
                HttpStatusCode.Forbidden => HandleForbidden(httpResponseMessage),
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

        private Task<ApiCreateSessionResult> HandleBadRequest()
        {
            _logger.LogWarning("Create Session returned bad bequest");
            return Task.FromResult<ApiCreateSessionResult>(new ApiCreateSessionResult.Failure());
        }

        private async Task<ApiCreateSessionResult> HandleForbidden(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned forbidden");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.Forbidden(response),
                () => new ApiCreateSessionResult.Failure());
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