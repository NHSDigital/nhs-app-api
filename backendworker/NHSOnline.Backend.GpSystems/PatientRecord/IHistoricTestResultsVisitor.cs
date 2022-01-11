namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public interface IHistoricTestResultsVisitor<out T>
    {
        T Visit(GetHistoricTestResultsResult.Success result);
        T Visit(GetHistoricTestResultsResult.BadGateway result);
    }
}
