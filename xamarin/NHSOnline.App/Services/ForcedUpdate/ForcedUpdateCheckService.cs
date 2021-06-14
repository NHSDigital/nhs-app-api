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
        private readonly INativeMinimumVersionCheck _nativeMinimumVersionCheck;
        private readonly IConfigurationService _configurationService;
        private Task<UpdateRequired>? _task;

        public ForcedUpdateCheckService(
            ILogger<ForcedUpdateCheckService> logger,
            IConfigurationService configurationService,
            INativeMinimumVersionCheck nativeMinimumVersionCheck)
        {
            _logger = logger;
            _nativeMinimumVersionCheck = nativeMinimumVersionCheck;
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

        private async Task<UpdateRequired> IsUpdateRequiredCheck()
        {
            try
            {
                var configurationResult = await _configurationService.GetConfiguration().ConfigureAwait(true);

                var updateRequired = configurationResult.Accept(new AssessUpdateRequiredVisitor(_nativeMinimumVersionCheck));

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