using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UsersApi.Repository
{
    internal class RepositoryUpdateResultVisitor : IRepositoryUpdateResultVisitor<UserDevice, UpdateDeviceResult>
    {
        public UpdateDeviceResult Visit(RepositoryUpdateResult<UserDevice>.Updated result)
        {
            return new UpdateDeviceResult.Updated();
        }

        public UpdateDeviceResult Visit(RepositoryUpdateResult<UserDevice>.NotFound result)
        {
            return new UpdateDeviceResult.NotFound();
        }

        public UpdateDeviceResult Visit(RepositoryUpdateResult<UserDevice>.RepositoryError result)
        {
            return new UpdateDeviceResult.InternalServerError();
        }

        public UpdateDeviceResult Visit(RepositoryUpdateResult<UserDevice>.NoChange result)
        {
            return new UpdateDeviceResult.Updated();
        }
    }
}