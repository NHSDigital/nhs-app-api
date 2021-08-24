using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Configuration;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.ForcedUpdate
{
    internal class ForcedUpdateCheckService : IForcedUpdateCheckService
    {
        private readonly ILogger<ForcedUpdateCheckService> _logger;
        private readonly INativeAppVersionCheckService _nativeAppVersionCheckService;
        private readonly IConfigurationService _configurationService;
        private readonly IUpdateService _updateService;
        private Task<UpdateRequired>? _task;

        public ForcedUpdateCheckService(
            ILogger<ForcedUpdateCheckService> logger,
            IConfigurationService configurationService,
            INativeAppVersionCheckService nativeAppVersionCheckService,
            IUpdateService updateService)
        {
            _logger = logger;
            _nativeAppVersionCheckService = nativeAppVersionCheckService;
            _updateService = updateService;
            _configurationService = configurationService;
        }

        public void Initiate()
        {
            _task = Task.Run(IsUpdateRequiredCheck);
        }

        public async Task<UpdateRequired> RequiresForcedUpdate()
        {
            if (_task != null)
            {
                return await _task.ResumeOnThreadPool();
            }

            throw new InvalidOperationException("Initiate not called before required update check");
        }

        public async Task OpenAppStoreUrl() => await _updateService.OpenAppStoreUrl().PreserveThreadContext();

        private async Task<UpdateRequired> IsUpdateRequiredCheck()
        {
            try
            {
                var configurationResult = await _configurationService.GetConfiguration().PreserveThreadContext();

                var updateRequired = configurationResult.Accept(new AssessUpdateRequiredVisitor(_nativeAppVersionCheckService));

                _logger.LogInformation(message: $"Update Required Check finished. UpdateRequired:'{updateRequired}'");

                return updateRequired;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed Update Required check");
                return UpdateRequired.Failed;
            }
        }
    }
}