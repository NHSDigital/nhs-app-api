namespace NHSOnline.Backend.Worker.Router.Prescriptions
{
    public interface IPrescriptionResultVisitor<out T>
    {
        T Visit(GetPrescriptionsResult.SuccessfullyRetrieved result);
        T Visit(GetPrescriptionsResult.Unsuccessful result);
        T Visit(GetPrescriptionsResult.SupplierBadData result);
    }
}
