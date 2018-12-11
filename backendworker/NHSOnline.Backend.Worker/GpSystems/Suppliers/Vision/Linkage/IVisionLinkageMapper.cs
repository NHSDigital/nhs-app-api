using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Linkage
{
    public interface IVisionLinkageMapper
    {
        LinkageResponse Map(LinkageKeyGetResponse response);

        LinkageResponse Map(LinkageKeyPostResponse response);
    }
}
