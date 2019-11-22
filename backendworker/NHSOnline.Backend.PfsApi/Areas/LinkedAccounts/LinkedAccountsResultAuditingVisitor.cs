using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.LinkedAccounts;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    public class LinkedAccountsResultAuditingVisitor : ILinkedAccountsResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<LinkedAccountsController> _logger;
        private const string AuditType = AuditingOperations.GetLinkedAccountsResponse;

        public LinkedAccountsResultAuditingVisitor(IAuditor auditor, ILogger<LinkedAccountsController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(LinkedAccountsResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Linked Accounts retrieved successfully");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(LinkedAccountsResult)}");
            }
        }
    }
}
