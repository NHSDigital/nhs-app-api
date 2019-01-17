namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public interface IMyRecordSectionResultVisitor<out T>
    {
        T Visit(GetMyRecordSectionResult.SuccessfullyRetrieved result);

        T Visit(GetMyRecordSectionResult.Unsuccessful result);

        T Visit(GetMyRecordSectionResult.SupplierBadData result);

        T Visit(GetMyRecordSectionResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader);

        T Visit(GetMyRecordSectionResult.InvalidUserCredentials invalidUserCredentials);

        T Visit(GetMyRecordSectionResult.InvalidRequest invalidRequest);

        T Visit(GetMyRecordSectionResult.UnknownError unknownError);

        T Visit(GetMyRecordSectionResult.InternalServerError internalServerError);
    }
}