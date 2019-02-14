using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public interface IVisionLinkageMapper
    {
        LinkageResponse Map(LinkageKeyGetResponse response);

        LinkageResponse Map(LinkageKeyPostResponse response);
    }
}
