namespace NHSOnline.Backend.Worker.GpSystems.Demographics
{
    public interface IMyRecordResultVisitor<out T>
    {
        T Visit(GetMyRecordResult.UserHasNoAccess result);
        T Visit(GetMyRecordResult.SuccessfullyRetrieved result);
        T Visit(GetMyRecordResult.SupplierSystemUnavailable supplierSystemUnavailable);
        T Visit(GetMyRecordResult.Unsuccessful result);
    }
}