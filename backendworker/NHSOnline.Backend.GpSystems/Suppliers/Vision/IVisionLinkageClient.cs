using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;
using static NHSOnline.Backend.GpSystems.Suppliers.Vision.VisionLinkageClient;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public interface IVisionLinkageClient
    {
        Task<VisionApiObjectResponse<LinkageKeyGetResponse>> GetLinkageKey(GetLinkageKey getLinkageKey);

        Task<VisionApiObjectResponse<LinkageKeyPostResponse>> CreateLinkageKey(CreateLinkageKey createLinkageKey);
    }
}
