using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute
{
    public sealed class ComputeExecutor<TRequest> : IComputeExecutor<TRequest>
    {
        private readonly RequestParser<TRequest> _parser;

        public ComputeExecutor(RequestParser<TRequest> parser)
        {
            _parser = parser;
        }

        public async Task Execute(
            ILogger logger,
            CloudQueueMessage message,
            CloudQueue queue,
            CloudQueue poisonQueue,
            Func<TRequest, Task> compute)
        {
            logger.LogEnter();
            try
            {
                var request = await _parser.Parse(message);

                await queue.UpdateMessageAsync(message, TimeSpan.FromHours(1), MessageUpdateFields.Visibility);

                await compute(request);
            }
            catch (PermanentException e)
            {
                logger.LogMethodFailure(e);
                logger.LogInformation("Moving message to poison queue {PoisonQueueName}", poisonQueue.Name);
                await poisonQueue.AddMessageAsync(new CloudQueueMessage(message.AsBytes));
            }
            catch (Exception e)
            {
                logger.LogMethodFailure(e);
                throw;
            }
            finally
            {
                logger.LogExit();
            }
        }
    }
}
