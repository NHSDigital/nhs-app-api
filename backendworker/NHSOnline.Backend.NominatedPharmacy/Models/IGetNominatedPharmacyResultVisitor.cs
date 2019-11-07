namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public interface IGetNominatedPharmacyResultVisitor<out T>
    {
        T Visit(GetNominatedPharmacyResult.Success result);
        T Visit(GetNominatedPharmacyResult.PersonalChecksFailed result);
        T Visit(GetNominatedPharmacyResult.PharmacyChecksFailed result);
        T Visit(GetNominatedPharmacyResult.PharmacyRetrievalFailure result);
        T Visit(GetNominatedPharmacyResult.NhsNumberSuperseded result);
        T Visit(GetNominatedPharmacyResult.ConfidentialAccount result);
        T Visit(GetNominatedPharmacyResult.InternalServerError result);
        T Visit(GetNominatedPharmacyResult.ConfigNotEnabled result);
        T Visit(GetNominatedPharmacyResult.GpPracticeEpsNotEnabled result);
        T Visit(GetNominatedPharmacyResult.GpPracticeFailure result);
        T Visit(GetNominatedPharmacyResult.NoNominatedPharmacy result);
        T Visit(GetNominatedPharmacyResult.PharmacyDetailFailure result);
        T Visit(GetNominatedPharmacyResult.InvalidPharmacySubtype result);
    }
}