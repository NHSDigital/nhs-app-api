namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public interface IMyRecordResultVisitor<out T>
    {
        T Visit(GetMyRecordResult.SuccessfullyRetrieved result);
        T Visit(GetMyRecordResult.Unsuccessful result);
        T Visit(GetMyRecordResult.SupplierBadData result);
    }
}