using System.Linq;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    internal class RepositoryGetResultVisitor : IRepositoryFindResultVisitor<UserDevice, SearchDeviceResult>
    {
        public SearchDeviceResult Visit(RepositoryFindResult<UserDevice>.NotFound result)
        {
            return new SearchDeviceResult.NotFound();
        }

        public SearchDeviceResult Visit(RepositoryFindResult<UserDevice>.InternalServerError result)
        {
            return new SearchDeviceResult.InternalServerError();
        }

        public SearchDeviceResult Visit(RepositoryFindResult<UserDevice>.RepositoryError result)
        {
            return new SearchDeviceResult.BadGateway();
        }

        public SearchDeviceResult Visit(RepositoryFindResult<UserDevice>.Found result)
        {
            return new SearchDeviceResult.Found(result.Records.First());
        }
    }
}