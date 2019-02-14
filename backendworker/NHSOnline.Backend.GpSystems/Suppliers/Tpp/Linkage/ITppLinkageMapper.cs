using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    public interface ITppLinkageMapper
    {
        LinkageResponse Map(AddNhsUserRequest addNhsUserRequest, AddNhsUserResponse addNhsUserResponse);
    }
}