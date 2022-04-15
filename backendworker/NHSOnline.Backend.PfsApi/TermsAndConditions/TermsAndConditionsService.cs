using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.Messages;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public class TermsAndConditionsService : ITermsAndConditionsService
    {
        private readonly ILogger<TermsAndConditionsService> _logger;
        private readonly ITermsAndConditionsRepository _repository;
        private readonly IMapper<TermsAndConditionsRecord, ConsentResponse> _termsAndConditionsToConsentMapper;
        private readonly IConsentRequestToTermsAndConditionsMapper _consentRequestToTermsAndConditionsMapper;
        private readonly IMapper<ConsentRequest, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>> _consentRequestToUpdateMapper;
        private readonly IMapper<AnalyticsCookieAcceptance, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>> _analyticsCookieAcceptanceToUpdateMapper;
        private readonly IIntroMessagesService _introMessagesService;
        private readonly IMetricLogger<UserSessionMetricContext> _metricLogger;

        public TermsAndConditionsService
        (
            ILogger<TermsAndConditionsService> logger,
            ITermsAndConditionsRepository repository,
            IMapper<TermsAndConditionsRecord, ConsentResponse> termsAndConditionsToConsentMapper,
            IConsentRequestToTermsAndConditionsMapper consentRequestToTermsAndConditionsMapper,
            IMapper<ConsentRequest, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>> consentRequestToUpdateMapper,
            IMapper<AnalyticsCookieAcceptance, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>> analyticsCookieAcceptanceToUpdateMapper,
            IIntroMessagesService introMessagesService,
            IMetricLogger<UserSessionMetricContext> metricLogger
        )
        {
            _logger = logger;
            _repository = repository;
            _termsAndConditionsToConsentMapper = termsAndConditionsToConsentMapper;
            _consentRequestToTermsAndConditionsMapper = consentRequestToTermsAndConditionsMapper;
            _consentRequestToUpdateMapper = consentRequestToUpdateMapper;
            _analyticsCookieAcceptanceToUpdateMapper = analyticsCookieAcceptanceToUpdateMapper;
            _introMessagesService = introMessagesService;
            _metricLogger = metricLogger;
        }

        public async Task<TermsAndConditionsFetchConsentResult> FetchConsent(string nhsLoginId)
        {
            _logger.LogEnter();

            try
            {
                var result = await _repository.Find(nhsLoginId);
                return result.Accept(new RepositoryGetConsentResponseVisitor(_logger, _termsAndConditionsToConsentMapper));
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
            if (request.UpdatingConsent)
            {
                return await UpdateConsent(nhsLoginId, request, consentTime);
            }

            var result = await RecordInitialConsent(nhsLoginId, request, consentTime);

            if (result is TermsAndConditionsRecordConsentResult.InitialConsentRecorded)
            {
                await _introMessagesService.SendIntroductoryMessage(nhsLoginId);
            }

            return result;
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
                var termsAndConditions = _consentRequestToTermsAndConditionsMapper.Map(request, consentTime, nhsLoginId);
                var result = await _repository.Create(termsAndConditions);
                return await result.Accept(new RepositoryCreateConsentResponseVisitor(_metricLogger));
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
                var updates = _consentRequestToUpdateMapper.Map(request, consentTime);
                var updateResult = await _repository.Update(nhsLoginId, updates);
                return updateResult.Accept(new RepositoryUpdateTermsAndConditionsResultVisitor());
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
                var update = _analyticsCookieAcceptanceToUpdateMapper.Map(analyticsCookieConsent, consentTime);
                var updateResult = await _repository.Update(nhsLoginId, update);
                return updateResult.Accept( new RepositoryUpdateAnalyticsCookieResultVisitor());
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
