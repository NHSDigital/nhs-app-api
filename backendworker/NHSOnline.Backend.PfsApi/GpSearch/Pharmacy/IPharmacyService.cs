using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSearch.Models.Pharmacy;

namespace NHSOnline.Backend.Worker.GpSearch.Pharmacy
{
    public interface IPharmacyService
    {
        Task<PharmacyDetailResponse> GetPharmacyDetail(string odsCode);
    }
}
