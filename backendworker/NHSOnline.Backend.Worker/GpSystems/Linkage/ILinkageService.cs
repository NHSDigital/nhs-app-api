using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public interface ILinkageService
    {
        Task<LinkageResult> GetLinkageKey(string nhsNumber, string odsCode, string identityToken);
        
        Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest);
    }
}
