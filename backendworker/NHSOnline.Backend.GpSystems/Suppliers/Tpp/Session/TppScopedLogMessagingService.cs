using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    internal sealed class TppScopedLogMessagingService : ITppLogMessagingService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public TppScopedLogMessagingService(ILogger<TppScopedLogMessagingService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void FetchAndLogAccessInformation(TppUserSession userSession)
        {
            Task.Factory
                .StartNew(RunInScope, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                .Unwrap()
                .ContinueWith(LogException, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);

            async Task RunInScope()
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var tppAccessInformationService = scope.ServiceProvider.GetRequiredService<TppLogMessagingService>();
                    await tppAccessInformationService.FetchAndLogAccessInformation(userSession);
                }
            }

            void LogException(Task task) => _logger.LogError(task.Exception, "Failed request to get list of service accesses");
        }
    }
}