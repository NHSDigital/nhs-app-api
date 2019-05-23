namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentSlotsResultVisitor<out T>
    {
        T Visit(AppointmentSlotsResult.Success result);
        T Visit(AppointmentSlotsResult.BadGateway result);
        T Visit(AppointmentSlotsResult.InternalServerError result);
        T Visit(AppointmentSlotsResult.Forbidden result);
    }
}
