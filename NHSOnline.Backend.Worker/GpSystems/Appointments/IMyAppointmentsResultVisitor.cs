namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public interface IMyAppointmentsResultVisitor<out T>
    {
        T Visit(MyAppointmentsResult.SuccessfullyRetrieved result);
        T Visit(MyAppointmentsResult.BadRequest result);
        T Visit(MyAppointmentsResult.SupplierSystemUnavailable result);
        T Visit(MyAppointmentsResult.InternalServerError result);
    }
}