using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.TermsAndConditions.Models;
using Microsoft.Azure.Documents.Client;

namespace NHSOnline.Backend.Worker.Support.TermsAndConditions
{
    public class TermsAndConditionsService : ITermsAndConditionsService, IDisposable
    {
        private ILogger<TermsAndConditionsService> _logger;
        private DocumentClient _client;
        private Uri _collectionUri;
        private bool _disposed;
        private ITermsAndConditionsConfig _configuration;
               
        public TermsAndConditionsService(ITermsAndConditionsConfig configuration, ILogger<TermsAndConditionsService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _client = new DocumentClient(_configuration.EndpointUri, _configuration.AuthKey);
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseId, _configuration.CollectionName);
        }
        
        public async Task<TermsAndConditionsRecordConsentResult> RecordConsent(string nhsNumber, ConsentRequest request)
        {
            var methodName = "RecordConsent";
            _logger.LogDebug("Entered: {0}", methodName);
            
            var auditRecord = new TermsAndConditionsRecord(nhsNumber, request.ConsentGiven, request.DateOfConsent);
        
            try
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                
                await _client.CreateDocumentAsync(_collectionUri, auditRecord);
                _logger.LogDebug("Exiting: {0}", methodName); 
                return new TermsAndConditionsRecordConsentResult.ConsentRecorded();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unsuccessful request to record patient consent");
                _logger.LogDebug("Exiting: {0}", methodName); 
                return new TermsAndConditionsRecordConsentResult.FailureToRecordConsent();
            }         
        }      

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TermsAndConditionsService()
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
