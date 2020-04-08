using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        public TppLinkedAccountsService(ILogger<TppLinkedAccountsService> logger, IGpSessionManager gpSessionManager)
        {
            _logger = logger;
            _gpSessionManager = gpSessionManager;
        }

        public string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            var tppUserSession = (TppUserSession)gpUserSession;
            return tppUserSession.OdsCode;
        }

        public async Task<SwitchAccountResult> SwitchAccount(GpUserSession gpUserSession, Guid id)
        {
            var proxy = GetProxyPatient(gpUserSession, id);
            var validId = proxy != null || gpUserSession.Id == id;

            if (!validId)
            {
                _logger.LogInformation("Unknown patient guid - could not find match on TppUserSession");
                return new SwitchAccountResult.Failure();
            }

            var patientId = proxy != null ? proxy.PatientId : ((TppUserSession)gpUserSession).PatientId;

            var result = await _gpSessionManager.RecreateSession(patientId);

            if (result is RecreateSessionResult.Failure)
            {
                _logger.LogInformation("Recreate TPP User Session failed");
                return new SwitchAccountResult.Failure();
            }

            _logger.LogInformation("Successfully Recreated TPP User Session - account switched");

            //TODO - next story...
            //await _gpSessionManager.LogoffSession(gpUserSession);

            return new SwitchAccountResult.Success();
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

         private static TppProxyUserSession GetLinkedAccountFromGpUserSession(GpUserSession gpUserSession, Guid linkedAccountId)
         {
             var tppUserSession = (TppUserSession)gpUserSession;
             var proxy = tppUserSession.ProxyPatients.FirstOrDefault(x => x.Id == linkedAccountId);
             return proxy;
         }

        public async Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            var response = new GetAccountAccessSummaryResponse
            {
                IsValidData = false,
            };
            return await Task.FromResult(new LinkedAccountAccessSummaryResult.Success(response));
        }

        public LinkedAccountAuditInfo GetProxyAuditData(GpUserSession gpUserSession, Guid id)
        {
            var linkedAccountAuditResult = new LinkedAccountAuditInfo();

            if (IsValidLinkedAccount(gpUserSession, id))
            {
                var tppUserSession = (TppUserSession)gpUserSession;

                linkedAccountAuditResult.IsProxyMode = true;
                linkedAccountAuditResult.ProxyNhsNumber
                    = tppUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id)?.NhsNumber;
            }

            return linkedAccountAuditResult;
        }

        private bool IsValidLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            return GetProxyPatient(gpUserSession, id) != null;
        }

        private TppProxyUserSession GetProxyPatient(GpUserSession gpUserSession, Guid id)
        {
            var tppUserSession = (TppUserSession) gpUserSession;
            return tppUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);
        }

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, Guid id)
        {
            var tppUserSession = (TppUserSession)gpUserSession;
            var proxy = tppUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);

            return proxy?.NhsNumber;
        }
    }
}