using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public interface IFakeUserRepository
    {
        IFakeUser Find(string nhsNumber);
    }

    public class FakeUserRepository : IFakeUserRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FakeUserRepository> _logger;

        public FakeUserRepository(IServiceProvider serviceProvider, ILogger<FakeUserRepository> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public IFakeUser Find(string nhsNumber)
        {
            var users = _serviceProvider.GetServices<IFakeUser>();

            try
            {
                _logger.LogInformation($"Finding Fake User with NHS Number: '{nhsNumber}'.");
                return users.Single(u =>
                    u.NhsNumber.Equals(nhsNumber.RemoveWhiteSpace(), StringComparison.OrdinalIgnoreCase));
            }
            catch (InvalidOperationException exception)
            {
                var errorMessage = $"Failed to find Fake User with NHS Number: '{nhsNumber}'.";
                _logger.LogWarning(errorMessage);
                throw new UnknownFakeUserException(errorMessage, exception);
            }
        }
    }
}