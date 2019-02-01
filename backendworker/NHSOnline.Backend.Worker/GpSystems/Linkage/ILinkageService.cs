using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Linkage.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public interface ILinkageService
    {
        Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest);
        
        Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest);
    }
}
