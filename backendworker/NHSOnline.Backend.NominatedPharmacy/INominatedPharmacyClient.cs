using System.Threading.Tasks;
using static NHSOnline.Backend.NominatedPharmacy.NominatedPharmacyClient;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyClient
    {
        Task<NominatedPharmacyApiObjectResponse<QUPA_IN000009UK03_Response>> NominatedPharmacyGet(QUPA_IN000008UK02 getNominatedPharmacyRequest);
    }
}
