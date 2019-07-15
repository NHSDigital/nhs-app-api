using System.Threading.Tasks;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyService
    {
        Task<GetNominatedPharmacyResult> GetNominatedPharmacy(string nhsNumber, CitizenIdUserSession cidUserSession);
        
        Task<UpdateNominatedPharmacyResult> UpdateNominatedPharmacy(NominatedPharmacyUpdate nominatedPharmacyUpdate);
    }
}
