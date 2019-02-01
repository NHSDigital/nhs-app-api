using NHSOnline.Backend.Worker.GpSystems.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Verifications;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage
{
    public interface IEmisLinkageMapper
    {
        LinkageResponse Map(AddVerificationResponse addVerificationResponse);
    }
}
