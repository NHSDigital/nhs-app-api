using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public interface ILinkageService
    {
        Task<GetLinkageResult> GetLinkageKey(string nhsNumber, string odsCode);
        
        Task<CreateLinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest);
    }
}
