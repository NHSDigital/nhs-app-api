using NHSOnline.Backend.Worker.GpSystems.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage
{
    public interface ITppLinkageMapper
    {
        LinkageResponse Map(AddNhsUserRequest addNhsUserRequest, AddNhsUserResponse addNhsUserResponse);
    }
}