using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage
{
    [FakeGpArea("Linkage")]
    public interface ILinkageAreaBehaviour
    {
        Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest);

        Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest);
    }
}