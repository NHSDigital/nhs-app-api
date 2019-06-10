using System.Threading.Tasks;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using static NHSOnline.Backend.NominatedPharmacy.Soap.GetNominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces
{
    public interface INominatedPharmacyPDSClient
    {
        Task<NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>> NominatedPharmacyGet(QUPAIN000008UK02 getNominatedPharmacyRequest);
    }
}
