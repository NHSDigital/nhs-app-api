namespace NHSOnline.Backend.GpSystems.LinkedAccounts.Models
{
    public interface IPatientGuidResultVisitor<out T>
    {
        T Visit(GetPatientGuidResult.Success result);   
    }
}