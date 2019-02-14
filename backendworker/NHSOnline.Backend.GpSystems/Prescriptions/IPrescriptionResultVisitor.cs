namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface IPrescriptionResultVisitor<out T>
    {
        T Visit(PrescriptionResult.SuccessfulGet result);
        T Visit(PrescriptionResult.SuccessfulPost result);
        T Visit(PrescriptionResult.SupplierSystemUnavailable result);
        T Visit(PrescriptionResult.SupplierNotEnabled result);
        T Visit(PrescriptionResult.InternalServerError result);
        T Visit(PrescriptionResult.BadRequest result);
        T Visit(PrescriptionResult.CannotReorderPrescription result);
        T Visit(PrescriptionResult.MedicationAlreadyOrderedWithinLast30Days result);
    }
}