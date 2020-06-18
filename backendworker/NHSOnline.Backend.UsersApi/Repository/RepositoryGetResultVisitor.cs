using System.Linq;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UsersApi.Repository
{
    internal class RepositoryGetResultVisitor : IRepositoryFindResultVisitor<UserDevice, SearchDeviceResult>
    {
        public SearchDeviceResult Visit(RepositoryFindResult<UserDevice>.NotFound result)
        {
            return new SearchDeviceResult.NotFound();
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