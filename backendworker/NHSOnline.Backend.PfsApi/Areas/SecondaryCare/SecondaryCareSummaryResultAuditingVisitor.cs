using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.SecondaryCare;

namespace NHSOnline.Backend.PfsApi.Areas.SecondaryCare
{
    public class SecondaryCareSummaryResultAuditingVisitor : ISecondaryCareSummaryResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<SecondaryCareController> _logger;

        private const string AuditType = AuditingOperations.SecondaryCareGetSummaryResponse;

        public SecondaryCareSummaryResultAuditingVisitor(IAuditor auditor, ILogger<SecondaryCareController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(SecondaryCareSummaryResult.Success result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType,
                    "Secondary Care Summary successfully retrieved. " +
                    $"Total Referrals: {result.Response.Referrals.Count()}, " +
                    $"Total Upcoming Appointments: {result.Response.UpcomingAppointments.Count()}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(SecondaryCareSummaryResult.Success)}");
            }
        }

        public async Task Visit(SecondaryCareSummaryResult.BadGateway _)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error retrieving Secondary Care Summary: bad gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(SecondaryCareSummaryResult.BadGateway)}");
            }
        }
    }
}