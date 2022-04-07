using System.Linq;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.Users.Repository
{
    internal class RepositoryFindResultVisitor : IRepositoryFindResultVisitor<UserDevice, FindRegistrationsResult>
    {
        public FindRegistrationsResult Visit(RepositoryFindResult<UserDevice>.NotFound result)
        {
            return new FindRegistrationsResult.NotFound();
        }

        public FindRegistrationsResult Visit(RepositoryFindResult<UserDevice>.RepositoryError result)
        {
            return new FindRegistrationsResult.BadGateway();
        }

        public FindRegistrationsResult Visit(RepositoryFindResult<UserDevice>.Found result)
        {
            return new FindRegistrationsResult.Found(result.Records.Select(s => s.RegistrationId).ToList());
        }
    }
}