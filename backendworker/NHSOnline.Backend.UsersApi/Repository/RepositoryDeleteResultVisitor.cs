using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UsersApi.Repository
{
    internal class RepositoryDeleteResultVisitor : IRepositoryDeleteResultVisitor<UserDevice, DeleteDeviceResult>
    {
        public DeleteDeviceResult Visit(RepositoryDeleteResult<UserDevice>.NotFound result)
        {
            return new DeleteDeviceResult.Success();
        }

        public DeleteDeviceResult Visit(RepositoryDeleteResult<UserDevice>.RepositoryError result)
        {
            return new DeleteDeviceResult.BadGateway();
        }

        public DeleteDeviceResult Visit(RepositoryDeleteResult<UserDevice>.Deleted result)
        {
            return new DeleteDeviceResult.Success();
        }
    }
}