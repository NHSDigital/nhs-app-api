using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    public class OrganDonationRegistrationAuditingVisitor : IOrganDonationRegistrationResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.OrganDonationRegistrationAuditTypeResponse;

        public OrganDonationRegistrationAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }
        
        public object Visit(OrganDonationRegistrationResult.SuccessfullyRegistered result)
        {
            _auditor.Audit(AuditType, "The organ donation decision has been successfully registered");

            return null;
        }
        
        public object Visit(OrganDonationRegistrationResult.SystemError result)
        {
            _auditor.Audit(AuditType, "There was an issue registering the organ donation decision");

            return null;
        }

        public object Visit(OrganDonationRegistrationResult.UpstreamError result)
        {
            _auditor.Audit(AuditType, "There was an upstream error when registering the organ donation decision");

            return null;
        }

        public object Visit(OrganDonationRegistrationResult.Timeout result)
        {
            _auditor.Audit(AuditType, "The organ donation registration system took too long to respond");

            return null;
        }
    }
}
