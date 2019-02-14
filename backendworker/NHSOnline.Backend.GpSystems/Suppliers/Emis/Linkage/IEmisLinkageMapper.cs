using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public interface IEmisLinkageMapper
    {
        LinkageResponse Map(AddVerificationResponse addVerificationResponse);
    }
}
