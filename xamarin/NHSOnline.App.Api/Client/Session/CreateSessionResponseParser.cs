using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Client.Cookies;
using NHSOnline.App.Api.Client.Errors;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client.Session
{
    internal sealed class CreateSessionResponseParser : IApiClientResponseParser<ApiCreateSessionResult>
    {
        private const HttpStatusCode OdsCodeNotSupported = (HttpStatusCode)464;
        private const HttpStatusCode OdsCodeNotFound = (HttpStatusCode)468;
        private const HttpStatusCode NoNhsNumber = (HttpStatusCode)469;
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
                OdsCodeNotSupported => HandleOdsCodeNotSupported(httpResponseMessage),
                OdsCodeNotFound => HandleOdsCodeNotFound(httpResponseMessage),
                NoNhsNumber => HandleNoNhsNumber(httpResponseMessage),
                FailedAgeRequirement => HandleFailedAgeRequirement(httpResponseMessage),
                HttpStatusCode.InternalServerError => HandleInternalServerError(httpResponseMessage),
                HttpStatusCode.BadGateway => HandleBadGateway(httpResponseMessage),
                HttpStatusCode.GatewayTimeout => HandleGatewayTimeout(httpResponseMessage),
                _ => HandleUnknownStatusCode(httpResponseMessage)
            };

            return await handler.ResumeOnThreadPool();
        }

        private async Task<ApiCreateSessionResult> HandleCreated(HttpResponseMessage httpResponseMessage)
        {
            var model = await _jsonResponseParser.Parse<UserSessionResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _responseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response =>
                {
                    var cookies = new CookieJar();
                    if (httpResponseMessage.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders) &&
                        setCookieHeaders != null)
                    {
                        foreach (var cookieHeader in setCookieHeaders)
                        {
                            try
                            {
                                cookies.Add(new ApiCookie(httpResponseMessage.RequestMessage.RequestUri, cookieHeader));
                            }
                            catch (Exception exception)
                            {
                                _logger.LogError(exception,"Failed to add cookie from session response to CookieJar");
                            }
                        }
                    }

                    return new ApiCreateSessionResult.Success(response, cookies);
                },
                () => new ApiCreateSessionResult.Failure());
        }

        private async Task<ApiCreateSessionResult> HandleBadRequest(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned bad request");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.BadRequest(response),
                () => new ApiCreateSessionResult.Failure());
        }

        private async Task<ApiCreateSessionResult> HandleOdsCodeNotSupported(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned Odscode not supported");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.OdsCodeNotSupported(response),
                () => new ApiCreateSessionResult.Failure());
        }

        private async Task<ApiCreateSessionResult> HandleOdsCodeNotFound(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned Odscode not found");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.OdsCodeNotFound(response),
                () => new ApiCreateSessionResult.Failure());
        }

        private async Task<ApiCreateSessionResult> HandleNoNhsNumber(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned Nhs number not found");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.NoNhsNumber(response),
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

        private async Task<ApiCreateSessionResult> HandleInternalServerError(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Create Session returned internal server error");

            var model = await _jsonResponseParser.Parse<PfsErrorResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _errorResponseModelValidator.Validate(model);

            return validationResult.Accept<ApiCreateSessionResult>(
                response => new ApiCreateSessionResult.InternalServerError(response),
                () => new ApiCreateSessionResult.Failure());
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