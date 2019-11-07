using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public interface IPharmacySearchResponseVisitor<out T>
    {
        T Visit(PharmacySearchResult.Success result);
        T Visit(PharmacySearchResult.InvalidPostcode result);
        T Visit(PharmacySearchResult.PostcodeResultFailure result);
        T Visit(PharmacySearchResult.BadRequest result);
        T Visit(PharmacySearchResult.InternalServerError result);
        T Visit(PharmacySearchResult.ModelValidationError result);
    }
}