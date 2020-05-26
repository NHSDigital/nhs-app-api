using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage
{
    public interface ILinkageBehaviour
    {
        Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest);

        Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest);
    }
}