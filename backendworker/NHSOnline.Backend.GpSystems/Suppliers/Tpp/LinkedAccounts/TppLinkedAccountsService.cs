using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts
{
    public class TppLinkedAccountsService : ILinkedAccountsService
    {
        private readonly ILogger<TppLinkedAccountsService> _logger;
        private readonly IGpSessionManager _gpSessionManager;
        private readonly IFireAndForgetService _fireAndForgetService;

        public TppLinkedAccountsService(
            ILogger<TppLinkedAccountsService> logger,
            IGpSessionManager gpSessionManager,
            IFireAndForgetService fireAndForgetService)
        {
            _logger = logger;
            _gpSessionManager = gpSessionManager;
            _fireAndForgetService = fireAndForgetService;
        }

        public string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            var tppUserSession = (TppUserSession)gpUserSession;
            return tppUserSession.OdsCode;
        }

        public async Task<SwitchAccountResult> SwitchAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var tppUserSession = (TppUserSession) gpLinkedAccountModel.GpUserSession;

            if (tppUserSession.GetCurrentlyAuthenticatedId() == gpLinkedAccountModel.PatientId)
            {
                _logger.LogInformation("TPP user already authenticated");
                return new SwitchAccountResult.AlreadyAuthenticated(gpLinkedAccountModel.PatientId);
            }

            var proxy = GetProxyPatient(gpLinkedAccountModel);
            var validId = proxy != null || tppUserSession.Id == gpLinkedAccountModel.PatientId;

            if (!validId)
            {
                _logger.LogInformation("Unknown patient guid - could not find match on TppUserSession");
                return new SwitchAccountResult.NotFound(gpLinkedAccountModel.PatientId);
            }

            // The TppUserSession instance gets amended as part of re-creating a session, but
            // we still want a snapshot of the pre-amended version to close the old session with.
            var tppUserSessionBeforeRecreated = CloneTppUserSession(tppUserSession);

            var patientIdToSwitchTo = proxy != null ? proxy.PatientId : tppUserSession.PatientId;

            var result = await _gpSessionManager.RecreateSession(patientIdToSwitchTo);

            if (result is RecreateSessionResult.Failure)
            {
                _logger.LogInformation("Recreate TPP User Session failed");
                return new SwitchAccountResult.Failure(gpLinkedAccountModel.PatientId);
            }

            _fireAndForgetService.Run(
                (svcProvider) => CloseTppSession(svcProvider, tppUserSessionBeforeRecreated),
                "Failed to close TPP User Session");

            _logger.LogInformation("Successfully Recreated TPP User Session - account switched");

            return new SwitchAccountResult.Success();
        }

        private TppUserSession CloneTppUserSession(TppUserSession tppUserSession)
        {
            return JsonConvert.DeserializeObject<TppUserSession>(JsonConvert.SerializeObject(tppUserSession));
        }

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession)
        {
            var tppUserSession = (TppUserSession)gpUserSession;
            LinkedAccountsBreakdownSummary summary = new LinkedAccountsBreakdownSummary();

            if (tppUserSession.HasLinkedAccounts)
            {
                var linkedAccounts = new List<LinkedAccount>();

                foreach (var proxyPatient in tppUserSession.ProxyPatients)
                {
                    var dateOfBirth = proxyPatient.DateOfBirth;
                    linkedAccounts.Add(new LinkedAccount
                    {
                        Id = proxyPatient.Id,
                        GivenName = proxyPatient.FullName,
                        FullName = proxyPatient.FullName,
                        AgeMonths = CalculateAge.CalculateAgeInMonthsAndYears(dateOfBirth).AgeMonths,
                        AgeYears = CalculateAge.CalculateAgeInMonthsAndYears(dateOfBirth).AgeYears,
                        DisplayPersonalizedContent = false
                    });
                }
                summary = GroupLinkedAccounts(tppUserSession, linkedAccounts);
            }

            return await Task.FromResult(new LinkedAccountsResult.Success(summary, false));
        }

        private LinkedAccountsBreakdownSummary GroupLinkedAccounts(TppUserSession tppUserSession, IEnumerable<LinkedAccount> linkedAccounts)
        {
            var withoutNhsNumbers = new List<LinkedAccount>();
            var validAccounts = new List<LinkedAccount>();

            foreach (var account in linkedAccounts)
            {
                var accountInSession = tppUserSession.ProxyPatients.FirstOrDefault(pp => pp.Id == account.Id);

                if (!string.IsNullOrEmpty(accountInSession?.NhsNumber))
                {
                    validAccounts.Add(account);
                }
                else
                {
                    withoutNhsNumbers.Add(account);
                }
            }

            _logger.LogInformation($"Linked_profiles_count={linkedAccounts.Count()}, " +
                                   $"excluded_for_not_having_NHS_number={withoutNhsNumbers.Count}, " +
                                   $"excluding_for_having_different_ODS_code=0, " +
                                   $"valid_and_being_returned: {validAccounts.Count}");

            return new LinkedAccountsBreakdownSummary
            {
                ValidAccounts = validAccounts,
                AccountsWithNoNhsNumber = withoutNhsNumbers,
                AccountsWithMismatchingOdsCode = new List<LinkedAccount>(),
            };
        }

         public async Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            var response = new GetAccountAccessSummaryResponse
            {
                IsValidData = false,
            };
            return await Task.FromResult(new LinkedAccountAccessSummaryResult.Success(response));
        }

        public LinkedAccountAuditInfo GetProxyAuditData(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var linkedAccountAuditResult = new LinkedAccountAuditInfo();

            if (IsValidLinkedAccount(gpLinkedAccountModel))
            {
                var tppUserSession = (TppUserSession)gpLinkedAccountModel.GpUserSession;

                linkedAccountAuditResult.IsProxyMode = true;
                linkedAccountAuditResult.ProxyNhsNumber = tppUserSession.ProxyPatients
                    .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId)?.NhsNumber;
            }

            return linkedAccountAuditResult;
        }

        private bool IsValidLinkedAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return GetProxyPatient(gpLinkedAccountModel) != null;
        }

        private TppProxyUserSession GetProxyPatient(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var tppUserSession = (TppUserSession) gpLinkedAccountModel.GpUserSession;
            return tppUserSession.ProxyPatients
                .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId);
        }

        private async Task CloseTppSession(IServiceProvider serviceProvider, TppUserSession tppUserSession)
        {
            var scopedGpSessionManager = serviceProvider.GetService<IGpSessionManager>();
            var closeSessionResult = await scopedGpSessionManager.CloseSession(tppUserSession);

            if (closeSessionResult is CloseSessionResult.Success)
            {
                _logger.LogInformation(
                    $"Successfully closed TPP User Session for id {tppUserSession.GetCurrentlyAuthenticatedId()}");
            }
        }

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, Guid id)
        {
            var tppUserSession = (TppUserSession)gpUserSession;
            var proxy = tppUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);

            return proxy?.NhsNumber;
        }
    }
}