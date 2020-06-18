using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UsersApi.Repository
{
    internal class RepositoryCreateResultVisitor : IRepositoryCreateResultVisitor<UserDevice, RegisterDeviceResult>
    {
        public RegisterDeviceResult Visit(RepositoryCreateResult<UserDevice>.RepositoryError result)
        {
            return new RegisterDeviceResult.BadGateway();
        }

        public RegisterDeviceResult Visit(RepositoryCreateResult<UserDevice>.Created result)
        {
            return new RegisterDeviceResult.Created(result.Record);
        }
    }
}