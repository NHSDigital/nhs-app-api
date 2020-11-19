using System.Threading.Tasks;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    public class OrganDonationWithdrawAuditingVisitor : IOrganDonationWithdrawResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly IMetricLogger _metricLogger;
        private readonly P9UserSession _userSession;
        private const string AuditType = AuditingOperations.OrganDonationWithdrawAuditTypeResponse;

        public OrganDonationWithdrawAuditingVisitor(IAuditor auditor, IMetricLogger metricLogger,
            P9UserSession userSession)
        {
            _auditor = auditor;
            _metricLogger = metricLogger;
            _userSession = userSession;
        }

        public async Task Visit(OrganDonationWithdrawResult.SuccessfullyWithdrawn result)
        {
            await _auditor.Audit(AuditType, "The organ donation decision has been successfully Withdrawn");
            await _metricLogger.OrganDonationWithdrawRegistration(new OrganDonationData(_userSession.Key));
        }

        public async Task Visit(OrganDonationWithdrawResult.SystemError result)
        {
            await _auditor.Audit(AuditType, "There was an issue withdrawing the organ donation decision");
        }

        public async Task Visit(OrganDonationWithdrawResult.UpstreamError result)
        {
            await _auditor.Audit(AuditType, "There was an upstream error when withdrawing the organ donation decision");
        }

        public async Task Visit(OrganDonationWithdrawResult.Timeout result)
        {
            await _auditor.Audit(AuditType, "The organ donation Withdraw system took too long to respond");
        }

        public async Task Visit(OrganDonationWithdrawResult.BadRequest result)
        {
            await _auditor.Audit(AuditType, "The organ donation withdraw request failed validation");
        }
    }
}
