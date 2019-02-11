using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Demographics
{
    public class DemographicsAuditingVisitor : IDemographicsResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.GetDemographicsAuditTypeResponse;

        public DemographicsAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }
        
        public object Visit(DemographicsResult.UserHasNoAccess result)
        {
            _auditor.Audit(AuditType, "Error viewing Demographics: patient does not have access to data");

            return null;
        }

        public object Visit(DemographicsResult.SuccessfullyRetrieved result)
        {
            _auditor.Audit(AuditType, "Demographics successfully viewed");

            return null;
        }

        public object Visit(DemographicsResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            _auditor.Audit(AuditType, "Error viewing Demographics: supplier system unavailable");

            return null;
        }

        public object Visit(DemographicsResult.Unsuccessful result)
        {
            _auditor.Audit(AuditType, "Error viewing Demographics: unsuccessful");

            return null;
        }

        public object Visit(DemographicsResult.InternalServerError result)
        {
            _auditor.Audit(AuditType, "Error viewing Demographics: internal server error");
            
            return null;
        }
    }
}