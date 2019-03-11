using System.Threading.Tasks;
using NHSOnline.Backend.NominatedPharmacy.Models;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyService
    {
        Task<GetNominatedPharmacyResult> GetNominatedPharmacy(string nhsNumber);
    }
}
