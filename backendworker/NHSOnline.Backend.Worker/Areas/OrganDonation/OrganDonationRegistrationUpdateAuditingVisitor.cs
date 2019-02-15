using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Worker.OrganDonation;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    public class OrganDonationRegistrationUpdateAuditingVisitor : IOrganDonationRegistrationResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.OrganDonationUpdateAuditTypeResponse;

        public OrganDonationRegistrationUpdateAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }
        
        public object Visit(OrganDonationRegistrationResult.SuccessfullyRegistered result)
        {
            _auditor.Audit(AuditType, "The organ donation decision has been successfully updated");

            return null;
        }
        
        public object Visit(OrganDonationRegistrationResult.SystemError result)
        {
            _auditor.Audit(AuditType, "There was an issue registering the organ donation decision update");

            return null;
        }

        public object Visit(OrganDonationRegistrationResult.UpstreamError result)
        {
            _auditor.Audit(AuditType, "There was an upstream error when registering the organ donation decision update");

            return null;
        }

        public object Visit(OrganDonationRegistrationResult.Timeout result)
        {
            _auditor.Audit(AuditType, "The organ donation registration update system took too long to respond");

            return null;
        }
    }
}
