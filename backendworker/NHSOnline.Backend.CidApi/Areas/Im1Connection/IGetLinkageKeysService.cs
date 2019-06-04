using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    public interface IGetLinkageKeysService
    {
        Task<LinkageResult> GetLinkageKey(GetLinkageRequest request, IGpSystem gpSystem);
    }
}
