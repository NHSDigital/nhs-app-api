using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Logging;

namespace NHSOnline.MetricLogFunctionApp.Compute
{
    public interface IComputeExecutor<out TRequest>
    {
        public Task Execute(
            ILogger logger,
            CloudQueueMessage message,
            CloudQueue queue,
            CloudQueue poisonQueue,
            Func<TRequest, Task> compute);
    }
}