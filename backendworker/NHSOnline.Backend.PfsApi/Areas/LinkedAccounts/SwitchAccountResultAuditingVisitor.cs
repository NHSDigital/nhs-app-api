using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.LinkedAccounts;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    public class SwitchAccountResultAuditingVisitor : ISwitchAccountResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<LinkedAccountsController> _logger;
        private const string AuditType = AuditingOperations.LinkedAccountsSwitchResponse;

        public SwitchAccountResultAuditingVisitor(IAuditor auditor, ILogger<LinkedAccountsController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(SwitchAccountResult.Success result)
        {
            try
            {
                await _auditor.Audit(
                    AuditType,
                    $"Successfully switched profile to NhsNumber {result.ToNhsNumber}");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(SwitchAccountResult.Success)}");
            }
        }

        public async Task Visit(SwitchAccountResult.AlreadyAuthenticated result)
        {
            try
            {
                await _auditor.Audit(AuditType,
                    $"Profile with id {result.AuthenticatedId} already authenticated");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(SwitchAccountResult.AlreadyAuthenticated)}");
            }
        }

        public async Task Visit(SwitchAccountResult.NotFound result)
        {
            try
            {
                await _auditor.Audit(AuditType, $"Couldn't find profile with id {result.AttemptedIdToSwitchTo} to switch to");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(SwitchAccountResult.NotFound)}");
            }
        }

        public async Task Visit(SwitchAccountResult.Failure result)
        {
            try
            {
                await _auditor.Audit(AuditType,
                    $"Error: Failed to switch to profile {result.AttemptedIdToSwitchTo}");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(SwitchAccountResult.Failure)}");
            }
        }
    }
}
