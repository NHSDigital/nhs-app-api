using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceJourneyRules
{
    public class LinkedAccountConfigResultAuditingVisitor : ILinkedAccountsConfigResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<ServiceJourneyRulesController> _logger;

        private const string AuditType = AuditingOperations.GetPatientConfigResponse;

        public LinkedAccountConfigResultAuditingVisitor(IAuditor auditor, ILogger<ServiceJourneyRulesController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(LinkedAccountsConfigResult.Success result)
        {
            try
            {
                StringBuilder auditMessage = new StringBuilder("Returning patient config. ");

                if (result.SessionSettings.ProxyEnabled)
                {
                    auditMessage.Append($"{result.LinkedAccountsBreakdownSummary.ValidAccounts.Count()} linked accounts returned. " +
                        $"{result.LinkedAccountsBreakdownSummary.AccountsWithNoNhsNumber.Count()} excluded for no NHS number. " +
                        $"{result.LinkedAccountsBreakdownSummary.AccountsWithMismatchingOdsCode.Count()} excluded for mismatching ODS code.");
                }
                else
                {
                    auditMessage.Append("Proxy disabled");
                }

                await _auditor.Audit(AuditingOperations.GetPatientConfigResponse, auditMessage.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(LinkedAccountsConfigResult.Success)}");
            }
        }
    }
}
