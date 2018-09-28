using System.Linq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    public class GetPrescriptionResultAuditingVisitor :  IPrescriptionResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        
        private const string AuditType = Constants.AuditingTitles.RepeatPrescriptionsViewHistoryResponse;

        public GetPrescriptionResultAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }
        
        public object Visit(PrescriptionResult.SuccessfulGet result)
        {
            _auditor.Audit(AuditType, $"Prescriptions successfully retrieved - { result.Response?.Prescriptions?.Select(x => x.Courses).Count() } courses");
            return null;
        }

        public object Visit(PrescriptionResult.SuccessfulPost result)
        {
            _auditor.Audit(AuditType, "Repeat prescription request successfully created");
            return null;
        }

        public object Visit(PrescriptionResult.SupplierSystemUnavailable result)
        {
            _auditor.Audit(AuditType, "Error retrieving prescriptions: Supplier Unavailable");
            return null;
        }

        public object Visit(PrescriptionResult.SupplierNotEnabled result)
        {
            _auditor.Audit(AuditType, "Error retrieving prescriptions: Supplier Not Enabled");
            return null;
        }

        public object Visit(PrescriptionResult.InternalServerError result)
        {
            _auditor.Audit(AuditType, "Error retrieving prescriptions: Internal Server Error");
            return null;
        }

        public object Visit(PrescriptionResult.BadRequest result)
        {
            _auditor.Audit(AuditType, "Error retrieving prescriptions: Bad Request");
            return null;
        }

        public object Visit(PrescriptionResult.CannotReorderPrescription result)
        {
            _auditor.Audit(AuditType, "Error retrieving prescriptions: Cannot Reorder Prescription");
            return null;
        }
    }
}
