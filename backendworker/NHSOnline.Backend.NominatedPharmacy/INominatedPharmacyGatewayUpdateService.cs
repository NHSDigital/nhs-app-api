using System.Threading.Tasks;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyGatewayUpdateService
    {
        Task<UpdateNominatedPharmacyResponse> UpdateNominatedPharmacy(string nhsNumber, string updatedOdsCode, CitizenIdUserSession cidUserSession);
    }
}
