using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Linkage;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.VisionLinkageClient;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public interface IVisionLinkageClient
    {
        Task<VisionApiObjectResponse<LinkageKeyGetResponse>> GetLinkageKey(GetLinkageKey getLinkageKey);

        Task<VisionApiObjectResponse<LinkageKeyPostResponse>> CreateLinkageKey(CreateLinkageKey createLinkageKey);
    }
}
