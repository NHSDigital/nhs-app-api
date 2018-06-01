namespace NHSOnline.Backend.Worker.Router.Demographics
{
    public interface IMyRecordResultVisitor<out T>
    {
        T Visit(GetMyRecordResult.SuccessfullyRetrieved result);
        T Visit(GetMyRecordResult.SupplierSystemUnavailable supplierSystemUnavailable);
        T Visit(GetMyRecordResult.Unsuccessful result);
    }
}