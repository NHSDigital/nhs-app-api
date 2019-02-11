using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    public class MyRecordAuditingVisitor : IMyRecordResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.ViewPatientRecordAuditTypeResponse;

        public MyRecordAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }
        
        public object Visit(GetMyRecordResult.SuccessfullyRetrieved result)
        {
            var hasSummaryRecordAccess = result.Response.HasSummaryRecordAccess;
            var hasDetailedRecordAccess = result.Response.HasDetailedRecordAccess;
            
            _auditor.Audit(AuditType, 
                $"Patient record successfully retrieved. {nameof(hasSummaryRecordAccess)}={hasSummaryRecordAccess}," +
                $" {nameof(hasDetailedRecordAccess)}={hasDetailedRecordAccess}");
            
            return null;
        }

        public object Visit(GetMyRecordResult.SupplierBadData result)
        {
            _auditor.Audit(AuditType, "Error: Supplier - bad data");
            return null;
        }

        public object Visit(GetMyRecordResult.Unsuccessful result)
        {
            _auditor.Audit(AuditType, "Error: Unsuccessful");
            return null;
        }

        public object Visit(GetMyRecordResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            _auditor.Audit(AuditType, "Error: Error processing security header");
            return null;
        }

        public object Visit(GetMyRecordResult.InvalidUserCredentials invalidUserCredentials)
        {
            _auditor.Audit(AuditType, "Error: Invalid user credentials");
            return null;
        }

        public object Visit(GetMyRecordResult.InvalidRequest invalidRequest)
        {
            _auditor.Audit(AuditType, "Error: Invalid request");
            return null;
        }

        public object Visit(GetMyRecordResult.UnknownError unknownError)
        {
            _auditor.Audit(AuditType, "Error: Unknown error");
            return null;
        }

        public object Visit(GetMyRecordResult.InternalServerError internalServerError)
        {
            _auditor.Audit(AuditType, "Error: Internal server error");
            return null;
        }
    }
}