using System.Threading.Tasks;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using NHSOnline.Backend.NominatedPharmacy.Models;

namespace NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces
{
    public interface INominatedPharmacySubmitClient
    {
        Task<NominatedPharmacyApiObjectResponse<NominatedPharmacyUpdateResponse>> UpdateNominatedPharmacy(NominatedPharmacyUpdateRequest nominatedPharmacyUpdateRequest);
    }
}
