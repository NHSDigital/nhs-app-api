using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    public class OrganDonationWithdrawAuditingVisitor : IOrganDonationWithdrawResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.OrganDonationWithdrawAuditTypeResponse;

        public OrganDonationWithdrawAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }
        
        public object Visit(OrganDonationWithdrawResult.SuccessfullyWithdrawn result)
        {
            _auditor.Audit(AuditType, "The organ donation decision has been successfully Withdrawn");

            return null;
        }
        
        public object Visit(OrganDonationWithdrawResult.SystemError result)
        {
            _auditor.Audit(AuditType, "There was an issue withdrawing the organ donation decision");

            return null;
        }

        public object Visit(OrganDonationWithdrawResult.UpstreamError result)
        {
            _auditor.Audit(AuditType, "There was an upstream error when withdrawing the organ donation decision");

            return null;
        }

        public object Visit(OrganDonationWithdrawResult.Timeout result)
        {
            _auditor.Audit(AuditType, "The organ donation Withdraw system took too long to respond");

            return null;
        }
    }
}
