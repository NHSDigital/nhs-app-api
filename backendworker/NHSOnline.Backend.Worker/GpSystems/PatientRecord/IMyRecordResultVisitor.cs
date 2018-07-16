namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public interface IMyRecordResultVisitor<out T>
    {
        T Visit(GetMyRecordResult.SuccessfullyRetrieved result);

        T Visit(GetMyRecordResult.Unsuccessful result);

        T Visit(GetMyRecordResult.SupplierBadData result);

        T Visit(GetMyRecordResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader);

        T Visit(GetMyRecordResult.InvalidUserCredentials invalidUserCredentials);

        T Visit(GetMyRecordResult.InvalidRequest invalidRequest);

        T Visit(GetMyRecordResult.UnknownError unknownError);
    }
}