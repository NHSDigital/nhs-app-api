namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface IGetPrescriptionsResultVisitor<out T>
    {
        T Visit(GetPrescriptionsResult.Success result);
        T Visit(GetPrescriptionsResult.BadGateway result);
        T Visit(GetPrescriptionsResult.Forbidden result);
        T Visit(GetPrescriptionsResult.InternalServerError result);
        T Visit(GetPrescriptionsResult.BadRequest result);
    }
}