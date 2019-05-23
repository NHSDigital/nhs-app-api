namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public interface IMyRecordResultVisitor<out T>
    {
        T Visit(GetMyRecordResult.Success result);

        T Visit(GetMyRecordResult.BadGateway result);

        T Visit(GetMyRecordResult.InternalServerError result);
    }
}