using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public interface IFireAndForgetService
    {
        void Run(Func<IServiceProvider, Task> action, string errorMessage);
    }

    public class FireAndForgetService : IFireAndForgetService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public FireAndForgetService(IServiceProvider serviceProvider, ILogger<FireAndForgetService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Run(Func<IServiceProvider, Task> action, string errorMessage)
        {
            Task.Factory
                .StartNew(RunInScope, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                .Unwrap()
                .ContinueWith(LogError, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);

            async Task RunInScope()
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    await action(scope.ServiceProvider);
                }
            }

            void LogError(Task task)
            {
                _logger.LogError(task.Exception, errorMessage);
            }
        }
    }
}
