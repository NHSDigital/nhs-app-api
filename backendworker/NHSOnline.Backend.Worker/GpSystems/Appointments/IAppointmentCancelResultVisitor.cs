namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public interface IAppointmentCancelResultVisitor<out T>
    {
        T Visit(AppointmentCancelResult.SuccessfullyCancelled successfullyCancelled);
        T Visit(AppointmentCancelResult.BadRequest badRequest);
        T Visit(AppointmentCancelResult.AppointmentNotCancellable appointmentNotCancellable);
        T Visit(AppointmentCancelResult.TooLateToCancel tooLateToCancel);
        T Visit(AppointmentCancelResult.InsufficientPermissions insufficientPermissions);
        T Visit(AppointmentCancelResult.SupplierSystemUnavailable supplierSystemUnavailable);
    }
}