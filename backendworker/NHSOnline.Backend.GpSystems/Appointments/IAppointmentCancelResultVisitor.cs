namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentCancelResultVisitor<out T>
    {
        T Visit(AppointmentCancelResult.Success result);
        T Visit(AppointmentCancelResult.BadRequest result);
        T Visit(AppointmentCancelResult.AppointmentNotCancellable result);
        T Visit(AppointmentCancelResult.TooLateToCancel result);
        T Visit(AppointmentCancelResult.Forbidden result);
        T Visit(AppointmentCancelResult.BadGateway result);
        T Visit(AppointmentCancelResult.InternalServerError result);
    }
}
