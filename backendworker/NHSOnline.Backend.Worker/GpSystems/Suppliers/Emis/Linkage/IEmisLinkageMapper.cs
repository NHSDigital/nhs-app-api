using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage
{
    public interface IEmisLinkageMapper
    {
        LinkageResponse Map(LinkageDetailsResponse linkageDetailsResponse);
    }
}
