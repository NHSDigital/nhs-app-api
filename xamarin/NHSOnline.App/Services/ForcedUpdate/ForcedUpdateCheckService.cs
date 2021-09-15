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
            _task = Task.Run(() => IsUpdateRequiredCheck(CreateTimeoutCancellationToken()));
        }

        private CancellationToken CreateTimeoutCancellationToken()
        {
            _taskCancellationToken?.Cancel();
            _taskCancellationToken?.Dispose();

            _taskCancellationToken = new CancellationTokenSource();
            return _taskCancellationToken.Token;
        }

        public async Task<UpdateRequired> RequiresForcedUpdate()
        {
            if (_task != null && _taskCancellationToken != null)
            {
                _taskCancellationToken.CancelAfter(ForcedUpdateRequestTimeout);
                return await _task.ResumeOnThreadPool();
            }

            throw new InvalidOperationException("Initiate not called before required update check");
        }

        public async Task OpenAppStoreUrl() => await _updateService.OpenAppStoreUrl().ResumeOnThreadPool();

        private async Task<UpdateRequired> IsUpdateRequiredCheck(CancellationToken token)
        {
            try
            {
                var configurationResult = await _configurationService.GetConfiguration(token).ResumeOnThreadPool();

                var updateRequired =
                    configurationResult.Accept(new AssessUpdateRequiredVisitor(_nativeAppVersionCheckService));

                _logger.LogInformation(message: $"Update Required Check finished. UpdateRequired:'{updateRequired}'");

                return updateRequired;
            }
            catch (TaskCanceledException taskCancelledException)
            {
                _logger.LogError(taskCancelledException, "Failed, task cancelled");
                return UpdateRequired.Failed;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed Update Required check");
                return UpdateRequired.Failed;
            }
        }

        public void Dispose()
        {
            _task?.Dispose();
            _taskCancellationToken?.Dispose();
        }
    }
}