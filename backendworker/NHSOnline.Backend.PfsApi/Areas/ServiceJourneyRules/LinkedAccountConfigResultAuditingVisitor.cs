using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.LinkedAccounts;

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
                    auditMessage.Append($"{result.LinkedAccounts.Count()} linked accounts returned.");
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
