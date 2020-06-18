using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UsersApi.Repository
{
    internal class RepositoryDeleteResultVisitor : IRepositoryDeleteResultVisitor<UserDevice, DeleteDeviceResult>
    {
        private readonly string _deviceId;

        public RepositoryDeleteResultVisitor(string deviceId)
        {
            _deviceId = deviceId;
        }

        public DeleteDeviceResult Visit(RepositoryDeleteResult<UserDevice>.NotFound result)
        {
            return new DeleteDeviceResult.Success(_deviceId);
        }

        public DeleteDeviceResult Visit(RepositoryDeleteResult<UserDevice>.RepositoryError result)
        {
            return new DeleteDeviceResult.BadGateway();
        }

        public DeleteDeviceResult Visit(RepositoryDeleteResult<UserDevice>.Deleted result)
        {
            return new DeleteDeviceResult.Success(_deviceId);
        }
    }
}