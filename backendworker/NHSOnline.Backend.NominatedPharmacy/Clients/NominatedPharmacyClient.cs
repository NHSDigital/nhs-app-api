using System.Threading.Tasks;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using NHSOnline.Backend.NominatedPharmacy.Models;
using static NHSOnline.Backend.NominatedPharmacy.Soap.GetNominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy.Clients
{
    public class NominatedPharmacyClient : INominatedPharmacyClient
    {
        private readonly INominatedPharmacyPDSClient _nominatedPharmacyPDSClient;
        private readonly INominatedPharmacySubmitClient _nominatedPharmacySubmitClient;

        public NominatedPharmacyClient(
            INominatedPharmacyPDSClient nominatedPharmacyPDSClient,
            INominatedPharmacySubmitClient nominatedPharmacySubmitClient)
        {
            _nominatedPharmacyPDSClient = nominatedPharmacyPDSClient;
            _nominatedPharmacySubmitClient = nominatedPharmacySubmitClient;
        }

        public async Task<NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>> NominatedPharmacyGet(
            QUPAIN000008UK02 getNominatedPharmacyRequest)
        {
            return await _nominatedPharmacyPDSClient.NominatedPharmacyGet(getNominatedPharmacyRequest);
        }

        public async Task<UpdateNominatedPharmacyApiObjectResponse> UpdateNominatedPharmacy(NominatedPharmacyUpdateRequest nominatedPharmacyUpdateRequest)
        {
            return await _nominatedPharmacySubmitClient.UpdateNominatedPharmacy(nominatedPharmacyUpdateRequest);
        }
    }
}