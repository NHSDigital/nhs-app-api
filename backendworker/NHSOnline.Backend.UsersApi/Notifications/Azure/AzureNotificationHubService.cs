using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Notifications.Azure.Steps;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure
{
    internal class AzureNotificationHubService : INotificationService
    {
        private readonly IRegistrationDescriptionFactory _registrationFactory;
        private readonly ILogger<AzureNotificationHubService> _logger;
        private readonly IOperationStepBuilder _stepBuilder;

        public AzureNotificationHubService(
            IRegistrationDescriptionFactory registrationFactory, 
            ILogger<AzureNotificationHubService> logger,
            IOperationStepBuilder stepBuilder)
        {
            _registrationFactory = registrationFactory;
            _logger = logger;
            _stepBuilder = stepBuilder;
        }
        
        public async Task<RegistrationResult> Register(NotificationRegistrationRequest request)
        {
            try
            {
                _logger.LogEnter();

                var registration = _registrationFactory.Create(request);

                return await _stepBuilder.Add<RemoveRegistrationsStep>()
                    .Add<CreateRegistrationIdStep>()
                    .Add<CreateOrUpdateRegistrationStep>()
                    .Execute(registration, request);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}