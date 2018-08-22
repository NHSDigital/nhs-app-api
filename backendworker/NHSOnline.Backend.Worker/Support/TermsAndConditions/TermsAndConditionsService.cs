using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.TermsAndConditions.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace NHSOnline.Backend.Worker.Support.TermsAndConditions
{
    #pragma warning disable CA1309  
    [CLSCompliant(false)]  // CA1309  
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

        public async Task<TermsAndConditionsFetchConsentResult> FetchConsent(string nhsNumber)
        {
            var methodName = "FetchConsent";
            _logger.LogDebug("Entered: {0}", methodName);
                  
            try
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                
                var docQuery = _client.CreateDocumentQuery<TermsAndConditionsRecord>(_collectionUri, new FeedOptions {MaxItemCount = 1})
                    .Where(x => x.NhsNumber == nhsNumber)
                    .OrderByDescending(x => x.DateOfConsent)
                    .AsDocumentQuery();
                
                var response = new ConsentResponse();
                var matchFound = false;
                
                while (!matchFound && docQuery.HasMoreResults)
                {
                    var records = await docQuery.ExecuteNextAsync<TermsAndConditionsRecord>();

                    if (records.Count > 0)
                    {
                        var record = records.ElementAt(0);
                        response.ConsentGiven = record.ConsentGiven;
                        matchFound = true;
                    }
                }

                if (matchFound)
                {           
                    _logger.LogDebug("Exiting: {0} - patient consent found", methodName); 
                    return new TermsAndConditionsFetchConsentResult.Success(response);                    
                }
                else
                {               
                    _logger.LogDebug("Exiting: {0} - no existing patient consent record", methodName); 
                    return new TermsAndConditionsFetchConsentResult.NoConsentFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unsuccessful request to record patient consent");
                _logger.LogDebug("Exiting: {0}", methodName); 
                return new TermsAndConditionsFetchConsentResult.FailureToFetchConsent();
            }           
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
