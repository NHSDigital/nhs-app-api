using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    internal class RepositoryCreateResultVisitor : IRepositoryCreateResultVisitor<UserDevice, DeviceRegistrationResult>
    {
        public DeviceRegistrationResult Visit(RepositoryCreateResult<UserDevice>.RepositoryError result)
        {
            return new DeviceRegistrationResult.BadGateway();
        }

        public DeviceRegistrationResult Visit(RepositoryCreateResult<UserDevice>.Created result)
        {
            return new DeviceRegistrationResult.Created(result.Record);
        }
    }
}