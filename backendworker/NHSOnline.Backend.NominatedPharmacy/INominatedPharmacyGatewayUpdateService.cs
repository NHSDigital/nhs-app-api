using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyGatewayUpdateService
    {
        Task<StatusCodeResult> UpdateNominatedPharmacy(string nhsNumber, string updatedOdsCode, CitizenIdUserSession cidUserSession);
    }
}
