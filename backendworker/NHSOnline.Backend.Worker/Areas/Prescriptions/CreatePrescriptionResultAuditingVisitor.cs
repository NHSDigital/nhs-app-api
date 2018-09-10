using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    public class CreatePrescriptionResultAuditingVisitor :  IPrescriptionResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private readonly string _courseIds;
        
        private const string AuditType = Constants.AuditingTitles.RepeatPrescriptionsOrderRepeatMedicationsResponse;

        public CreatePrescriptionResultAuditingVisitor(IAuditor auditor, string courseIds)
        {
            _auditor = auditor;
            _courseIds = courseIds;
        }

        public object Visit(PrescriptionResult.SuccessfulGet result)
        {
            _auditor.Audit(AuditType, "Prescriptions successfully retrieved with course ids: {0}", _courseIds);
            return null;
        }

        public object Visit(PrescriptionResult.SuccessfulPost result)
        {
            _auditor.Audit(AuditType, "Repeat prescription request successfully created with course ids: {0}", _courseIds);
            return null;
        }

        public object Visit(PrescriptionResult.SupplierSystemUnavailable result)
        {
            _auditor.Audit(AuditType, "Error creating prescription request: Supplier Unavailable with course ids: {0}", _courseIds);
            return null;
        }

        public object Visit(PrescriptionResult.SupplierNotEnabled result)
        {
            _auditor.Audit(AuditType, "Error creating prescription request: Supplier Not Enabled with course ids: {0}", _courseIds);
            return null;
        }

        public object Visit(PrescriptionResult.InternalServerError result)
        {
            _auditor.Audit(AuditType, "Error creating prescription request: Internal Server Error with course ids: {0}", _courseIds);
            return null;
        }

        public object Visit(PrescriptionResult.BadRequest result)
        {
            _auditor.Audit(AuditType, "Error creating prescription request: Bad Request with course ids: {0}", _courseIds);
            return null;
        }

        public object Visit(PrescriptionResult.CannotReorderPrescription result)
        {
            _auditor.Audit(AuditType, "Error creating prescription request: Cannot Reorder Prescription with course ids: {0}", _courseIds);
            return null;
        }
    }
}
