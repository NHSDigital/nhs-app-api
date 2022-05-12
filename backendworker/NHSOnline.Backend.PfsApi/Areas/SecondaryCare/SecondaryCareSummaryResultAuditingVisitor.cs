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
                var referralsNotInReviewCount = result.Response.ReferralsNotInReview.Count();
                var referralsInReviewCount = result.Response.ReferralsInReview.Count();
                var unconfirmedAppointments = result.Response.UnconfirmedAppointments.Count();
                var confirmedAppointmentsCount = result.Response.ConfirmedAppointments.Count();

                var totalReferralsCount = (referralsInReviewCount + referralsNotInReviewCount);
                var totalUpcomingAppointmentsCount = (unconfirmedAppointments + confirmedAppointmentsCount);

                await _auditor.PostOperationAudit(AuditType,
                    "Secondary Care Summary successfully retrieved. " +
                    $"Total Referrals: {totalReferralsCount}, " +
                    $"Total Upcoming Appointments: {totalUpcomingAppointmentsCount}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown auditing {AuditType} {Result}", AuditType, nameof(SecondaryCareSummaryResult.Success));
            }
        }

        public async Task Visit(SecondaryCareSummaryResult.BadGateway _)
            => await VisitError(nameof(SecondaryCareSummaryResult.BadGateway));

        public async Task Visit(SecondaryCareSummaryResult.Timeout _)
            => await VisitError(nameof(SecondaryCareSummaryResult.Timeout));

        private async Task VisitError(string error)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, $"Error retrieving Secondary Care Summary: {error}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown auditing {AuditType} {Error}", AuditType, error);
            }
        }
    }
}