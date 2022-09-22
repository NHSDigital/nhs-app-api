using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.SecondaryCare;

namespace NHSOnline.Backend.PfsApi.Areas.SecondaryCare
{
    public class SecondaryCareWaitTimesResultAuditingVisitor : ISecondaryCareWaitTimesResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<SecondaryCareController> _logger;

        private const string AuditType = AuditingOperations.SecondaryCareGetWaitTimesResponse;

        public SecondaryCareWaitTimesResultAuditingVisitor(IAuditor auditor, ILogger<SecondaryCareController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(SecondaryCareWaitTimesResult.Success result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType,
                    "Secondary Care WaitTimes successfully retrieved. " +
                    $"Total Wait Times: {result.Response.WaitTimes.Count}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown auditing {AuditType} {Result}", AuditType, nameof(SecondaryCareWaitTimesResult.Success));
            }
        }

        public async Task Visit(SecondaryCareWaitTimesResult.BadGateway _)
            => await VisitError(nameof(SecondaryCareWaitTimesResult.BadGateway));

        public async Task Visit(SecondaryCareWaitTimesResult.Timeout _)
            => await VisitError(nameof(SecondaryCareWaitTimesResult.Timeout));

        public async Task Visit(SecondaryCareWaitTimesResult.NotEnabled _)
            => await VisitError(nameof(SecondaryCareWaitTimesResult.NotEnabled));

        private async Task VisitError(string error)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, $"Error retrieving Secondary Care WaitTimes: {error}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown auditing {AuditType} {Error}", AuditType, error);
            }
        }
    }
}