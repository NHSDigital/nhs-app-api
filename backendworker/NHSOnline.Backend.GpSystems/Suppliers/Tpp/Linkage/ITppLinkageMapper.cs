using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    public interface ITppLinkageMapper
    {
        LinkageResponse Map(LinkAccount linkAccount, LinkAccountReply linkAccountReply);
    }
}
