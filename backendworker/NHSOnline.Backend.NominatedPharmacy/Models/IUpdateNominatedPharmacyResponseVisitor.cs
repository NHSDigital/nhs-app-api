namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public interface IUpdateNominatedPharmacyResponseVisitor<out T>
    {
        T Visit(UpdateNominatedPharmacyResponse.Success result);
        T Visit(UpdateNominatedPharmacyResponse.GetNominatedPharmacyFailure result);
        T Visit(UpdateNominatedPharmacyResponse.InternalServerError result);
        T Visit(UpdateNominatedPharmacyResponse.BadGateway result);
        T Visit(UpdateNominatedPharmacyResponse.UpdatedButStillOldCode result);
        T Visit(UpdateNominatedPharmacyResponse.NominatedPharmacyIsDisabled result);
        T Visit(UpdateNominatedPharmacyResponse.BadRequest result);
    }
}