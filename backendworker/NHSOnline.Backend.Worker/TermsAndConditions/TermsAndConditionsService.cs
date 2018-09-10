using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.TermsAndConditions.Models;

namespace NHSOnline.Backend.Worker.TermsAndConditions
{
    #pragma warning disable CA1309  
    [CLSCompliant(false)]  // CA1309  
    public class TermsAndConditionsService : ITermsAndConditionsService, IDisposable
    {
        private readonly ILogger<TermsAndConditionsService> _logger;
        private readonly DocumentClient _client;
        private readonly Uri _collectionUri;
        private bool _disposed;
        private readonly ITermsAndConditionsConfig _configuration;
               
        public TermsAndConditionsService(ITermsAndConditionsConfig configuration, ILogger<TermsAndConditionsService> logger)
        {
            _logger = logger;
            _configuration = configuration;

            if (_configuration.Stubbed) return;
            _client = new DocumentClient(_configuration.EndpointUri, _configuration.AuthKey);
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseId, _configuration.CollectionName);   
        }

        public async Task<TermsAndConditionsFetchConsentResult> FetchConsent(string nhsNumber)
        {
            const string methodName = "FetchConsent";
            _logger.LogDebug("Entered: {0}", methodName);
            if (_configuration.Stubbed)
            {
                var response = new ConsentResponse { ConsentGiven = true };
                _logger.LogDebug("Exiting: {0} - patient consent found", methodName); 
                return new TermsAndConditionsFetchConsentResult.Success(response); 
            } 
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
                _logger.LogDebug("Exiting: {0} - no existing patient consent record", methodName); 
                return new TermsAndConditionsFetchConsentResult.NoConsentFound();
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
            const string  methodName = "RecordConsent";
            _logger.LogDebug("Entered: {0}", methodName);
            
            if (_configuration.Stubbed)
            {
                _logger.LogDebug("Exiting: {0}", methodName);
                return new TermsAndConditionsRecordConsentResult.ConsentRecorded();
            }
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
