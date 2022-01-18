using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client.Configuration
{
    internal sealed class GetConfigurationResponseParser : IApiClientResponseParser<ApiGetConfigurationResult>
    {
        private readonly ILogger _logger;
        private readonly JsonResponseParser _jsonResponseParser;
        private readonly IResponseModelValidator<GetConfigurationResponseModel, GetConfigurationResponse> _responseModelValidator;

        public GetConfigurationResponseParser(
            ILogger<GetConfigurationResponseParser> logger,
            JsonResponseParser jsonResponseParser,
            IResponseModelValidator<GetConfigurationResponseModel, GetConfigurationResponse> responseModelValidator)
        {
            _logger = logger;
            _jsonResponseParser = jsonResponseParser;
            _responseModelValidator = responseModelValidator;
        }

        public async Task<ApiGetConfigurationResult> Parse(HttpResponseMessage httpResponseMessage)
        {
            var handler = httpResponseMessage.StatusCode switch
            {
                HttpStatusCode.OK => HandleOk(httpResponseMessage),
                HttpStatusCode.BadRequest => HandleBadRequest(),
                HttpStatusCode.InternalServerError => HandleInternalServerError(),
                _ => HandleUnknownStatusCode(httpResponseMessage)
            };

            return await handler.ResumeOnThreadPool();
        }

        private async Task<ApiGetConfigurationResult> HandleOk(
            HttpResponseMessage httpResponseMessage)
        {
            var model = await _jsonResponseParser.Parse<GetConfigurationResponseModel>(httpResponseMessage).ResumeOnThreadPool();
            var validationResult = _responseModelValidator.Validate(model);

            return validationResult.Accept<ApiGetConfigurationResult>(
                response => new ApiGetConfigurationResult.Success(response),
                () => new ApiGetConfigurationResult.Failure());
        }

        private Task<ApiGetConfigurationResult> HandleBadRequest()
        {
            _logger.LogWarning("Get Configuration returned bad request");
            return Task.FromResult<ApiGetConfigurationResult>(new ApiGetConfigurationResult.BadRequest());
        }

        private Task<ApiGetConfigurationResult> HandleInternalServerError()
        {
            _logger.LogWarning("Get Configuration returned internal server error");
            return Task.FromResult<ApiGetConfigurationResult>(new ApiGetConfigurationResult.Failure());
        }

        private Task<ApiGetConfigurationResult> HandleUnknownStatusCode(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning("Get Configuration returned HTTP status {UnexpectedHttpStatus}", httpResponseMessage.StatusCode);
            return Task.FromResult<ApiGetConfigurationResult>(new ApiGetConfigurationResult.Failure());
        }
    }
}