using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Areas.TermsAndConditions.Models;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.TermsAndConditions
{
    [SuppressMessage("Microsoft.Globalization", "CA1309", Justification = "Method ‘Equals’ is not supported., Linux/9 documentdb-netcore-sdk/1.9.1")]
    public class TermsAndConditionsService : ITermsAndConditionsService, IDisposable
    {
        private const string DateFormat = "yyyy-MM-ddTHH:mm:ss.f'Z'";
        private readonly ILogger<TermsAndConditionsService> _logger;
        private readonly DocumentClient _client;
        private readonly Uri _collectionUri;
        private bool _disposed;
        private readonly ITermsAndConditionsConfig _termsConfig;
        private readonly DateTimeOffset _latestEffectiveDate;
        private readonly IAuditor _auditor;

        public TermsAndConditionsService(ITermsAndConditionsConfig termsConfig,
            IConfiguration appConfig, ILogger<TermsAndConditionsService> logger, IAuditor auditor)
        {
            _logger = logger;
            _termsConfig = termsConfig;
            _auditor = auditor;

            var latestEffectiveDateStr = appConfig.ConfigurationSettings().GetOrWarn(
                "CurrentTermsConditionsEffectiveDate",
                _logger);

            _latestEffectiveDate = DateTimeOffset.Parse(latestEffectiveDateStr, CultureInfo.InvariantCulture);

            _logger.LogDebug("Effective date {0}", _latestEffectiveDate.ToString(DateFormat, CultureInfo.InvariantCulture));

            if (_termsConfig.Stubbed) return;
            _client = new DocumentClient(_termsConfig.EndpointUri, _termsConfig.AuthKey);
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_termsConfig.DatabaseId, _termsConfig.CollectionName);
        }

        public async Task<TermsAndConditionsFetchConsentResult> FetchConsent(string nhsNumber)
        {
            _logger.LogEnter(nameof(FetchConsent));
            
            if (_termsConfig.Stubbed)
            {
                var response = new ConsentResponse
                {
                    ConsentGiven = true,
                    AnalyticsCookieAccepted = true,
                    UpdatedConsentRequired = false,
                };

                _logger.LogDebug("Exiting: {0} - patient consent found", nameof(FetchConsent));
                return new TermsAndConditionsFetchConsentResult.Success(response); 
            }

            try
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                var termsAndConditions = await GetTermsAndConditionsConsent(nhsNumber);

                if (termsAndConditions != null)
                {
                    _logger.LogInformation("Patient consent found for terms and conditions");
                    
                    //Updated consent required if date of last consent is prior to date of updated terms
                    var updatedConsentRequired = _latestEffectiveDate <= DateTimeOffset.Now
                                                 && _latestEffectiveDate > termsAndConditions.DateOfConsent;

                    var response = new ConsentResponse
                    {
                        ConsentGiven = termsAndConditions.ConsentGiven,
                        UpdatedConsentRequired = updatedConsentRequired,
                        AnalyticsCookieAccepted = termsAndConditions.AnalyticsCookieAccepted,
                    };
                    
                    _logger.LogDebug($"ConsentGiven: {termsAndConditions.ConsentGiven}, " +
                                     $"UpdatedConsentRequired: {updatedConsentRequired}, " +
                                     $"AnalyticsCookieAccepted: {termsAndConditions.AnalyticsCookieAccepted}");

                    _logger.LogExit(nameof(FetchConsent));
                    return new TermsAndConditionsFetchConsentResult.Success(response);
                }

                _logger.LogInformation("No patient consent exists for terms and conditions");
                _logger.LogExit(nameof(FetchConsent));
                return new TermsAndConditionsFetchConsentResult.NoConsentFound(new ConsentResponse());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unsuccessful request to fetch patient consent");
                _logger.LogExit(nameof(FetchConsent));
                return new TermsAndConditionsFetchConsentResult.FailureToFetchConsent();
            }             
        }

        public async Task<TermsAndConditionsRecordConsentResult> RecordConsent(string nhsNumber, string odsCode, ConsentRequest request, DateTimeOffset termsAndConditionsAcceptanceDate)
        {
            return request.UpdatingConsent
                ? await UpdateConsent(nhsNumber, request, termsAndConditionsAcceptanceDate)
                : await RecordInitialConsent(nhsNumber, odsCode, request, termsAndConditionsAcceptanceDate);
        }

        private async Task<TermsAndConditionsRecordConsentResult> RecordInitialConsent(string nhsNumber, string odsCode, ConsentRequest request,
            DateTimeOffset termsAndConditionsAcceptanceDate)
        {
            _logger.LogEnter(nameof(RecordInitialConsent));

            if (_termsConfig.Stubbed)
            {
                _logger.LogExit(nameof(RecordInitialConsent));
                return new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();
            }

            if (!request.AnalyticsCookieAccepted)
            {
                _logger.LogInformation("Recording user did not accept optional analytics cookies. OdsCode: {0}",
                    odsCode);
            }

            await _auditor.Audit(Constants.AuditingTitles.TermsAndConditionsAnalyticsCookieAcceptance,
                "Attempting to record analytics cookies acceptance - AnalyticsCookieAccepted={0}{1}", request.AnalyticsCookieAccepted,
                request.AnalyticsCookieAccepted ?
                    string.Format(CultureInfo.InvariantCulture, " at DateAnalyticsCookieAccepted={0:O}", termsAndConditionsAcceptanceDate)
                    : string.Empty);

            try
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                var termsAndConditions = new TermsAndConditionsRecord(nhsNumber, 
                    request.ConsentGiven, request.AnalyticsCookieAccepted, termsAndConditionsAcceptanceDate,
                            request.AnalyticsCookieAccepted ? termsAndConditionsAcceptanceDate : (DateTimeOffset?)null);

                await _client.CreateDocumentAsync(_collectionUri, termsAndConditions);

                _logger.LogExit(nameof(RecordInitialConsent));

                return new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unsuccessful request to record patient consent");
                _logger.LogExit(nameof(RecordInitialConsent));
                return new TermsAndConditionsRecordConsentResult.FailureToRecordConsent();
            }
        }

        private async Task<TermsAndConditionsRecordConsentResult> UpdateConsent(string nhsNumber, ConsentRequest request, DateTimeOffset termsAndConditionsConsentDate)
        {
            _logger.LogEnter(nameof(UpdateConsent));

            if (_termsConfig.Stubbed)
            {
                _logger.LogExit(nameof(UpdateConsent));
                return new TermsAndConditionsRecordConsentResult.UpdateConsentRecorded();
            }

            try
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                var termsAndConditions = await GetTermsAndConditionsConsent(nhsNumber);

                if (termsAndConditions != null)
                {
                    termsAndConditions.ConsentGiven = request.ConsentGiven;
                    termsAndConditions.DateOfConsent = termsAndConditionsConsentDate;

                    var docLink = string.Format(CultureInfo.InvariantCulture, "dbs/{0}/colls/{1}/docs/{2}",
                        _termsConfig.DatabaseId, _termsConfig.CollectionName, termsAndConditions.Id);

                    await _client.ReplaceDocumentAsync(docLink, termsAndConditions);

                    return new TermsAndConditionsRecordConsentResult.UpdateConsentRecorded();
                }

                _logger.LogExit(nameof(UpdateConsent));
                return new TermsAndConditionsRecordConsentResult.FailureToRecordConsent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unsuccessful request to update patient consent");
                _logger.LogExit(nameof(UpdateConsent));
                return new TermsAndConditionsRecordConsentResult.FailureToRecordConsent();
            }
        }

        private async Task<TermsAndConditionsRecord> GetTermsAndConditionsConsent(string nhsNumber)
        {
            var termsAndConditionsQuery =
                _client.CreateDocumentQuery<TermsAndConditionsRecord>(_collectionUri,
                        new FeedOptions { MaxItemCount = 1 })
                    .Where(x => x.NhsNumber == nhsNumber)
                    .OrderByDescending(x => x.DateOfConsent).AsDocumentQuery();

            return
                (await termsAndConditionsQuery.ExecuteNextAsync<TermsAndConditionsRecord>()).FirstOrDefault();
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
