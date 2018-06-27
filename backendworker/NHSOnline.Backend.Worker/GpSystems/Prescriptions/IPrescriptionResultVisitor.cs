namespace NHSOnline.Backend.Worker.GpSystems.Prescriptions
{
    public interface IPrescriptionResultVisitor<out T>
    {
        T Visit(PrescriptionResult.SuccessfullGet result);
        T Visit(PrescriptionResult.SuccessfullPost result);
        T Visit(PrescriptionResult.SupplierSystemUnavailable result);
        T Visit(PrescriptionResult.SupplierNotEnabled result);
        T Visit(PrescriptionResult.InternalServerError result);
        T Visit(PrescriptionResult.BadRequest result);
        T Visit(PrescriptionResult.CannotReorderPrescription result);
    }
}