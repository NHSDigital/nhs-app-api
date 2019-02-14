namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentsResultVisitor<out T>
    {
        T Visit(AppointmentsResult.SuccessfullyRetrieved result);
        T Visit(AppointmentsResult.BadRequest result);
        T Visit(AppointmentsResult.SupplierSystemUnavailable result);
        T Visit(AppointmentsResult.InternalServerError result);
        T Visit(AppointmentsResult.CannotViewAppointments result);
    }
}