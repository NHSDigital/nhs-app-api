namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public interface IDetailedTestResultVisitor<out T>
    {
        T Visit(GetDetailedTestResult.Success result);
        T Visit(GetDetailedTestResult.BadGateway result);
    }
}