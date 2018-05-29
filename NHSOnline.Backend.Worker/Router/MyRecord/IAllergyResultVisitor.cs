namespace NHSOnline.Backend.Worker.Router.MyRecord
{
    public interface IAllergyResultVisitor<out T>
    {
        T Visit(GetAllergyResult.SuccessfullyRetrieved result);
        T Visit(GetAllergyResult.Unsuccessful result);
        T Visit(GetAllergyResult.SupplierBadData result);
    }
}