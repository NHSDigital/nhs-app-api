using System.Threading.Tasks;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces
{
    public interface INominatedPharmacyPDSClient
    {
        Task<NominatedPharmacyApiObjectResponse<QUPA_IN000009UK03_Response>> NominatedPharmacyGet(QUPA_IN000008UK02 getNominatedPharmacyRequest);
    }
}
