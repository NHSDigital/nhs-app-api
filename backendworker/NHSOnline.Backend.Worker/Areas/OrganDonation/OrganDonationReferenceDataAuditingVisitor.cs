using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    public class OrganDonationReferenceDataAuditingVisitor : IOrganDonationReferenceDataResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.GetOrganDonationReferenceDataAuditTypeResponse;

        public OrganDonationReferenceDataAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }

        public object Visit(OrganDonationReferenceDataResult.SuccessfullyRetrieved result)
        {
            _auditor.Audit(AuditType, "The organ donation reference data has been retrieved successfully");

            return null;
        }
        
        public object Visit(OrganDonationReferenceDataResult.SystemError result)
        {
            _auditor.Audit(AuditType, "There was an issue getting the organ donation reference data");

            return null;
        }

        public object Visit(OrganDonationReferenceDataResult.UpstreamError result)
        {
            _auditor.Audit(AuditType, "There was an upstream error when getting the organ donation reference data");

            return null;
        }

        public object Visit(OrganDonationReferenceDataResult.Timeout result)
        {
            _auditor.Audit(AuditType, "The organ donation reference data system took too long to respond");

            return null;
        }
    }
}
