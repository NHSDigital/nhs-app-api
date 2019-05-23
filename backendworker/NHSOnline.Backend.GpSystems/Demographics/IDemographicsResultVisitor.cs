namespace NHSOnline.Backend.GpSystems.Demographics
{
    public interface IDemographicsResultVisitor<out T>
    {
        T Visit(DemographicsResult.Forbidden result);
        T Visit(DemographicsResult.Success result);
        T Visit(DemographicsResult.BadGateway result);
        T Visit(DemographicsResult.InternalServerError result);
    }
}