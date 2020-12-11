using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceJourneyRules
{
    internal sealed class LinkedAccountPatientConfigVisitor: IUserSessionVisitor<Task<LinkedAccountsConfigResult>>
    {
        private readonly ILogger<ServiceJourneyRulesController> _logger;
        private readonly IAuditor _auditor;
        private readonly SessionConfigurationSettings _sessionSettings;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly GpUserSession _gpUserSession;

        public LinkedAccountPatientConfigVisitor(
            ILogger<ServiceJourneyRulesController> logger,
            IAuditor auditor,
            SessionConfigurationSettings sessionSettings,
            IGpSystemFactory gpSystemFactory,
            ISessionCacheService sessionCacheService,
            GpUserSession gpUserSession)
        {
            _logger = logger;
            _auditor = auditor;
            _sessionSettings = sessionSettings;
            _gpSystemFactory = gpSystemFactory;
            _sessionCacheService = sessionCacheService;
            _gpUserSession = gpUserSession;
        }

        public Task<LinkedAccountsConfigResult> Visit(P5UserSession userSession)
        {
            LinkedAccountsConfigResult result = new LinkedAccountsConfigResult.Success(
                Guid.Empty,
                _sessionSettings,
                Enumerable.Empty<LinkedAccount>());

            return Task.FromResult(result);
        }

        public async Task<LinkedAccountsConfigResult> Visit(P9UserSession userSession)
        {
            LinkedAccountsConfigResult result = new LinkedAccountsConfigResult.Success(
                Guid.Empty,
                _sessionSettings,
                Enumerable.Empty<LinkedAccount>());

            await _auditor.Audit(AuditingOperations.GetPatientConfigRequest, "Attempting to get config for patient");

            var validAccounts = Enumerable.Empty<LinkedAccount>();

            var gpSystem = _gpSystemFactory.CreateGpSystem(_gpUserSession.Supplier);

            if (gpSystem.SupportsLinkedAccounts)
            {
                var linkedAccountsService = gpSystem.GetLinkedAccountsService();

                var linkedAccountsResult = await linkedAccountsService.GetLinkedAccounts(_gpUserSession);

                if (linkedAccountsResult is LinkedAccountsResult.Success success)
                {
                    validAccounts = success.ValidAccounts;
                    if (success.HasAnyProxyInfoBeenUpdatedInSession)
                    {
                        _logger.LogInformation("Updating session as proxy info has been updated");
                        await _sessionCacheService.UpdateUserSession(userSession);
                    }
                }
            }

            result = new LinkedAccountsConfigResult.Success(
                userSession.GpUserSession.Id,
                _sessionSettings,
                validAccounts);

            await result.Accept(new LinkedAccountConfigResultAuditingVisitor(_auditor, _logger));

            return result;
        }
    }
}