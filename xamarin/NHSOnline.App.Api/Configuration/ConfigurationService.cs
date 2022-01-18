using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Client;
using NHSOnline.App.Api.Client.Configuration;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Polly;

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

        public async Task<GetConfigurationResult> GetConfiguration(int maxAttempts, CancellationToken token)
        {
            try
            {
                var request = new ApiConfigurationRequest();
                var retry = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(maxAttempts - 1, i => TimeSpan.FromSeconds(2));

                return await retry.ExecuteAsync(async cancellationToken =>
                {
                    var result = await _endpoint.Call(request, token).ResumeOnThreadPool();

                    if (result is ApiGetConfigurationResult.Success)
                    {
                        return result.Accept(this);
                    }
                    
                    throw new GetConfigurationException("Get Configuration Failed");

                }, token).ResumeOnThreadPool();
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
            _logger.LogInformation("Get Configuration Success");
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