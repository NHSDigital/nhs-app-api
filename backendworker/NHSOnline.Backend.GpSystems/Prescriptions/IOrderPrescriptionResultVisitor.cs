namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface IOrderPrescriptionResultVisitor<out T>
    {
        T Visit(OrderPrescriptionResult.Success result);
        T Visit(OrderPrescriptionResult.BadGateway result);
        T Visit(OrderPrescriptionResult.Forbidden result);
        T Visit(OrderPrescriptionResult.InternalServerError result);
        T Visit(OrderPrescriptionResult.BadRequest result);
        T Visit(OrderPrescriptionResult.CannotReorderPrescription result);
        T Visit(OrderPrescriptionResult.MedicationAlreadyOrderedWithinLast30Days result);
    }
}