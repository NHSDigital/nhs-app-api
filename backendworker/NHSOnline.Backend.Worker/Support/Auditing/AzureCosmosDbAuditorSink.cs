using System;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class AzureCosmosDbAuditorSink : IAuditSink, IDisposable
    {
        private readonly ILogger<AzureCosmosDbAuditorSink> _logger;
        private readonly DocumentClient _client;
        private readonly Uri _auditCollectionUri;
        private bool _disposed;

        public AzureCosmosDbAuditorSink(
            IAzureCosmosDbAuditorSinkConfig config, 
            ILogger<AzureCosmosDbAuditorSink> logger)
        {
            _logger = logger;

            _client = new DocumentClient(config.CosmosDbSinkUri, config.CosmosDbSinkKey);

            _auditCollectionUri =
                UriFactory.CreateDocumentCollectionUri(config.CosmosDbSinkDatabaseId, config.CosmosDbSinkCollectionId);
        }

        public void WriteAudit(DateTime timestamp, string nhsNumber, Supplier supplier, string operation, string details)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            
            _logger.LogEnter(nameof(WriteAudit));

            var auditRecord = new AuditRecord(timestamp, nhsNumber, supplier, operation, details);

            _client.CreateDocumentAsync(_auditCollectionUri, auditRecord).Wait();
            
            _logger.LogExit(nameof(WriteAudit));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AzureCosmosDbAuditorSink()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _client.Dispose();
            }

            _disposed = true;
        }
    }
}
