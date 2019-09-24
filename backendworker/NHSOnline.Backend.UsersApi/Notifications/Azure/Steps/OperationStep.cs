using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure.Steps
{
    internal abstract class OperationStep : IOperationStep
    {
        protected readonly ILogger _logger;
        protected IOperationStep Next { get; private set; }

        protected OperationStep(ILogger logger)
        {
            _logger = logger;
        }
        
        public void NextStep(IOperationStep nextStep) => Next = nextStep;

        public abstract Task<RegistrationResult> Execute(RegistrationDescription registration, string devicePns);
    }
}