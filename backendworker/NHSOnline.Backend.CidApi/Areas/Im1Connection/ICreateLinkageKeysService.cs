using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    public interface ICreateLinkageKeysService
    {   
        Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest request, IGpSystem gpSystem);
    }
}
