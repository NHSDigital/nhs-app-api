using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    public class OrganDonationAuditingVisitor : IOrganDonationResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.GetOrganDonationAuditTypeResponse;

        public OrganDonationAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }

        public object Visit(OrganDonationResult.NewRegistration result)
        {
            _auditor.Audit(AuditType, "A default organ donation registration has been generated");

            return null;
        }

        public object Visit(OrganDonationResult.ExistingRegistration result)
        {
            _auditor.Audit(AuditType, "An existing organ donation registration been found");

            return null;
        }

        public object Visit(OrganDonationResult.DemographicsRetrievalFailed result)
        {
            _auditor.Audit(AuditType, "There was an issue retrieving the demographics record");

            return null;
        }

        public object Visit(OrganDonationResult.DuplicateRecord result)
        {
            _auditor.Audit(AuditType, "Duplicate organ donation record error");

            return null;
        }

        public object Visit(OrganDonationResult.SearchSystemUnavailable result)
        {
            _auditor.Audit(AuditType, "The organ donation system is unavailable");

            return null;
        }

        public object Visit(OrganDonationResult.BadSearchRequest result)
        {
            _auditor.Audit(AuditType, "The search request is invalid");

            return null;
        }

        public object Visit(OrganDonationResult.SearchTimeout result)
        {
            _auditor.Audit(AuditType, "The organ donation system took too long to respond");

            return null;
        }

        public object Visit(OrganDonationResult.SearchError result)
        {
            _auditor.Audit(AuditType, "There was an issue searching for an organ donation record");

            return null;
        }
    }
}
