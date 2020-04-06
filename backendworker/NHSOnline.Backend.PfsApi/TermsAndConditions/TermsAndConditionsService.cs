using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public class TermsAndConditionsService : ITermsAndConditionsService
    {
        private readonly ILogger<TermsAndConditionsService> _logger;
        private readonly ITermsAndConditionsRepository _repository;
        private readonly ITermsAndConditionsConfiguration _configuration;

        public TermsAndConditionsService
        (
            ILogger<TermsAndConditionsService> logger,
            ITermsAndConditionsRepository repository,
            ITermsAndConditionsConfiguration configuration
        )
        {
            _logger = logger;
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<TermsAndConditionsFetchConsentResult> FetchConsent(string nhsLoginId)
        {
            _logger.LogEnter();

            try
            {
                var termsAndConditions = await _repository.Find(nhsLoginId);

                if (termsAndConditions == null)
                {
                    _logger.LogInformation("No patient consent exists for terms and conditions");
                    return new TermsAndConditionsFetchConsentResult.NoConsentFound(new ConsentResponse());
                }

                _logger.LogInformation("Patient consent found for terms and conditions");

                //Updated consent required if date of last consent is prior to date of updated terms
                var updatedConsentRequired = _configuration.EffectiveDate <= DateTimeOffset.Now
                                             && _configuration.EffectiveDate > DateTimeOffset.Parse(
                                                 termsAndConditions.DateOfConsent, CultureInfo.InvariantCulture);

                var response = new ConsentResponse
                {
                    ConsentGiven = termsAndConditions.ConsentGiven,
                    UpdatedConsentRequired = updatedConsentRequired,
                    AnalyticsCookieAccepted = termsAndConditions.AnalyticsCookieAccepted,
                };

                _logger.LogDebug($"{nameof(termsAndConditions.ConsentGiven)}: {termsAndConditions.ConsentGiven}, " +
                                 $"{nameof(response.UpdatedConsentRequired)}: {updatedConsentRequired}, " +
                                 $"{nameof(termsAndConditions.AnalyticsCookieAccepted)}: {termsAndConditions.AnalyticsCookieAccepted}");

                return new TermsAndConditionsFetchConsentResult.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unsuccessful request to fetch patient consent");
                return new TermsAndConditionsFetchConsentResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<TermsAndConditionsRecordConsentResult> RecordConsent
        (
            string nhsLoginId,
            ConsentRequest request,
            DateTimeOffset consentTime
        )
        {
            return request.UpdatingConsent
                ? await UpdateConsent(nhsLoginId, request, consentTime)
                : await RecordInitialConsent(nhsLoginId, request, consentTime);
        }

        private async Task<TermsAndConditionsRecordConsentResult> RecordInitialConsent
        (
            string nhsLoginId,
            ConsentRequest request,
            DateTimeOffset consentTime
        )
        {
            _logger.LogEnter();
            try
            {
                var consentTimeString = consentTime.ToString("s", CultureInfo.InvariantCulture);

                var termsAndConditions = new TermsAndConditionsRecord
                {
                    NhsLoginId = nhsLoginId,
                    ConsentGiven = request.ConsentGiven,
                    AnalyticsCookieAccepted = request.AnalyticsCookieAccepted,
                    DateOfConsent = consentTimeString,
                    DateOfAnalyticsCookieToggle = consentTimeString
                };

                await _repository.Create(termsAndConditions);

                return new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unsuccessful request to record patient consent");
                return new TermsAndConditionsRecordConsentResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<TermsAndConditionsRecordConsentResult> UpdateConsent
        (
            string nhsLoginId,
            ConsentRequest request,
            DateTimeOffset consentTime
        )
        {
            _logger.LogEnter();

            try
            {
                var termsAndConditions = await _repository.Find(nhsLoginId);

                if (termsAndConditions == null)
                {
                    return new TermsAndConditionsRecordConsentResult.InternalServerError();
                }

                termsAndConditions.ConsentGiven = request.ConsentGiven;
                termsAndConditions.DateOfConsent = consentTime.ToString("s", CultureInfo.InvariantCulture);

                await _repository.Update(termsAndConditions);

                return new TermsAndConditionsRecordConsentResult.UpdateConsentRecorded();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unsuccessful request to update patient consent");
                return new TermsAndConditionsRecordConsentResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<ToggleAnalyticsCookieAcceptanceResult> ToggleAnalyticsCookieAcceptance
        (
            string nhsLoginId,
            AnalyticsCookieAcceptance analyticsCookieConsent,
            DateTimeOffset consentTime
        )
        {
            _logger.LogEnter();

            try
            {
                var termsAndConditions = await _repository.Find(nhsLoginId);

                if (termsAndConditions == null)
                {
                    _logger.LogError("Cannot find terms and conditions consent");
                    return new ToggleAnalyticsCookieAcceptanceResult.Failure();
                }

                termsAndConditions.AnalyticsCookieAccepted = analyticsCookieConsent.AnalyticsCookieAccepted;
                termsAndConditions.DateOfAnalyticsCookieToggle =
                    consentTime.ToString("s", CultureInfo.InvariantCulture);

                await _repository.Update(termsAndConditions);

                return new ToggleAnalyticsCookieAcceptanceResult.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unsuccessful request to toggle analytics cookie acceptance");
                return new ToggleAnalyticsCookieAcceptanceResult.Failure();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
