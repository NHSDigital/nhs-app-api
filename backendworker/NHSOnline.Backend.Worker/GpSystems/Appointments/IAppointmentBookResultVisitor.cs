namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public interface IAppointmentBookResultVisitor<out T>
    {
        T Visit(AppointmentBookResult.SuccessfullyBooked successfullyBooked);
        T Visit(AppointmentBookResult.InsufficientPermissions insufficientPermissions);
        T Visit(AppointmentBookResult.SlotNotAvailable slotNotAvailable);
        T Visit(AppointmentBookResult.SupplierSystemUnavailable supplierSystemUnavailable);
        T Visit(AppointmentBookResult.BadRequest badRequest);
    }
}