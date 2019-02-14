namespace NHSOnline.Backend.GpSystems.Demographics
{
    public interface IDemographicsResultVisitor<out T>
    {
        T Visit(DemographicsResult.UserHasNoAccess result);
        T Visit(DemographicsResult.SuccessfullyRetrieved result);
        T Visit(DemographicsResult.SupplierSystemUnavailable supplierSystemUnavailable);
        T Visit(DemographicsResult.Unsuccessful result);
        T Visit(DemographicsResult.InternalServerError result);
    }
}