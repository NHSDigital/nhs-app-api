namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public interface IPatientDocumentDownloadResultVisitor<out T>
    {
        T Visit(GetPatientDocumentDownloadResult.Success result);
        T Visit(GetPatientDocumentDownloadResult.BadGateway result);
    }
}