using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyGatewayUpdateService
    {
        Task<StatusCodeResult> UpdateNominatedPharmacy(string nhsNumber, string updatedOdsCode);
    }
}
