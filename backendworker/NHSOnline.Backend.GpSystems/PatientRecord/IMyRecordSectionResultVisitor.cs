namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public interface IMyRecordSectionResultVisitor<out T>
    {
        T Visit(GetMyRecordSectionResult.Success result);

        T Visit(GetMyRecordSectionResult.BadGateway result);

        T Visit(GetMyRecordSectionResult.BadRequest result);

        T Visit(GetMyRecordSectionResult.InternalServerError result);
    }
}