using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.Support.Auditing;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    public class GetPrescriptionResultAuditingVisitor :  IPrescriptionResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        
        private const string AuditType = Constants.AuditingTitles.RepeatPrescriptionsViewHistoryResponse;

        public GetPrescriptionResultAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }
        
        public async Task Visit(PrescriptionResult.SuccessfulGet result)
        {
            await _auditor.PostAudit(AuditType, $"Prescriptions successfully retrieved - { result.Response?.Prescriptions?.Select(x => x.Courses?.Count()).Sum() } courses");
        }

        public async Task Visit(PrescriptionResult.SuccessfulPost result)
        {
            await _auditor.PostAudit(AuditType, "Repeat prescription request successfully created");
        }

        public async Task Visit(PrescriptionResult.SupplierSystemUnavailable result)
        {
            await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Supplier Unavailable");
        }

        public async Task Visit(PrescriptionResult.SupplierNotEnabled result)
        {
            await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Supplier Not Enabled");
        }

        public async Task Visit(PrescriptionResult.InternalServerError result)
        {
            await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Internal Server Error");
        }

        public async Task Visit(PrescriptionResult.BadRequest result)
        {
            await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Bad Request");
        }

        public async Task Visit(PrescriptionResult.CannotReorderPrescription result)
        {
            await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Cannot Reorder Prescription");
        }

        public async Task Visit(PrescriptionResult.MedicationAlreadyOrderedWithinLast30Days result)
        {
            await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Medication already ordered within last 30 days");
        }
    }
}
