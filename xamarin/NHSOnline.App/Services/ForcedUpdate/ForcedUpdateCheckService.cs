using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Configuration;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.ForcedUpdate
{
    internal class ForcedUpdateCheckService : IForcedUpdateCheckService, IDisposable
    {
        private readonly ILogger<ForcedUpdateCheckService> _logger;
        private readonly INativeAppVersionCheckService _nativeAppVersionCheckService;
        private readonly IConfigurationService _configurationService;
        private readonly IUpdateService _updateService;
        private Task<UpdateRequired>? _task;
        private CancellationTokenSource? _taskCancellationToken;
        private static readonly TimeSpan ForcedUpdateRequestTimeout = TimeSpan.FromSeconds(10);

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
            _task = Task.Run(() => IsUpdateRequiredCheck(10, CreateTimeoutCancellationToken()));
        }

        public async Task<UpdateRequired> RequiresForcedUpdate()
        {
            if (_task == null)
            {
                _task = IsUpdateRequiredCheck(1, CreateTimeoutCancellationToken());
            }

            var taskResult = await _task.ResumeOnThreadPool();
            if (_task.Status == TaskStatus.RanToCompletion && taskResult == UpdateRequired.Failed)
            {
                return await IsUpdateRequiredCheck(1, CreateTimeoutCancellationToken()).ResumeOnThreadPool();
            }

            if (_task != null && _taskCancellationToken != null)
            {
                _taskCancellationToken.CancelAfter(ForcedUpdateRequestTimeout);
                return taskResult;
            }

            throw new InvalidOperationException("Initiate not called before required update check");
        }

        public async Task OpenAppStoreUrl() => await _updateService.OpenAppStoreUrl().ResumeOnThreadPool();

        private async Task<UpdateRequired> IsUpdateRequiredCheck(int maxAttempts, CancellationToken token)
        {
            try
            {
                var configurationResult = await _configurationService.GetConfiguration(maxAttempts, token).ResumeOnThreadPool();

                var updateRequired =
                    configurationResult.Accept(new AssessUpdateRequiredVisitor(_nativeAppVersionCheckService));

                _logger.LogInformation("Update Required Check finished. updateRequired: '{UpdateRequired}'", updateRequired);

                return updateRequired;
            }
            catch (TaskCanceledException e)
            {
                _logger.LogError(e, "Failed, task cancelled likely due to running longer than configured timeout");
                return UpdateRequired.Failed;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed Update Required check");
                return UpdateRequired.Failed;
            }
        }

        private CancellationToken CreateTimeoutCancellationToken()
        {
            _taskCancellationToken?.Cancel();
            _taskCancellationToken?.Dispose();

            _taskCancellationToken = new CancellationTokenSource();
            return _taskCancellationToken.Token;
        }

        public void Dispose()
        {
            _task?.Dispose();
            _taskCancellationToken?.Dispose();
        }
    }
}