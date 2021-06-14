using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Client;
using NHSOnline.App.Api.Client.Configuration;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Configuration
{
    internal sealed class ConfigurationService : IConfigurationService,
        IApiGetConfigurationResultVisitor<GetConfigurationResult>
    {
        private readonly ILogger<ConfigurationService> _logger;
        private readonly IApiClientEndpoint<ApiConfigurationRequest, ApiGetConfigurationResult> _endpoint;

        public ConfigurationService(
            ILogger<ConfigurationService> logger,
            IApiClientEndpoint<ApiConfigurationRequest, ApiGetConfigurationResult> endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
        }

        public async Task<GetConfigurationResult> GetConfiguration()
        {
            try
            {
                var request = new ApiConfigurationRequest();
                var result = await _endpoint.Call(request).ResumeOnThreadPool();

                return result.Accept(this);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get Configuration Failed");
                return new GetConfigurationResult.Failed();
            }
        }

        public GetConfigurationResult Visit(ApiGetConfigurationResult.Success success)
        {
            var configuration = new VersionConfiguration(success.GetConfigurationResponse);
            _logger.LogInformation(
                $"Get Configuration Success. Min Version: {configuration.MinimumSupportedAndroidVersion}");
            return new GetConfigurationResult.Success(configuration);
        }

        public GetConfigurationResult Visit(ApiGetConfigurationResult.Failure failure)
        {
            _logger.LogError("Get Configuration Failed");
            return new GetConfigurationResult.Failed();
        }

        public GetConfigurationResult Visit(ApiGetConfigurationResult.BadRequest badRequest)
        {
            _logger.LogError("Get Configuration Failed");
            return new GetConfigurationResult.Failed();
        }
    }
}