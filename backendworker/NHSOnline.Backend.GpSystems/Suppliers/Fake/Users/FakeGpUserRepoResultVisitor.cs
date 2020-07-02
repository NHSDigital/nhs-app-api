using System;
using System.Linq;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users
{
    internal class FakeGpUserRepoResultVisitor : IRepositoryFindResultVisitor<FakeUser, FakeUser>
    {
        private readonly IServiceProvider _serviceProvider;

        public FakeGpUserRepoResultVisitor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public FakeUser Visit(RepositoryFindResult<FakeUser>.NotFound result)
        {
            return null;
        }

        public FakeUser Visit(RepositoryFindResult<FakeUser>.Found result)
        {
            var user = result.Records.FirstOrDefault();

            if (user is null)
            {
                return null;
            }

            user.ServiceProvider = _serviceProvider;

            return user;
        }

        public FakeUser Visit(RepositoryFindResult<FakeUser>.RepositoryError result)
        {
            return null;
        }
    }
}