using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public interface IPatientDocumentResultVisitor<out T>
    {
        T Visit(GetPatientDocumentResult.Success result);
        T Visit(GetPatientDocumentResult.BadGateway result);
    }
}