namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public interface IAllergyResultVisitor<out T>
    {
        T Visit(GetAllergyResult.UserHasNoAccess result);
        T Visit(GetAllergyResult.SuccessfullyRetrieved result);
        T Visit(GetAllergyResult.Unsuccessful result);
        T Visit(GetAllergyResult.SupplierBadData result);
    }
}