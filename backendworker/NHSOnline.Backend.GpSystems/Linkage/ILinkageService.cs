using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Linkage.Models;

namespace NHSOnline.Backend.GpSystems.Linkage
{
    public interface ILinkageService
    {
        Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest);
        
        Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest);
    }
}
