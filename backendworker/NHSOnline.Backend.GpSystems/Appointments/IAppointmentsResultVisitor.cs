namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentsResultVisitor<out T>
    {
        T Visit(AppointmentsResult.Success result);
        T Visit(AppointmentsResult.BadRequest result);
        T Visit(AppointmentsResult.BadGateway result);
        T Visit(AppointmentsResult.InternalServerError result);
        T Visit(AppointmentsResult.Forbidden result);
    }
}