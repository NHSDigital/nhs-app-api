namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public interface IDetailedTestResultVisitor<out T>
    {
        T Visit(GetDetailedTestResult.SuccessfullyRetrieved result);
        T Visit(GetDetailedTestResult.Unsuccessful result);
        T Visit(GetDetailedTestResult.SupplierBadData result);
    }
}