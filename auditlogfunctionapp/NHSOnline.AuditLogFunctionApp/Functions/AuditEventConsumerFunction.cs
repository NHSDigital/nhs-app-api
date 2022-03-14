using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.AuditLogFunctionApp.Model;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace NHSOnline.AuditLogFunctionApp.Functions
{
    public class AuditEventConsumerFunction
    {
        private readonly ILogger<AuditEventConsumerFunction> _logger;

        public AuditEventConsumerFunction(ILogger<AuditEventConsumerFunction> logger)
        {
            _logger = logger;
        }

        [FunctionName(nameof(AuditEventConsumerTrigger))]
        public async Task AuditEventConsumerTrigger(
            [EventHubTrigger(
                "%AuditEventHubName%",
                Connection = "AuditEventHubConnectionDataReceiver",
                ConsumerGroup = "%AuditEventHubConsumerGroup%"
            )]
            AuditRecord[] auditRecords,
            [CosmosDB(
                "%AuditCosmosSQLDbName%",
                "%AuditCosmosSQLDbContainer%",
                ConnectionStringSetting = "AuditCosmosSQLDbConnectionString"
            )]
            IAsyncCollector<AuditRecord> auditCollection
        )
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "{} function started. recordCount={}",
                nameof(AuditEventConsumerTrigger),
                auditRecords.Length
            );

            try
            {
                await Task.WhenAll(
                    auditRecords.AsParallel().Select(r =>
                    {
                        r.Id = r.AuditId;
                        return auditCollection.AddAsync(r);
                    }).ToArray()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to write audit item to database due to exception"
                );

                throw;
            }

            await auditCollection.FlushAsync();

            stopwatch.Stop();

            _logger.LogInformation(
                "{} function finished. durationInMs={} recordCount={}",
                nameof(AuditEventConsumerTrigger),
                stopwatch.ElapsedMilliseconds,
                auditRecords.Length
            );
        }
    }
}
