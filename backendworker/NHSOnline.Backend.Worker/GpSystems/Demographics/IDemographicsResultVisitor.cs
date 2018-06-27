namespace NHSOnline.Backend.Worker.GpSystems.Demographics
{
    public interface IDemographicsResultVisitor<out T>
    {
        T Visit(GetDemographicsResult.UserHasNoAccess result);
        T Visit(GetDemographicsResult.SuccessfullyRetrieved result);
        T Visit(GetDemographicsResult.SupplierSystemUnavailable supplierSystemUnavailable);
        T Visit(GetDemographicsResult.Unsuccessful result);
    }
}