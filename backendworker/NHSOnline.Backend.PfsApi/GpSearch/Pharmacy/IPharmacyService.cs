using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;

namespace NHSOnline.Backend.PfsApi.GpSearch.Pharmacy
{
    public interface IPharmacyService
    {
        Task<PharmacyDetailResponse> GetPharmacyDetail(string odsCode);
    }
}
