namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public interface IAppointmentSlotsResultVisitor<out T>
    {
        T Visit(AppointmentSlotsResult.SuccessfullyRetrieved result);
        T Visit(AppointmentSlotsResult.BadRequest result);
        T Visit(AppointmentSlotsResult.SupplierSystemUnavailable result);
        T Visit(AppointmentSlotsResult.InternalServerError result);
    }
}
