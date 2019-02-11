using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    public class DetailedTestResultAuditingVisitor : IDetailedTestResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.GetTestResultAuditTypeResponse;
        
        public DetailedTestResultAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }

        public object Visit(GetDetailedTestResult.SuccessfullyRetrieved result)
        {
            _auditor.Audit(AuditType, "Test result successfully viewed");

            return null;
        }

        public object Visit(GetDetailedTestResult.SupplierBadData result)
        {
            _auditor.Audit(AuditType, "Error viewing test result: supplier bad data");

            return null;
        }

        public object Visit(GetDetailedTestResult.Unsuccessful result)
        {
            _auditor.Audit(AuditType, "Error viewing test result: unsuccessful");

            return null;
        }

    }
}