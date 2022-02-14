using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NHSOnline.AuditLogFunctionApp.Model;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.AuditLogFunctionApp.Functions
{
    public class AuditEventConsumerFunction
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuditEventConsumerFunction> _logger;

        public AuditEventConsumerFunction(IConfiguration configuration, ILogger<AuditEventConsumerFunction> logger)
        {
            _configuration = configuration;
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
                ConnectionStringSetting = "AuditCosmosSQLDbConnectionString"
            )]
            IDocumentClient documentClient
        )
        {
            _logger.LogInformation(
                "{} function started. recordCount={}",
                nameof(AuditEventConsumerTrigger),
                auditRecords.Length
            );

            var databaseId = _configuration["AuditCosmosSQLDbName"];
            var containerId = _configuration["AuditCosmosSQLDbContainer"];

            foreach (var record in auditRecords)
            {
                record.Id = record.AuditId;

                try
                {
                    await documentClient.UpsertDocumentAsync(
                        UriFactory.CreateDocumentCollectionUri(databaseId, containerId),
                        record
                    );

                    _logger.LogInformation("Wrote audit item to database. auditId={}", record.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to write audit item to database due to exception. auditId={}",
                        record.Id
                    );

                    throw;
                }
            }
        }
    }
}
