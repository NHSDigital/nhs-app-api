using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    public class MyRecordSectionAuditingVisitor : IMyRecordSectionResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.ViewPatientRecordSectionAuditTypeResponse;

        public MyRecordSectionAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }

        public object Visit(GetMyRecordSectionResult.SuccessfullyRetrieved result)
        {
            var section = result.Response.SectionName;

            _auditor.Audit(AuditType,
                $"Patient record {section} successfully retrieved.");

            return null;
        }

        public object Visit(GetMyRecordSectionResult.SupplierBadData result)
        {
            _auditor.Audit(AuditType, "Error: Supplier - bad data");
            return null;
        }

        public object Visit(GetMyRecordSectionResult.Unsuccessful result)
        {
            _auditor.Audit(AuditType, "Error: Unsuccessful");
            return null;
        }

        public object Visit(GetMyRecordSectionResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            _auditor.Audit(AuditType, "Error: Error processing security header");
            return null;
        }

        public object Visit(GetMyRecordSectionResult.InvalidUserCredentials invalidUserCredentials)
        {
            _auditor.Audit(AuditType, "Error: Invalid user credentials");
            return null;
        }

        public object Visit(GetMyRecordSectionResult.InvalidRequest invalidRequest)
        {
            _auditor.Audit(AuditType, "Error: Invalid request");
            return null;
        }

        public object Visit(GetMyRecordSectionResult.UnknownError unknownError)
        {
            _auditor.Audit(AuditType, "Error: Unknown error");
            return null;
        }

        public object Visit(GetMyRecordSectionResult.InternalServerError internalServerError)
        {
            _auditor.Audit(AuditType, "Error: Internal server error");
            return null;
        }
    }
}