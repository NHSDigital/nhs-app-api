namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentBookResultVisitor<out T>
    {
        T Visit(AppointmentBookResult.Success result);
        T Visit(AppointmentBookResult.Forbidden result);
        T Visit(AppointmentBookResult.SlotNotAvailable result);
        T Visit(AppointmentBookResult.BadGateway result);
        T Visit(AppointmentBookResult.BadRequest result);
        T Visit(AppointmentBookResult.AppointmentLimitReached result);
        T Visit(AppointmentBookResult.InternalServerError result);
    }
}
